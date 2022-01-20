using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TablePackObject
    {
        /// <summary>
        ///  中文名
        /// </summary>
        public string target_cn { get; set; }

        /// <summary>
        /// 已打包
        /// </summary>
        public int? IspackNumber { get; set; }


        /// <summary>
        /// 未打包
        /// </summary>
        public int? NopackNumber { get; set; }


        /// <summary>
        /// 总人数
        /// </summary>
        public int? PeopleNumber { get; set; }

        /// <summary>
        /// 人均单数
        /// </summary>
        public int? PeopleAvgNumber { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public TablePackDataObject[] table  { get; set; }
    
    }

    public class TablePackDataObject
    {
        /// <summary>
        /// 台号名称
        /// </summary>
        public string TableNumber { get; set; }

        /// <summary>
        /// 打包数量
        /// </summary>
        public int? Order { get; set; }
        

    }
}
