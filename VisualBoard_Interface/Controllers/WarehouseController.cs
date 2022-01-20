 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// 仓储可视化PC后端BI数据展示
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseBL warehouseBL;

        public WarehouseController(IWarehouseBL warehouseBL)
        {
            this.warehouseBL = warehouseBL;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetWarehouseIndex(IndexQueryObject IndexQuery)
        {
            return warehouseBL.WarehouseIndex(IndexQuery);
        }

        /// <summary>
        /// 大促看板
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetSaleOrder(IndexQueryObject IndexQuery)
        {
            return warehouseBL.SaleOrder(IndexQuery);
        }

        /// <summary>
        /// 分公司看板
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetCompanyOrder(IndexQueryObject IndexQuery)
        {
            return warehouseBL.CompanyOrder(IndexQuery);
        }


        /// <summary>
        /// 现场看板
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetSceneOrder(IndexQueryObject IndexQuery)
        {
            return warehouseBL.SceneOrder(IndexQuery);
        }

        /// <summary>
        /// 快递路由报表
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObjectV2 GetExpressRoute(SelectRouteObject IndexQuery)
        {
            return warehouseBL.ExpressRoute(IndexQuery);
        }

        /// <summary>
        /// 快递路由报表-带有详细路由信息
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObjectV2 RptExpressRouteDetails(SelectRouteObject IndexQuery)
        {
            return warehouseBL.ExpressRouteDetails(IndexQuery);
        }

        /// <summary>
        /// 快递路由报表-各个大类分类状态下的订单数量
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject RptExpressRouteBigClassCount(SelectRouteObject IndexQuery)
        {
            return warehouseBL.ExpressRouteBigClassCount(IndexQuery);
        }


        /// <summary>
        /// 获取快递公司列表
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetExpressCompany()
        {
            return warehouseBL.GetExpressCompany();
        }

        /// <summary>
        /// 获取欧莱雅来源平台列表
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetSourcePlatform()
        {
            return warehouseBL.GetSourcePlatform();
        }

        /// <summary>
        /// 获取欧莱雅货主列表
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetHuoZhu()
        {
            return warehouseBL.GetHuoZhu();
        }

        /// <summary>
        /// 获取欧莱雅店铺名称列表
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetStoreName()
        {
            return warehouseBL.GetStoreName();
        }

        /// <summary>
        /// 大促看板V2
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetSaleOrderV2(IndexQueryObject IndexQuery)
        {
            return warehouseBL.SaleOrderV2(IndexQuery);
        }

        /// <summary>
        /// 订单报表
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject RptOrder(SelectRouteObject IndexQuery)
        {
            return warehouseBL.RptOrder(IndexQuery);
        }

        /// <summary>
        /// 订单报表-各个状态下的订单数量
        /// </summary>
        /// <param name="IndexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject RptOrdeStatusCount(SelectRouteObject IndexQuery)
        {
            return warehouseBL.RptOrdeStatusCount(IndexQuery);
        }

        /// <summary>
        /// 订单报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        /*[HttpPost]
        public string RptOrderExecl(SelectRouteObject indexQuery)
        {
            //return warehouseBL.RptOrderExecl(indexQuery);
        }*/

        /// <summary>
        /// 订单报表导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public string RptOrderExecl(SelectRouteObject indexQuery)
        {
            return warehouseBL.RptOrderExecl2(indexQuery);
        }

 /*       /// <summary>
        /// 快递路由报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <param name="path"></param>
        /// <param name="DCname"></param>
        [HttpPost]
        public void ExpressRouteExecl(SelectRouteObject indexQuery, string path, string DCname)
        {
            ExpressRouteExecl(indexQuery,System.Web.HttpUtility.UrlDecode(path), System.Web.HttpUtility.UrlDecode(DCname));
        }*/

        /// <summary>
        /// 快递路由报表导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public string ExpressRouteExecl(SelectRouteObject indexQuery)
        {
            return warehouseBL.ExpressRouteExecl2(indexQuery);
        }

        /// <summary>
        /// 下载列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject GetDownloads(FileUploadInfoSearchObject fileUploadInfoSearch)
        {
            return warehouseBL.GetDownloads(fileUploadInfoSearch);
        }
    }
}
