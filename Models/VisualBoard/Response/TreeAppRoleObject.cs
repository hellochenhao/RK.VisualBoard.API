using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TreeAppRoleObject
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string menu_title { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string menu_icon { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string menu_path { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int? father_id { get; set; }

        /// <summary>
        /// 是否父节点
        /// </summary>
        public bool? issubs { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int? OrderType { get; set; }


        public List<TreeAppRoleObject> children { get; set; }
    }
}
