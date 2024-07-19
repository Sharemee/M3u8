using System.CommandLine;
using System.Diagnostics;
using M3u8;
using M3u8.Entities;

var rootCommand = new RootCommand("M3u8 manager");

// init 命令
var initCommand = new Command("init", "Init something");
initCommand.SetHandler(() =>
{
    // 确保数据库已经生成
    try
    {
        M3u8DbContext dbContext = new();
        dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
});
rootCommand.AddCommand(initCommand);

// add 命令
var addCommand = new Command("add", "Add m3u8 info");
var addArg = new Argument<string>("A m3u8 arg or text file")
{
    Arity = ArgumentArity.ExactlyOne
};
addCommand.AddArgument(addArg);
var addOption = new Option<string?>("--type", "Download file type, such as: .mp4")
{
    Arity = ArgumentArity.ZeroOrOne
};
addCommand.AddOption(addOption);
addCommand.SetHandler((arg, type) =>
{
    // 检查参数
    if (string.IsNullOrEmpty(arg))
    {
        Console.WriteLine("参数 'arg' 不能为空");
        return;
    }

    // 读取文件中的参数
    List<string> urls = [];
    if (arg.EndsWith(".txt"))
    {
        FileInfo fi = new(arg);
        if (fi.Exists == false)
        {
            Console.Write("文件不存在");
            return;
        }
        string[] lines = File.ReadAllLines(arg);
        if (lines.Length == 0)
        {
            Console.WriteLine("文件内容为空");
            return;
        }
        urls.AddRange(lines);
    }
    else
    {
        urls.Add(arg);
    }

    // 循环写入数据库
    int addCount = 0;
    int existCount = 0;
    int totalCount = urls.Count;
    M3u8DbContext dbContext = new();
    foreach (var url in urls)
    {
        // 跳过空行
        if (string.IsNullOrEmpty(url))
        {
            totalCount--;
            continue;
        }
        // 检查是否是重复数据
        bool isExists = dbContext.Data.Any(x => x.Url == url);
        if (isExists == false)
        {
            dbContext.Data.Add(new Data()
            {
                Url = url,
                CreateTime = DateTime.Now,
                TargetFileType = type ?? ".mp4",
            });
        }
        else
        {
            existCount++;
        }
    }
    addCount = dbContext.SaveChanges();
    Console.WriteLine($"已添加: {addCount}, 已存在: {existCount}, 总数量: {totalCount}");
}, addArg, addOption);
rootCommand.AddCommand(addCommand);

// download 命令
var downloadCommand = new Command("download", "Download m3u8 media");
var downloadArg = new Argument<int?>("Max download context, default 1")
{
    Arity = ArgumentArity.ZeroOrOne,
};
downloadCommand.AddArgument(downloadArg);
downloadCommand.SetHandler(async (downloadArg) =>
{
    // 检查参数
    if (downloadArg.HasValue == false)
    {
        downloadArg = 1;
    }
    if (downloadArg < 0 || downloadArg > 100)
    {
        Console.WriteLine("警告: 下载数量大于100, 将重新设置为 1");
        downloadArg = 1;
    }

    // 创建数据库访问实例
    M3u8DbContext dbContext = new();
    var list = dbContext.Data.Where(x => x.IsDownloaded == false).Take(downloadArg.Value).ToList();

    if (list.Count == 0)
    {
        Console.WriteLine("没有需要下载的数据");
        return;
    }

    HttpClient client = new();
    string downloadPath = App.DownloadPath;
    foreach (var item in list)
    {
        // 生成下载的文件路径
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string fileName = $"{timestamp}{item.TargetFileType}";
        string fullPath = Path.Combine(downloadPath, fileName);

        // 检查文件是否存在
        FileInfo fi = new(fullPath);
        item.TargetFile = fi.FullName;
        dbContext.SaveChanges();
        if (fi.Exists)
        {
            item.DownloadMessage = "文件已存在";
            item.IsDownloaded = false;
            item.UpdateTime = DateTime.Now;
            dbContext.SaveChanges();
            continue;
        }

        // 下载 m3u8 文件, 并将内容保存到数据库
        try
        {
            var response = await client.GetAsync(item.Url);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                item.Content = content;
                item.UpdateTime = DateTime.Now;
                item.Hash = MD5Encryption.ComputeHash(content);
                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("下载 m3u8 文件时出错:");
            Console.WriteLine(ex.ToString());
        }

        // 调用 ffmpeg 下载 m3u8
        Process process = new()
        {
            StartInfo = GetPSI(item)
        };
        process.Start();
        process.WaitForExit();
        await process.WaitForExitAsync();
        int result = process.ExitCode;
        if (result == 0)
        {

            item.IsDownloaded = true;
            item.DownloadMessage = result.ToString();
        }
        else
        {
            string errorMessage = process.StandardError.ReadToEnd();
            item.DownloadMessage = errorMessage;
        }
        item.DownloadTime = DateTime.Now;
        item.UpdateTime = DateTime.Now;
        dbContext.SaveChanges();
    }
}, downloadArg);
rootCommand.AddCommand(downloadCommand);

return await rootCommand.InvokeAsync(args);

static ProcessStartInfo GetPSI(Data data)
{
    string argument = $"-i {data.Url} {data.TargetFile}";
    ProcessStartInfo psi = new("ffmpeg", argument);
    return psi;
}
