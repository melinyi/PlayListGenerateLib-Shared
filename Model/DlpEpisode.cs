using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlayListLib.Model
{
    public class DlpEpisode
    {
        public DlpEpisode(string url, string title = default, bool isPlayed = false, long duration = 0, long start = 0)
        {
            Url = url.Trim();
            Title = string.IsNullOrWhiteSpace(title) ? Path.GetFileNameWithoutExtension(Url) : title;
            IsPlayed = isPlayed;
            Duration = duration;
            Start = start;
        }
        public string Title { get; set; }
        /// <summary>
        /// 播放地址，可以是文件或链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 集数索引，起始1
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        ///持续时间，毫秒 Unit:millisencond
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        ///默认开始播放的位置，毫秒 Unit:millisencond
        /// </summary>
        public long Start { get; set; } = 0;
        public bool IsPlayed { get; set; } = false;
    }

}
