using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicLib
{
    public static class Utils
    {
        public static string GetProgramPath()
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RU-Danmaku-MusicPlayer");

        public static readonly Regex UrlRegex = new Regex(@"((http|ftp|https)://)((\[::\])|([a-zA-Z0-9\._-]+(\.[a-zA-Z]{2,6})?)|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?((/[a-zA-Z0-9\._-]+|/)*(\?[a-zA-Z0-9\&%_\./-~-]*)?)?");

        public static string ToUrlParameter(Dictionary<string, object> param = null)
        {
            if (param == null)
                return string.Empty;
            var sb = new List<string>();
            foreach (var p in param)
                sb.Add($"{UrlEncode(p.Key)}={UrlEncode(p.Value == null ? string.Empty : p.Value.ToString())}");
            return string.Join("&", sb);
        }

        public static readonly DateTime BaseUTC = new DateTime(1970, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime GetDateTime(double ts)
            => BaseUTC.AddSeconds(ts);

        public static long GetTimeStamp()
            => (long)(DateTime.UtcNow - BaseUTC).TotalSeconds;

        static Regex _UrlEncodeParser = new Regex("%[a-f0-9]{2}");
        public static string UrlEncode(string url)
        {
            var encode = System.Web.HttpUtility.UrlEncode(url, Encoding.UTF8);
            return _UrlEncodeParser.Replace(encode, (s) => s.Value.ToUpper());
        }
    }
}
