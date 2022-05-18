using PlayListLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PlayListLib
{
    public class M3u
    {
        public static M3u Create() => new M3u();
        public static M3u Parse(Stream stream)
        {
            var readrStrem = new StreamReader(stream);

            var result = new M3u();

            M3uEpisode tempM3uEpisode = null;
            while (!readrStrem.EndOfStream)
            {
                var line = readrStrem.ReadLine();

                if (line.StartsWith("#EXTM3U")) continue;
                if (line.StartsWith("#") && tempM3uEpisode == null)
                {
                    tempM3uEpisode = new M3uEpisode();
                }

                if (line.StartsWith("#EXTINF"))
                {
                    tempM3uEpisode.Name = GetAttribute("tvg-name", line);
                    tempM3uEpisode.Logo = GetAttribute("tvg-logo", line);
                    tempM3uEpisode.SmallLogo = GetAttribute("tvg-logo-small", line);
                    tempM3uEpisode.Id = GetAttribute("tvg-id", line);
                    tempM3uEpisode.Group = GetAttribute("group-title", line);
                    tempM3uEpisode.Code = GetAttribute("parent-code", line);

                    Console.WriteLine(tempM3uEpisode.Logo);

                    var titleIndex = line.IndexOf(',');
                    if (titleIndex > -1) tempM3uEpisode.Title = line.Substring(titleIndex + 1).Trim();

                    continue;
                }

                if (!line.StartsWith("#") && tempM3uEpisode != null)
                {
                    tempM3uEpisode.Url = line;
                    tempM3uEpisode.Title = string.IsNullOrWhiteSpace(tempM3uEpisode.Title) ? Path.GetFileNameWithoutExtension(tempM3uEpisode.Url)?.Trim() : tempM3uEpisode.Title;
                    result.Episodes.Add(tempM3uEpisode.Clone());
                    tempM3uEpisode = null;
                }
            }

            stream.Close();
            stream.Dispose();
            readrStrem.Close();
            readrStrem.Dispose();

            return result;
        }

        public static M3u Parse(string path) => Parse(File.OpenRead(path));

        public string Builder()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("#EXTM3U");
            foreach (var item in Episodes)
            {
                builder.AppendLine($"#EXTINF:-1" +
                    $"{GetEXTINFValue("tvg-name", item.Name)}" +
                    $"{GetEXTINFValue("tvg-logo", item.Logo)}" +
                    $"{GetEXTINFValue("tvg-logo-small", item.SmallLogo)}" +
                    $"{GetEXTINFValue("tvg-id", item.Id)}" +
                    $"{GetEXTINFValue("group-title", item.Group)}" +
                    $"{GetEXTINFValue("parent-code", item.Code)}" +
                    $",{item.Title.Replace(",", "")}");
                builder.AppendLine(item.Url);
            }
            return builder.ToString();
        }

        public List<M3uEpisode> Episodes { get; } = new List<M3uEpisode>();



        private static string GetAttribute(string tvgName, string lineString)
        {
            var match = Regex.Match(lineString, $"{tvgName}=\"([^\"]+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static string GetEXTINFValue(string tvgName, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : @$" {tvgName}=""{value}""";
        }

    }
}
