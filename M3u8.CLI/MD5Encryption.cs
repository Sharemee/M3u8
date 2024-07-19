using System.Security.Cryptography;
using System.Text;

namespace M3u8;

/// <summary>
/// MD5 加密类
/// </summary>
public class MD5Encryption
{
    /// <summary>
    /// 计算字符串哈希值
    /// </summary>
    /// <param name="content">用于计算哈希值的字符串</param>
    /// <param name="encoding">指定字符串编码, 默认: <see cref="Encoding.UTF8"/></param>
    /// <param name="lowercase">是否使用小写哈希值, 默认: <see langword="false"/></param>
    /// <returns>返回 MD5 哈希字符串</returns>
    public static string ComputeHash(string content, Encoding? encoding = null, bool lowercase = false)
    {
        var bytes = encoding?.GetBytes(content) ?? Encoding.UTF8.GetBytes(content);
        return ComputeHash(bytes, lowercase);
    }

    /// <summary>
    /// 计算字节数组哈希值
    /// </summary>
    /// <param name="buffer">用于计算哈希值的字节数组</param>
    /// <param name="lowercase">是否使用小写哈希值, 默认: <see langword="false"/></param>
    /// <returns>返回 MD5 哈希字符串</returns>
    public static string ComputeHash(byte[] buffer, bool lowercase = false)
    {
        byte[] hash;
#if NETSTANDARD
        using MD5 md5 = MD5.Create();
        hash = md5.ComputeHash(buffer);
#else
        hash = MD5.HashData(buffer);
#endif
        var sb = new StringBuilder(32);
        string fmt = lowercase ? "{0:x2}" : "{0:X2}";
        for (int i = 0; i < hash.Length; i++)
        {
            sb.AppendFormat(fmt, hash[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// MD5 哈希比较
    /// </summary>
    /// <param name="content">用于计算哈希值的字符串</param>
    /// <param name="hash">MD5哈希值</param>
    /// <param name="encoding">指定字符串编码, 默认: <see cref="Encoding.UTF8"/></param>
    /// <returns>若传入的哈希值和计算结果一致, 则返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
    public static bool Compare(string content, string hash, Encoding? encoding = null)
    {
        string calcHash = ComputeHash(content, encoding);
        return PrivateCompare(calcHash, hash);
    }

    /// <summary>
    /// MD5 哈希比较
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="hash"></param>
    /// <returns>若传入的哈希值和计算结果一致, 则返回 <see langword="true"/>, 否则返回 <see langword="false"/></returns>
    public static bool Compare(byte[] buffer, string hash)
    {
        string calcHash = ComputeHash(buffer);
        return PrivateCompare(calcHash, hash);
    }

    private static bool PrivateCompare(string calcHash, string inputHash)
    {
        // 16位的哈希
        if (inputHash.Length == 16)
        {
            calcHash = calcHash.Substring(8, 16);
        }
        return calcHash.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
    }
}
