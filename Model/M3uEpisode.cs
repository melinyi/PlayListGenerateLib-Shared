using System;
using System.Collections.Generic;
using System.Text;

namespace PlayListLib.Model
{
    public class M3uEpisode
    {
        /// <summary>
        /// 标题 title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 地址 path.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Id "tvg-id"
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Logo Url "tvg-logo"
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 名称 "tvg-name"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 小Logo Url "tvg-logo-small"
        /// </summary>
        public string SmallLogo { get; set; }

        /// <summary>
        /// 组 "group-title"
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// group parent code. "parent-code"
        /// </summary>
        public string Code { get; set; }
        public M3uEpisode Clone() => (M3uEpisode)this.MemberwiseClone();
    }
}
