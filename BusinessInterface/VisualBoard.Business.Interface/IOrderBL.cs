using System;
using System.Collections.Generic;
using System.Text;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Interface
{
    public interface IOrderBL
    {

        /// <summary>
        /// 判断订单完成状态
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public ResponseObject SelectOder(string WavePick, int? TypeID);

        /// <summary>
        /// 订单信息录入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject InsterOrder(OrderObject obj);

        /// <summary>
        /// 拣货、复核、打包、MPS分拣、二次分拣、发运、上架、属性变更
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject PickingOrder(OrderObject obj);


        ///// <summary>
        ///// 根据波次号查询订单信息
        ///// </summary>
        ///// <param name="WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject SelectOrderInfo(string WavePick);

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject SelectAllOrderInfo(string UserID, string WavePick,int CustomerID);

        ///// <summary>
        ///// 模糊查询记录信息
        ///// </summary>
        ///// <param name="UserID"></param>
        ///// <param name="WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject SelectOrderInfo(string UserID,string WavePick);

        ///// <summary>
        ///// 发运
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject ForwardOrder(acc_WavePick _WavePick);


        ///// <summary>
        ///// 上架
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject Putaway(acc_WavePick _WavePick);


        /// <summary>
        /// 抽检查询订单信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject SelectCheckOrder(string WavePick);

        /// <summary>
        /// 抽检
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        public ResponseObject CheckOrder(acc_WavePickDetails _WavePickDetails);


        /// <summary>
        /// 封箱
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        public ResponseObject Sealing(acc_WavePickDetails _WavePickDetails);


        /// <summary>
        /// 属性变更查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject Attribute(string WavePick);

        /// <summary>
        /// 订单类型
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectOrderType();

        /// <summary>
        /// 强制完结查询信息
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ResponseObject EndOrder(EndObject end);


        /// <summary>
        /// 强制完结订单
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject QEndOder(OrderObject obj);




        /// <summary>
        /// 看板下拉列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SeeOrder();

        /// <summary>
        /// 看板饼图
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject PieMap(IndexQueryObject obj);


        ///// <summary>
        ///// 看板柱状图
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public ResponseObject ColumnarMap(IndexQueryObject obj);


        /// <summary>
        /// 看板柱状图
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject NumberCount1(IndexQueryObject indexQuery);


        public ResponseObject ManyJH(ManyJHObject pairs);
    }
}

