namespace M3u8.Entities;

/// <summary>
/// m3u8 管理数据
/// </summary>
public class Data
{
    /// <summary>
    /// 主键
    /// </summary>
    public int  Id { get; set; }

    /// <summary>
    /// m3u8 链接地址
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// m3u8 文件内容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// m3u8 文件内容哈希
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// 是否已下载
    /// </summary>
    public bool IsDownloaded { get; set; }

    /// <summary>
    /// 下载的文件地址
    /// </summary>
    public string TargetFile { get; set; } = string.Empty;

    /// <summary>
    /// 下载的文件类型
    /// </summary>
    public string TargetFileType { get; set; } = null!;

    /// <summary>
    /// 下载时间
    /// </summary>
    public DateTime? DownloadTime { get; set; }

    /// <summary>
    /// 下载消息, 也可能是错误消息
    /// </summary>
    public string? DownloadMessage { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
