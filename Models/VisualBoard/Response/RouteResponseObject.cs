using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    /// <summary>
    /// 快递路由对象
    /// </summary>
    public class RouteResponseObject
    {
        /// <summary>
        /// 分公司
        /// </summary>
        public string OrgName { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WarehouseCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }
        /// <summary>
        /// 货主编号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 货主名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OutboundCode { get; set; }
        /// <summary>
        /// 快递公司
        /// </summary>
        public string kdgs { get; set; }
        /// <summary>
        /// 快递公司（可多选）
        /// </summary>
        public string kdgsStr { get; set; }
        /// <summary>
        /// 快递单号
        /// </summary>
        public string kddh { get; set; }
        /// <summary>
        /// 来源平台
        /// </summary>
        public string SourcePlatform { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 付款时间
        /// </summary>
        public string PayTime { get; set; }
        /// <summary>
        /// 尾款支付时间
        /// </summary>
        public string PaidTime { get; set; }
        /// <summary>
        /// 路由最新更新时间
        /// </summary>
        public string UpdatedTime { get; set; }
        /// <summary>
        /// 最新物流状态
        /// </summary>
        public string wuliuzhuangtai { get; set; }
        /// <summary>
        /// 最新物流状态时间
        /// </summary>
        public DateTime? LastRouteTime { get; set; }
        /// <summary>
        /// 详细物流
        /// </summary>
        public string xiangxiwuliu { get; set; }
        /// <summary>
        /// 出库分类3
        /// </summary>
        public string ShipmentCategory3 { get; set; }
        /// <summary>
        /// 出库分类5
        /// </summary>
        public string ShipmentCategory5 { get; set; }
        /// <summary>
        /// 波次号
        /// </summary>
        public string WaveId { get; set; }
        /// <summary>
        /// 波次类型
        /// </summary>
        public string WaveType { get; set; }
        /// <summary>
        /// 来源平台
        /// </summary>
        public string SourceSys { get; set; }
        /// <summary>
        /// 处理类型
        /// </summary>
        public string ProcessType { get; set; }
        /// <summary>
        /// 订单创建时间/转单时间
        /// </summary>
        public string OrderCreateTime { get; set; }
        /// <summary>
        /// 订单最后更新时间
        /// </summary>
        public string OrderLastUpdateTime { get; set; }
        /// <summary>
        /// 快递发出时间（即揽收时间）
        /// </summary>
        public string fachushijian { get; set; }
        /// <summary>
        /// 主状态
        /// </summary>
        public string leadingSts { get; set; }
        /// <summary>
        /// 尾状态
        /// </summary>
        public string trailingSts { get; set; }
        /// <summary>
        /// 件数
        /// </summary>
        public int TotalQty { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int TotalLines { get; set; }
        /// <summary>
        /// 订单更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
        /// <summary>
        /// 波次创建时间
        /// </summary>
        public string WaveCreateTime { get; set; }
        /// <summary>
        /// 拣货完成时间
        /// </summary>
        public string PickingCompleteTime { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string CheckCompleteTime { get; set; }
        /// <summary>
        /// 称重完成时间
        /// </summary>
        public string WeighCompleteTime { get; set; }
        /// <summary>
        /// 发运时间
        /// </summary>
        public string ActualShipTime { get; set; }
        /// <summary>
        /// 快递最新更新时间
        /// </summary>
        public string zuixinshijian { get; set; }
        /// <summary>
        /// 揽收时间
        /// </summary>
        public string lanshoushijian { get; set; }
        /// <summary>
        /// 签收时间
        /// </summary>
        public string qianshoushijian { get; set; }

    }
    ///// <summary>
    ///// 快递路由查询对象
    ///// </summary>
    //public class SelectRouteObject : RouteObject
    //{
    //    /// <summary>
    //    /// 仓库编号（可多选）
    //    /// </summary>
    //    public string WarehouseCodeStr { get; set; }
    //    /// <summary>
    //    /// 货主编号（可多选）
    //    /// </summary>
    //    public string CompanyCodeStr { get; set; }
    //    /// <summary>
    //    /// 来源平台（可多选）
    //    /// </summary>
    //    public string SourcePlatformStr { get; set; }
    //    /// <summary>
    //    /// 店铺名称（可多选）
    //    /// </summary>
    //    public string StoreNameStr { get; set; }
    //    /// <summary>
    //    /// 付款时间开始日期
    //    /// </summary>
    //    public DateTime? PayTimeStart { get; set; }
    //    /// <summary>
    //    /// 付款时间截止日期
    //    /// </summary>
    //    public DateTime? PayTimeEnd { get; set; }

    //    /// <summary>
    //    /// 尾款支付开始时间
    //    /// </summary>
    //    public DateTime? PaidTimeStart { get; set; }
    //    /// <summary>
    //    /// 尾款支付截止时间
    //    /// </summary>
    //    public DateTime? PaidTimeEnd { get; set; }
    //    /// <summary>
    //    /// 订单创建时间/转单时间开始日期
    //    /// </summary>
    //    public DateTime? OrderCreateTimeStart { get; set; }
    //    /// <summary>
    //    /// 订单创建时间/转单时间截止日期
    //    /// </summary>
    //    public DateTime? OrderCreateTimeEnd { get; set; }

    //    /// <summary>
    //    /// 订单创建时间/转单时间开始日期
    //    /// </summary>
    //    public DateTime? OrderLastUpdateTimeStart { get; set; }
    //    /// <summary>
    //    /// 订单创建时间/转单时间截止日期
    //    /// </summary>
    //    public DateTime? OrderLastUpdateTimeEnd { get; set; }
    //    /// <summary>
    //    ///  路由最新时间查询开始日期
    //    /// </summary>
    //    public DateTime? UpdatedTimeStart { get; set; }
    //    /// <summary>
    //    /// 路由最新时间查询截止日期
    //    /// </summary>
    //    public DateTime? UpdatedTimeEnd { get; set; }
    //    /// <summary>
    //    /// 分页每页记录数
    //    /// </summary>
    //    public int PageSize { get; set; }
    //    /// <summary>
    //    /// 分页页码
    //    /// </summary>
    //    public int PageIndex { get; set; }
    //    /// <summary>
    //    /// 排序字段
    //    /// </summary>
    //    public string OrderBy { get; set; }
    //    /// <summary>
    //    /// 快递单号（可多选）
    //    /// </summary>
    //    public string kddhStr { get; set; }
    //}
}
