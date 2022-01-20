using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models
{
    public class MoQiuLiObject
    {
        public class SubscribeCallBackResult
        {
            public String code { get; set; }
            public String message { get; set; }
            public MerchantsTraceDTO data { get; set; }
            public String traceId { get; set; }
        }
        /// <summary>
        /// 运单轨迹订阅请求参数
        /// </summary>
        public class MerchantsTraceDTO
        {
            /**
             * 运单号
             */
            public String wayBillNo { get; set; }

            /**
             * 快递公司编号
             */
            public String expressCompanyCode { get; set; }
            /**
             * 快递公司名称
             */
            public String expressCompanyName { get; set; }

            /**
             * 轨迹明细集合
             */
            public List<TraceOpNodeDTO> traceOpNodes { get; set; }
        }
        /// <summary>
        /// 轨迹节点
        /// </summary>
        public class TraceOpNodeDTO
        {
            /**
             * 轨迹节点时间
             */
            public DateTime? opTime { get; set; }
            /**
             * 轨迹节点操作类型
             */
            public String opType { get; set; }
            /**
             * 轨迹节点描述
             */
            public String opMemo { get; set; }
        }

        public class SubscribeCallBack2Service
        {
            public string code { get; set; }
            public string message { get; set; }
        }
    }
}
