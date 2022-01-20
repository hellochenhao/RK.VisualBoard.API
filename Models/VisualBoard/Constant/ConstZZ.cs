using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Constant
{
    public struct ConstZZ
    {
        /// <summary>
        /// 猪猪云接口地址
        /// </summary>
        public const string ApiUrl = "http://yun.zhuzhufanli.com/mini";
        /// <summary>
        /// 猪猪云接口地址
        /// </summary>
        public const string CreateUrl = ApiUrl + "/create/";
        /// <summary>
        /// 猪猪云接口地址
        /// </summary>
        public const string SelectUrl = ApiUrl + "/select/";
        /// <summary>
        /// appid
        /// </summary>
        public const string appid = "173306";
        /// <summary>
        /// outerid
        /// </summary>
        public const string outerid = "1A2D2A4B3115419D";
        /// <summary>
        /// 任务有效期（单位=秒。最大4hours）
        /// </summary>
        public const int TaskExpSpan = 4 * 60 * 60;
        /// <summary>
        /// 任务查询提前量。10分钟
        /// </summary>
        public const int TaskQueryLeadSpan = 2 * 60;
        /// <summary>
        /// 创建一个任务最大包括快递单数（1W）
        /// </summary>
        public const int PrePostBillCount = 10;
        /// <summary>
        /// 云上所有任务累计允许的最大快递单数（5W）
        /// </summary>
        public const int ZZPoolMaxBillCount = PrePostBillCount * 5;
        /// <summary>
        /// 批量查询结果每页记录条数
        /// </summary>
        public const int RoutePageSize = 100;
        /// <summary>
        /// 累计路由结果记录写入数据库的最大记录条数
        /// </summary>
        public const int BatchInsertRouteTaskRowsCount = 100;


    }
}
