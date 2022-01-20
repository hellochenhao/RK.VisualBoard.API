using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Interface
{
    public interface IWarehouseBL
    {


        /// <summary>
        /// PC看板首页
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject WarehouseIndex(IndexQueryObject indexQuery);


        /// <summary>
        /// 大促看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject SaleOrder(IndexQueryObject indexQuery);

        /// <summary>
        /// 分公司看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject CompanyOrder(IndexQueryObject indexQuery);

        /// <summary>
        /// 现场看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject SceneOrder(IndexQueryObject indexQuery);

        /// <summary>
        /// 快递路由报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObjectV2 ExpressRoute(SelectRouteObject indexQuery);

        /// <summary>
        /// 快递路由报表-带有详细路由信息
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObjectV2 ExpressRouteDetails(SelectRouteObject indexQuery);

        /// <summary>
        /// 快递路由报表-各个大类分类状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject ExpressRouteBigClassCount(SelectRouteObject indexQuery);

        /// <summary>
        /// 获取欧莱雅快递公司列表
        /// </summary>
        /// <returns></returns>
        ResponseObject GetExpressCompany();

        /// <summary>
        /// 获取欧莱雅来源平台列表
        /// </summary>
        /// <returns></returns>
        ResponseObject GetSourcePlatform();

        /// <summary>
        /// 获取欧莱雅货主列表
        /// </summary>
        /// <returns></returns>
        ResponseObject GetHuoZhu();

        /// <summary>
        /// 获取欧莱雅店铺名称列表
        /// </summary>
        /// <returns></returns>
        ResponseObject GetStoreName();

        /// <summary>
        /// 大促看板V2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        ResponseObject SaleOrderV2(IndexQueryObject indexQuery);

        /// <summary>
        /// 订单报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder(SelectRouteObject indexQuery);

        /// <summary>
        /// 订单报表-各个状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject RptOrdeStatusCount(SelectRouteObject indexQuery);

        /// <summary>
        /// 订单报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public void RptOrderExecl(object indexQuery);
        /// <summary>
        /// 订单报表导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string RptOrderExecl2(SelectRouteObject indexQuery);




        /// <summary>
        /// 快递路由报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public void ExpressRouteExecl(object indexQuery);

        /// <summary>
        /// 快递路由报表导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string ExpressRouteExecl2(SelectRouteObject indexQuery);

        /// <summary>
        /// 下载列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetDownloads(FileUploadInfoSearchObject fileUploadInfoSearch);
    }
}
