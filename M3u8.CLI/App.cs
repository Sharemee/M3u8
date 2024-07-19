using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3u8;

internal class App
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    public const string APP_NAME = "Bit.M3u8";

    /// <summary>
    /// 数据库名称
    /// </summary>
    public const string DB_FILE_ANME = "m3u8.db";

    /// <summary>
    /// 下载目录名称
    /// </summary>
    public const string DOWNLOAD_DIR_NAME = ".m3u8";

    private static string? _appDataPath = null;

    /// <summary>
    /// 数据存储目录路径
    /// </summary>
    public static string AppDataPath
    {
        get
        {
            if (_appDataPath is null)
            {
                string path1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _appDataPath = Path.Combine(path1, APP_NAME);
            }
            DirectoryInfo di = new(_appDataPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            return _appDataPath;
        }
    }

    private static string? _databasePath = null;
    /// <summary>
    /// 数据库路径
    /// </summary>
    public static string DatabasePath
    {
        get
        {
            _databasePath ??= Path.Combine(AppDataPath, DB_FILE_ANME);
            return _databasePath;
        }
    }

    private static string? _downloadPath = null;

    /// <summary>
    /// 下载路径
    /// </summary>
    public static string DownloadPath
    {
        get
        {
            if (_downloadPath is null)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                _downloadPath = Path.Combine(userProfile, "Downloads", DOWNLOAD_DIR_NAME);
            }
            DirectoryInfo di = new(_downloadPath);
            if (di.Exists == false)
            {
                di.Create();
            }
            return _downloadPath;
        }
    }
}
