using System;
using System.Globalization;

namespace MusicLib
{
    public class MusicFormatProvider : IFormatProvider, ICustomFormatter
    {
        IFormatProvider _parent;
        public MusicFormatProvider() : this(CultureInfo.CurrentCulture) { }
        public MusicFormatProvider(IFormatProvider parent)
        {
            _parent = parent;
        }

        //0:PlayList Remains Count
        //1:Playing
        //2:Next
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg != null)
            {
                if (arg is PlayItem item)
                {
                    switch (format.ToLower())
                    {
                        case "title":
                            return item.Title;
                        case "author":
                            return item.Author;
                        case "id":
                            return item.Music.Id;
                        case "url":
                            return item.Music.Url;
                        case "api":
                            return item.Music.ApiType.ToString();
                        case "user":
                            return item.User;
                        case "current":
                            return (item.GetPlayableAudio()?.CurrentTimeString) ?? "00:00";
                        case "length":
                            return (item.GetPlayableAudio()?.TotalTimeString) ?? "00:00";
                        case "remains":
                            return (item.GetPlayableAudio()?.RemainingTimeString) ?? "00:00";
                        default:
                            break;
                    }
                }
            }
            return string.Format(_parent, "{0:" + format + "}", arg);
        }

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }
    }
}
