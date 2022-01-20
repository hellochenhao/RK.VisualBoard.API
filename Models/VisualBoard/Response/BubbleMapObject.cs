using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    /// <summary>
    /// 气泡地图
    /// </summary>
    public class BubbleMapObject
    {
        /// <summary>
        /// 标题中文
        /// </summary>
        public string target_cn { get; set; }

        /// <summary>
        /// 标题英文
        /// </summary>
        public string target_en { get; set; }

        public BodyBubbleMapObject[] Body { get; set; }

    }

    public class BodyBubbleMapObject
    {
        /// <summary>
        /// 目的地市
        /// </summary>
        public string ReceiveCity { get; set; }
        /// <summary>
        /// 拼音
        /// </summary>
        public string pinyin { get; set; }

        public string Lng { get; set; }

        public string Lat { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        public int? TotalWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public string TotalVol { get; set; }






    }
}
