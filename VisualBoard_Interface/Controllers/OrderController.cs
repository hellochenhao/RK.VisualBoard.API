using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
    /// 订单信息控制器
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {

        private readonly IOrderBL orderBL;

        public OrderController(IOrderBL orderBL)
        {
            this.orderBL = orderBL;
        }

        /// <summary>
        /// 判断订单完成状态
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SelectOrder(string WavePick, int? TypeID)
        {
            return orderBL.SelectOder(WavePick, TypeID);
        }

        /// <summary>
        /// 订单信息录入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject InsterCustomer(OrderObject obj)
        {
            return orderBL.InsterOrder(obj);
        }


        ///// <summary>
        ///// 根据波次号查询订单信息
        ///// </summary>
        ///// <param name="WavePick"></param>
        ///// <returns></returns>

        //[HttpGet]
        //public ResponseObject SelectOrderInfo(string WavePick)
        //{
        //    return orderBL.SelectOrderInfo(WavePick);
        //}






        /// <summary>
        /// 模糊查询记录信息
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ResponseObject SelectOrderInfo(string UserID, string WavePick)
        //{
        //    return orderBL.SelectOrderInfo(UserID, WavePick);
        //}

        ///// <summary>
        ///// 发运
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>

        //[HttpPost]
        //public ResponseObject ForwardOrder(acc_WavePick _WavePick)
        //{
        //    return orderBL.ForwardOrder(_WavePick);
        //}


        ///// <summary>
        ///// 上架
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>

        //[HttpPost]
        //public ResponseObject Putaway(acc_WavePick _WavePick)
        //{
        //    return orderBL.ForwardOrder(_WavePick);
        //}



        /// <summary>
        /// 拣货、复核、打包、MPS分拣、二次分拣、发运、上架、属性变更
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject PickingOrder(OrderObject obj)
        {
            return orderBL.PickingOrder(obj);
        }


        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SelectAllOrderInfo(string UserID, string WavePick, int CustomerID)
        {
            return orderBL.SelectAllOrderInfo(UserID, WavePick, CustomerID);
        }

        ///// <summary>
        ///// 模糊查询记录信息
        ///// </summary>
        ///// <param name="UserID"></param>
        ///// <param name="WavePick"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ResponseObject SelectOrderInfo(string UserID, string WavePick)
        //{
        //    return orderBL.SelectOrderInfo(UserID, WavePick);
        //}

        ///// <summary>
        ///// 发运
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>

        //[HttpPost]
        //public ResponseObject ForwardOrder(acc_WavePick _WavePick)
        //{
        //    return orderBL.ForwardOrder(_WavePick);
        //}


        ///// <summary>
        ///// 上架
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>

        //[HttpPost]
        //public ResponseObject Putaway(acc_WavePick _WavePick)
        //{
        //    return orderBL.ForwardOrder(_WavePick);
        //}

        /// <summary>
        /// 抽检查询订单信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SelectCheckOrder(string WavePick)
        {
            return orderBL.SelectCheckOrder(WavePick);
        }


        /// <summary>
        /// 抽检
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject CheckOrder(acc_WavePickDetails _WavePickDetails)
        {
            return orderBL.CheckOrder(_WavePickDetails);
        }


        /// <summary>
        /// 封箱
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject Sealing(acc_WavePickDetails _WavePickDetails)
        {
            return orderBL.Sealing(_WavePickDetails);
        }

        /// <summary>
        /// 属性变更查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject Attribute(string WavePick)
        {
            return orderBL.Attribute(WavePick);
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SelectOrderType()
        {
            return orderBL.SelectOrderType();
        }


        /// <summary>
        /// 强制完结查询信息
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject EndOrder(EndObject end)
        {
            return orderBL.EndOrder(end);
        }

        /// <summary>
        /// 强制完结订单
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject QEndOder(OrderObject obj)
        {
            return orderBL.QEndOder(obj);
        }


        /// <summary>
        /// 看板下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseObject SeeOrder()
        {
            return orderBL.SeeOrder();
        }

        /// <summary>
        /// 看板饼图
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject PieMap(IndexQueryObject obj)
        {
            return orderBL.PieMap(obj);
        }


        /// <summary>
        /// 看板柱状图
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject NumberCount1(IndexQueryObject indexQuery)
        {
            return orderBL.NumberCount1(indexQuery);
        }
        

        [HttpPost]
        public ResponseObject ManyJH(ManyJHObject pairs)
        {
            return orderBL.ManyJH(pairs);
        }
    }
}
