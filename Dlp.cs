using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using PlayListLib.Model;

namespace PlayListLib
{
    public class Dlp
    {
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="isSavePlayPosition">是否记录播放进度</param>
        /// <param name="episodes">集合集</param>
        /// <param name="defaultPlayIndex">默认播放的集索引</param>
        public static Dlp Create(bool isSavePlayPosition, List<DlpEpisode> episodes = null, int defaultPlayIndex = 0) => new Dlp(isSavePlayPosition, episodes, defaultPlayIndex);
     
        public static Dlp Parse(string dlpPath)
        {
            var stream = new StreamReader(dlpPath);

            var result = new Dlp(true);
            var playname = string.Empty;

            while (!stream.EndOfStream)
            {
                var line = stream.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (Regex.Match(line, "^DAUMPLAYLIST", RegexOptions.IgnoreCase).Success) continue;
                if (Regex.Match(line, "^topindex", RegexOptions.IgnoreCase).Success) continue;
                if (Regex.Match(line, "^playname=(.+)", RegexOptions.IgnoreCase).Success)
                {
                    playname = line.Substring("playname=".Length);
                }
                if (Regex.Match(line, "^playtime", RegexOptions.IgnoreCase).Success) continue;
                if (Regex.Match(line, "^saveplaypos=1", RegexOptions.IgnoreCase).Success)
                {
                    result.IsSavePlayPosition = true; continue;
                }

                var file = Regex.Match(line, @"^(\d+)\*file\*(.+)", RegexOptions.IgnoreCase);
                if (file.Success)
                {
                    result.Episodes.Add(new DlpEpisode(file.Groups[2].Value) { Index = int.Parse(file.Groups[1].Value) });
                }

                var title = Regex.Match(line, @"^(\d+)\*title\*(.+)", RegexOptions.IgnoreCase);
                if (title.Success)
                {
                    var index = int.Parse(title.Groups[1].Value);
                    result.Episodes.FirstOrDefault(i => i.Index == index).Title = title.Groups[2].Value;
                }

                var duration2 = Regex.Match(line, @"^(\d+)\*duration2\*(\d+)", RegexOptions.IgnoreCase);
                if (duration2.Success)
                {
                    var index = int.Parse(duration2.Groups[1].Value);
                    result.Episodes.FirstOrDefault(i => i.Index == index).Duration = long.Parse(duration2.Groups[2].Value);
                }

                var start = Regex.Match(line, @"^(\d+)\*start\*(\d+)", RegexOptions.IgnoreCase);
                if (start.Success)
                {
                    var index = int.Parse(start.Groups[1].Value);
                    result.Episodes.FirstOrDefault(i => i.Index == index).Start = long.Parse(start.Groups[2].Value);
                }

                var played = Regex.Match(line, @"^(\d+)\*played\*1", RegexOptions.IgnoreCase);
                if (played.Success)
                {
                    var index = int.Parse(played.Groups[1].Value);
                    result.Episodes.FirstOrDefault(i => i.Index == index).IsPlayed = true;
                }
            }

            if (!string.IsNullOrEmpty(playname))
            {
                result.DefaultPlayEpisodeIndex = result.Episodes.FirstOrDefault(i => i.Url == playname)?.Index ?? 0;
            }

            stream.Close();
            stream.Dispose();

            return result;
        }


        /// <summary>
        /// 生成播放列表内容
        /// </summary>
        /// <returns>播放列表内容</returns>
        /// <exception cref="ArgumentNullException">集为空</exception>
        public string Builder()
        {
            if (Episodes?.Count == 0) throw new ArgumentNullException("集不能为空");
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(DAUMPLAYLIST);
            builder.AppendLine(TOPINDEX);
            builder.AppendLine(SAVEPLAYPOS);
            if (DefaultPlayEpisodeIndex > 0 && DefaultPlayEpisodeIndex < Episodes.Count + 1)
            {
                builder.AppendLine(PLAYNAME);
                builder.AppendLine(PLAYTIME.ToString());
            }

            var index = 1;
            foreach (var item in Episodes)
            {
                builder.AppendLine($"{index}*file*{item.Url}");
                builder.AppendLine($"{index}*title*{item.Title}");
                if (item.Duration > 0) builder.AppendLine($"{index}*duration2*{item.Duration}");
                if (item.Start > 0) builder.AppendLine($"{index}*start*{item.Start}");
                if (item.IsPlayed) builder.AppendLine($"{index}*played*1");
                index++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// 生成播放列表内容并保存到指定文件路径
        /// </summary>
        /// <param name="savePath">保存的路径</param>
        /// <returns>是否成功</returns>
        public bool BuilderDlpStringAndSaveFile(string savePath)
        {
            var content = Builder();
            try
            {
                File.WriteAllText(savePath, content);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="isSavePlayPosition">是否记录播放进度</param>
        /// <param name="episodes">集合集</param>
        /// <param name="defaultPlayIndex">默认播放的集索引</param>
        private Dlp(bool isSavePlayPosition, List<DlpEpisode> episodes = null, int defaultPlayIndex = 0)
        {
            IsSavePlayPosition = isSavePlayPosition;
            Episodes = episodes ?? new List<DlpEpisode>();
            DefaultPlayEpisodeIndex = defaultPlayIndex;
        }
        /* Hearder
       DAUMPLAYLIST
       topindex=0 未知，始终为0
       saveplaypos=0 记录播放位置，0 关闭 1 启用
       playname=启动时播放的路径
       playtime=播放进度（毫秒）
       */

        /* Body
          i*file*路径
          i*title*标题
          i*duration2*1000  持续时间（毫秒）
          i*start*1000  上次播放的位置继续 （毫秒）,需启用头部 saveplaypos=1
          i*played*1  播放完毕 
         */

        private string DAUMPLAYLIST => "DAUMPLAYLIST";
        private string TOPINDEX => "topindex=0";
        private string SAVEPLAYPOS => $"saveplaypos={(IsSavePlayPosition ? 1 : 0)}";
        public string PLAYNAME => DefaultPlayEpisodeIndex <= Episodes.Count ? Episodes[DefaultPlayEpisodeIndex - 1].Url : "";
        public long PLAYTIME => DefaultPlayEpisodeIndex <= Episodes.Count ? Episodes[DefaultPlayEpisodeIndex - 1].Start : 0;

        /// <summary>
        /// 是否记录播放位置
        /// </summary>
        public bool IsSavePlayPosition { get; set; } = false;// saveplaypos=0 记录播放位置，0 关闭 1 启用
        /// <summary>
        /// 集数索引，起始1
        /// </summary>
        public int DefaultPlayEpisodeIndex { get; set; } = 1;

        public List<DlpEpisode> Episodes { get;  }

    }
}
