using Dapper;
using Rokin.Dapper;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Service
{
    public class OrderBL : IOrderBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IDbConnection connection;

        public OrderBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection)
        {
            WMS_Visualboard = _WMS_Visualboard;
            connection = _connection;
        }


        /// <summary>
        /// 判断订单状态
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="TypeID"></param>
        /// <returns></returns>
        public ResponseObject SelectOder(string WavePick, int? TypeID)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            acc_WavePick resultobj = null;
            try
            {
                if (TypeID == null)
                {
                    keyValues.Add("SelectOrderInfo", SelectOrderInfo(WavePick));
                }
                else
                {
                    if (TypeID == 2)
                    {

                        resultobj = JSelect(WavePick, result);
                    }
                    else if (TypeID == 3)
                    {
                        resultobj = FSelect(WavePick, result);
                    }
                    else if (TypeID == 4)
                    {
                        resultobj = DSelect(WavePick, result);
                    }
                    else if (TypeID == 5)
                    {
                        resultobj = YSelect(WavePick, result);
                    }
                    else if (TypeID == 6)
                    {
                        resultobj = MPSSelect(WavePick, result);
                    }
                    else if (TypeID == 7)
                    {
                        resultobj = ESelect(WavePick, result);
                    }
                }

                //result.result = resultobj;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 根据波次号查询订单信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject SelectOrderInfo(string WavePick)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                string Sql = $"select * from acc_WavePick where WavePick ='{WavePick}'";

                var obj = connection.Query<acc_WavePick>(Sql).FirstOrDefault();

                result.result = obj;
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;

        }


        /// <summary>
        /// 订单信息录入
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject InsterOrder(OrderObject obj)
        {
            ResponseObject result = new ResponseObject();
            try
            {

                acc_WavePickDetails _WavePickDetails = new acc_WavePickDetails();
                acc_WavePick _WavePick = new acc_WavePick();

                if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == obj.WavePick).Count() > 0)
                {
                    result.code = 1;
                    result.message = "该波次已录入，请录入其他波次";
                }
                else
                {

                    _WavePickDetails.UserID = obj.UserID;

                    _WavePick.OperateTime = DateTime.Now;
                    _WavePick.OrderStateNow = 1;


                    _WavePick.WavePick = obj.WavePick;
                    _WavePick.Order = obj.Order;
                    _WavePick.PCS = obj.PCS;
                    _WavePick.SKU = obj.SKU;
                    _WavePick.OrderType = obj.OrderType;
                    _WavePick.CustomerID = obj.CustomerID;

                    _WavePickDetails.WavePick = obj.WavePick;
                    _WavePickDetails.Order = obj.Order;
                    _WavePickDetails.PCS = obj.PCS;
                    _WavePickDetails.SKU = obj.SKU;
                    _WavePickDetails.CreateTime = DateTime.Now;
                    _WavePickDetails.OrderState = 100;

                    WMS_Visualboard.Add(_WavePick);
                    WMS_Visualboard.Add(_WavePickDetails);
                    WMS_Visualboard.SaveChanges();
                }

                var a = _WavePick;

                result.result = a;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// 拣货、复核、打包、MPS分拣、二次分拣、发运、上架、属性变更
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject PickingOrder(OrderObject obj)
        {
            ResponseObject result = new ResponseObject();

            acc_WavePickDetails _WavePickDetails = new acc_WavePickDetails();
            acc_WavePick _WavePick = new acc_WavePick();

            #region MyRegion


            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{obj.WavePick}' ";

            var query = connection.Query<EndObject>(Sql).FirstOrDefault();
            try
            {

                if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == obj.WavePick).Count() == 0)
                {
                    result.code = 1;

                    result.message = "波次号不存在";
                }
                else
                {
                    if (obj.OrderType == 1)
                    {
                        if (obj.OrderStateNow == 200)
                        {
                            if (query.OrderStateNow >= 2)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 2;
                            }

                        }
                        else if (obj.OrderStateNow == 300)
                        {
                            if (query.OrderStateNow >= 3)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 3;
                            }

                        }
                        else if (obj.OrderStateNow == 600)
                        {
                            if (query.OrderStateNow >= 4)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 4;
                            }

                        }
                        else if (obj.OrderStateNow == 700)
                        {
                            if (query.OrderStateNow >= 5)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 5;
                            }

                        }
                        else if (obj.OrderStateNow == 800)
                        {
                            if (query.OrderStateNow >= 6)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 6;
                            }
                        }
                        else if (obj.OrderStateNow == 900)
                        {
                            if (query.OrderStateNow >= 7)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 7;
                            }
                        }
                        else if (obj.OrderStateNow == 1000)
                        {
                            if (query.OrderStateNow == 8)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 8;
                            }
                        }
                    }
                    else if (obj.OrderType == 2)
                    {
                        if (obj.OrderStateNow == 200)
                        {
                            if (query.OrderStateNow >= 2)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 2;
                            }
                        }
                        else if (obj.OrderStateNow == 300)
                        {
                            if (query.OrderStateNow >= 9)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 9;
                            }
                        }
                        else if (obj.OrderStateNow == 301)
                        {
                            if (query.OrderStateNow >= 10)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 10;
                            }
                        }
                        else if (obj.OrderStateNow == 400)
                        {
                            if (query.OrderStateNow == 11)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 11;
                            }
                        }
                    }
                    else if (obj.OrderType == 3)
                    {
                        if (obj.OrderStateNow == 200)
                        {
                            if (query.OrderStateNow >= 2)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 2;
                            }
                        }
                        else if (obj.OrderStateNow == 300)
                        {
                            if (query.OrderStateNow >= 12)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 12;
                            }
                        }
                        else if (obj.OrderStateNow == 401)
                        {
                            if (query.OrderStateNow >= 13)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 13;
                            }
                        }
                        else if (obj.OrderStateNow == 500)
                        {
                            if (query.OrderStateNow == 14)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 14;
                            }
                        }
                    }
                    else if (obj.OrderType == 4)
                    {
                        if (obj.OrderStateNow == 200)
                        {
                            if (query.OrderStateNow >= 2)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 2;
                            }
                        }
                        else if (obj.OrderStateNow == 300)
                        {
                            if (query.OrderStateNow == 17)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 17;
                            }
                        }
                    }
                    else if (obj.OrderType == 5)
                    {
                        if (obj.OrderStateNow == 200)
                        {
                            if (query.OrderStateNow >= 2)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 2;
                            }
                        }
                        else if (obj.OrderStateNow == 300)
                        {
                            if (query.OrderStateNow >= 3)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 3;
                            }
                        }
                        else if (obj.OrderStateNow == 600)
                        {
                            if (query.OrderStateNow >= 4)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 4;
                            }
                        }
                        else if (obj.OrderStateNow == 700)
                        {
                            if (query.OrderStateNow >= 5)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 5;
                            }
                        }
                        else if (obj.OrderStateNow == 800)
                        {
                            if (query.OrderStateNow >= 6)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 6;
                            }
                        }
                        else if (obj.OrderStateNow == 900)
                        {
                            if (query.OrderStateNow >= 15)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 15;
                            }
                        }
                        else if (obj.OrderStateNow == 950)
                        {
                            if (query.OrderStateNow == 16)
                            {
                                result.code = 1;
                                result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                            }
                            else
                            {
                                _WavePick.OrderStateNow = 16;
                            }
                        }
                    }
                    #endregion
                    _WavePick.ID = obj.ID;
                    _WavePick.OperateTime = DateTime.Now;
                    _WavePick.WavePick = obj.WavePick;
                    _WavePick.Order = obj.Order;
                    _WavePick.PCS = obj.PCS;
                    _WavePick.SKU = obj.SKU;
                    _WavePick.OrderType = obj.OrderType;
                    _WavePick.CustomerID = obj.CustomerID;


                    _WavePickDetails.WavePick = obj.WavePick;
                    _WavePickDetails.Order = obj.Order;
                    _WavePickDetails.PCS = obj.PCS;
                    _WavePickDetails.SKU = obj.SKU;
                    _WavePickDetails.State = obj.State;
                    _WavePickDetails.OrderState = obj.OrderStateNow;
                    _WavePickDetails.CreateTime = DateTime.Now;
                    _WavePickDetails.PeopleNumber = obj.PeopleNumber;
                    _WavePickDetails.TableNumber = obj.TableNumber;
                    _WavePickDetails.BoxNumber = obj.BoxNumber;
                    _WavePickDetails.UserID = obj.UserID;
                    _WavePickDetails.Remark = obj.Remark;

                    obj.OrderStateNow = _WavePick.OrderStateNow;
                    WMS_Visualboard.Add(_WavePickDetails);
                    WMS_Visualboard.UpdateNotNull(_WavePick);
                    WMS_Visualboard.SaveChanges();
                    result.result = obj;
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject SelectAllOrderInfo(string UserID, string WavePick, int CustomerID)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string sql = $@"SELECT
  T.WavePick, T.OrderType, OrderState, MIN( `Order` ) AS 'Order', min(PCS) AS 'PCS', min(SKU) AS 'SKU',
  max(STARTTIME) AS 'STARTTIME', max(ENDTIME) AS 'ENDTIME', TIMESTAMPDIFF(MINUTE, max(STARTTIME), max(ENDTIME)) AS 'Leadtime'  ,T.UserID,T.UserName
FROM
  (SELECT a.WavePick, c.ItemName AS OrderType,
  CASE b.OrderState  WHEN 100 THEN '录入'  WHEN 200 THEN '拣货'  WHEN 300 THEN '拣货'  WHEN 600 THEN
      '复核'  WHEN 700 THEN '复核'  WHEN 800 THEN '打包'  WHEN 900 THEN '打包'  WHEN 1000 THEN
      '发运'  WHEN 301 THEN 'MPS分拣'  WHEN 400 THEN 'MPS分拣'  WHEN 401 THEN '二次分拣'
      WHEN 500 THEN '二次分拣'  WHEN 950 THEN   '上架'    WHEN 2000 THEN   '抽检通过'
      WHEN 2001 THEN '抽检不通过'  WHEN 3000 THEN '封箱'  WHEN 4000 THEN '强制完结'
      WHEN 4001 THEN '属性变更'  END AS 'OrderState',
  CASE WHEN b.OrderState IN(100, 200, 600, 800, 1000, 301, 401,2000,2001,950,4000) THEN b.CREATEtime END AS 'STARTTIME',
  CASE WHEN b.OrderState IN(300, 700, 900, 400, 500) THEN   b.CREATEtime END AS 'ENDTIME',
    b.ORDER, b.PCS, b.SKU, b.UserID, D.UserName, a.CustomerID
  FROM
    acc_WavePick a
    LEFT JOIN acc_WavePickDetails b ON a.WavePick = b.WavePick
    LEFT JOIN pub_Idreplace c ON a.ordertype = c.`VALUE` AND c.typename = '订单类型'
    LEFT JOIN bsc_User D ON b.UserID = D.UserID
  WHERE a.state = 0 AND b.state = 0";
                if (WavePick == null)
                {
                    sql += $@" AND b.UserID = '{UserID}'
    and a.CustomerID = {CustomerID} 
  AND b.CreateTime >= DATE_FORMAT(CURDATE(), '%Y-%m-%d %H:%i:%s') and b.CreateTime < DATE_SUB(curdate(), INTERVAL - 1 DAY)";

                }
                else
                {
                    sql += $@" and  a.WavePick='{WavePick}'";

                }

                sql += $@"  ) T GROUP BY T.WavePick, T.OrderType, OrderState ,T.UserID,T.UserName ORDER BY STARTTIME DESC ";

                var query = connection.Query<SelectOrderInfoObject>(sql).ToArray();
                var fh = query.Where(p => p.OrderState == "复核").FirstOrDefault();
                var DB = query.Where(p => p.OrderState == "打包").FirstOrDefault();
                var FY = query.Where(p => p.OrderState == "发运").FirstOrDefault();
                var SJ = query.Where(p => p.OrderState == "上架").FirstOrDefault();
                if (DB != null)
                {
                    DB.PCS = fh?.PCS ?? 0;
                    DB.SKU = fh?.SKU ?? 0;
                }
                if (FY != null)
                {
                    FY.Order = fh?.Order ?? 0;
                    FY.PCS = fh?.PCS ?? 0;
                    FY.SKU = fh?.SKU ?? 0;
                }
                if (SJ != null)
                {
                    SJ.Order = fh?.Order ?? 0;
                    SJ.PCS = fh?.PCS ?? 0;
                    SJ.SKU = fh?.SKU ?? 0;
                }
                result.result = query;
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        //        /// <summary>
        //        /// 模糊查询记录信息
        //        /// </summary>
        //        /// <param name="UserID"></param>
        //        /// <param name="WavePick"></param>
        //        /// <returns></returns>
        //        public ResponseObject SelectOrderInfo(string UserID, string WavePick)
        //        {
        //            ResponseObject result = new ResponseObject();

        //            try
        //            {
        //                string Sql = "   SELECT  T.WavePick ,T.OrderType,OrderState,MIN(`Order`) as 'Order',min(PCS) as 'PCS' ,min(SKU) as 'SKU' ,max(STARTTIME) as 'STARTTIME',max(ENDTIME) as 'ENDTIME', TIMESTAMPDIFF(MINUTE, max(STARTTIME), max(ENDTIME)) as 'Leadtime'/**/" +
        //   "from(SELECT  a.WavePick, c.ItemName as OrderType, CASE b.OrderState when 100 then '录入' when 200 then '拣货' when 300 then '拣货' when 600 then '复核' when  700 then '复核'" +
        //"when 800 then  '打包' when  900 then  '打包' when  1000 then '发运'  when  301 then  'MPS分拣' when  400 then 'MPS分拣' when 401 then  '二次分拣' when 500 then  '二次分拣' end as 'OrderState'," +
        //"case when b.OrderState in (100, 200, 600, 800, 1000, 301, 401) then b.CREATEtime end as 'STARTTIME',case when b.OrderState  in (300, 700, 900, 400, 500)  then  b.CREATEtime end as 'ENDTIME'," +
        //"b.Order, b.PCS, b.SKU FROM acc_WavePick   a LEFT JOIN acc_WavePickDetails b ON a.WavePick = b.WavePick left join pub_Idreplace c on a.ordertype = c.value and c.typename = '订单类型'" +
        //$"where a.state = 0 and b.state = 0 and a.WavePick like '%{WavePick}%'   and UserID = '{UserID}')T GROUP BY T.WavePick ,T.OrderType  ,OrderState";

        //                result.result = connection.Query<SelectOrderInfoObject>(Sql).Select(p=>new SelectOrderInfoObject { WavePick = p.WavePick, OrderType = p.OrderType, OrderState = p.OrderState, Order = p.Order, PCS = p.PCS, SKU = p.SKU, STARTTIME = p.STARTTIME, ENDTIME = p.ENDTIME, Leadtime = p.Leadtime }).ToArray();
        //            }
        //            catch (Exception ex)
        //            {

        //                result.code = 1;
        //                result.message = ex.Message;
        //            }

        //            return result;
        //        }

        ///// <summary>
        ///// 发运
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject ForwardOrder(acc_WavePick _WavePick)
        //{
        //    ResponseObject result = new ResponseObject();

        //    try
        //    {
        //        acc_WavePickDetails _WavePickDetails = new acc_WavePickDetails();

        //        if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == _WavePick.WavePick).Count() == 0)
        //        {
        //            result.code = 1;
        //            result.message = "波次号不存在";
        //        }
        //        else
        //        {
        //            _WavePick.OrderStateNow = 8;
        //            _WavePickDetails.OrderState = 8;

        //            WMS_Visualboard.UpdateNotNull(_WavePick);
        //            WMS_Visualboard.Add(_WavePickDetails);
        //            WMS_Visualboard.SaveChanges();

        //            result.result = "发运成功";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.code = 1;
        //        result.message = ex.Message;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 上架
        ///// </summary>
        ///// <param name="_WavePick"></param>
        ///// <returns></returns>
        //public ResponseObject Putaway(acc_WavePick _WavePick)
        //{
        //    ResponseObject result = new ResponseObject();

        //    try
        //    {
        //        acc_WavePickDetails _WavePickDetails = new acc_WavePickDetails();

        //        if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == _WavePick.WavePick).Count() == 0)
        //        {
        //            result.code = 1;
        //            result.message = "波次号不存在";
        //        }
        //        else
        //        {
        //            _WavePick.OrderStateNow = 16;

        //            _WavePickDetails.OrderState = 16;

        //            WMS_Visualboard.UpdateNotNull(_WavePick);
        //            WMS_Visualboard.Add(_WavePickDetails);
        //            WMS_Visualboard.SaveChanges();

        //            result.result = "上架成功";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.code = 1;
        //        result.message = ex.Message;
        //    }

        //    return result;
        //}


        /// <summary>
        /// 抽检查询订单信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject SelectCheckOrder(string WavePick)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $"SELECT a.WavePick,a.TableNumber,a.CreateTime FROM `acc_WavePickDetails` a WHERE a.WavePick = '{WavePick}' AND a.OrderState = 900";

                var obj = connection.Query<acc_WavePickDetails>(Sql);

                if (obj.Count() == 0)
                {
                    result.code = 1;
                    result.message = "该波次不能抽检/未完成，请联系文员处理";
                }
                else
                {
                    result.result = obj;
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 抽检
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        public ResponseObject CheckOrder(acc_WavePickDetails _WavePickDetails)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == _WavePickDetails.WavePick).Count() == 0)
                {

                    result.code = 1;
                    result.message = "该波次号不存在";
                }
                else
                {
                    _WavePickDetails.CreateTime = DateTime.Now;
                    WMS_Visualboard.Add(_WavePickDetails);
                    WMS_Visualboard.SaveChanges();
                }

                var obj = _WavePickDetails;
                result.result = obj;

            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 封箱子
        /// </summary>
        /// <param name="_WavePickDetails"></param>
        /// <returns></returns>
        public ResponseObject Sealing(acc_WavePickDetails _WavePickDetails)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                _WavePickDetails.CreateTime = DateTime.Now;
                _WavePickDetails.OrderState = 3000;


                WMS_Visualboard.acc_WavePickDetails.Add(_WavePickDetails);
                WMS_Visualboard.SaveChanges();
                result.result = _WavePickDetails;
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 拣货查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick JSelect(string WavePick, ResponseObject result)
        {

            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }


            if (obj.OrderStateNow != 1)
            {
                result.code = 1;
                result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                return null;
            }

            if (result.code == 0)
            {
                result.result = obj;
            }
            return obj;
        }


        /// <summary>
        /// 复核查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick FSelect(string WavePick, ResponseObject result)
        {
            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }

            if (obj.OrderType == 2 || obj.OrderType == 3 || obj.OrderType == 4)
            {
                result.code = 1;
                result.message = "该波次号不能复核，请确认波次类型";
                return null;
            }

            if ((obj.OrderType == 1 || obj.OrderType == 5) && obj.OrderStateNow != 3)
            {
                result.code = 1;
                result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                return null;
            }

            if (result.code == 0)
            {
                result.result = obj;
            }

            return obj;


        }

        /// <summary>
        /// 打包查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick DSelect(string WavePick, ResponseObject result)
        {
            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }

            if ((obj.OrderType == 1 || obj.OrderType == 5) && obj.OrderStateNow != 5)
            {
                result.code = 1;
                result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                return null;

            }

            if (obj.OrderType == 2 || obj.OrderType == 3 || obj.OrderType == 4)
            {
                result.code = 1;
                result.message = "该波次号不能打包，请确认波次类型";
                return null;
            }

            if (result.code == 0)
            {
                result.result = obj;
            }

            return obj;

        }

        /// <summary>
        /// 发运查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick YSelect(string WavePick, ResponseObject result)
        {
            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }

            if (obj.OrderType == 1)
            {
                if (obj.OrderType == 1 && obj.OrderStateNow != 7)
                {
                    result.code = 1;
                    result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                    return null;

                }
                if (obj.OrderType == 2 || obj.OrderType == 3 || obj.OrderType == 4 || obj.OrderType == 5)
                {
                    result.code = 1;
                    result.message = "该波次号不能发运，请确认波次类型";
                    return null;
                }
                if (result.code == 0)
                {
                    result.result = obj;
                }
            }

            if (obj.OrderType == 5)
            {
                if (obj.OrderType == 1 || obj.OrderType == 2 || obj.OrderType == 3 || obj.OrderType == 4)
                {
                    result.code = 1;
                    result.message = "该波次号不能上架，请确认波次类型";
                    return null;
                }
                if (obj.OrderType == 5 && obj.OrderStateNow != 15)
                {
                    result.code = 1;
                    result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                    return null;
                }
                if (result.code == 0)
                {
                    result.result = obj;
                }
            }

            return obj;
        }


        ///// <summary>
        ///// 上架查询信息
        ///// </summary>
        ///// <param name="WavePick"></param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public acc_WavePick SSelect(string WavePick, ResponseObject result)
        //{

        //    string Sql = $"select * from acc_WavePick where WavePick ='{WavePick}'";

        //    var obj = connection.Query<acc_WavePick>(Sql).FirstOrDefault();

        //    if (obj == null)
        //    {
        //        result.code = 1;
        //        result.message = "该波次号错误";
        //        return null;
        //    }

        //    if (obj.OrderType == 1)
        //    {
        //        result.code = 1;
        //        result.message = "该波次号错误";
        //        return null;

        //    }
        //    else if (obj.OrderType == 2)
        //    {
        //        result.code = 1;
        //        result.message = "该波次号错误";
        //        return null;
        //    }
        //    else if (obj.OrderType == 3)
        //    {
        //        result.code = 1;
        //        result.message = "该波次号错误";
        //        return null;
        //    }
        //    else if (obj.OrderType == 4)
        //    {
        //        result.code = 1;
        //        result.message = "该波次号错误";
        //        return null;
        //    }
        //    else if (obj.OrderType == 5 && obj.OrderStateNow == 16)
        //    {
        //        result.code = 1;
        //        result.message = "该波次已上架，请上架其他波次";
        //        return null;
        //    }
        //    if (result.code == 0)
        //    {
        //        result.result = obj;
        //    }
        //    return obj;

        //}

        /// <summary>
        /// MPS分拣查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick MPSSelect(string WavePick, ResponseObject result)
        {

            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }

            if (obj.OrderType == 1 || obj.OrderType == 3 || obj.OrderType == 4 || obj.OrderType == 5)
            {
                result.code = 1;
                result.message = "该波次号不能MPS分拣，请确认波次类型";
                return null;
            }

            if (obj.OrderType == 2 && obj.OrderStateNow != 9)
            {
                result.code = 1;
                result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                return null;
            }

            if (result.code == 0)
            {
                result.result = obj;
            }
            return obj;
        }

        /// <summary>
        /// 二次分拣查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public acc_WavePick ESelect(string WavePick, ResponseObject result)
        {

            string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{WavePick}' ";

            var obj = connection.Query<EndObject>(Sql).FirstOrDefault();

            if (obj == null)
            {
                result.code = 1;
                result.message = "该波次号未录入";
                return null;
            }

            if (obj.OrderType == 1 || obj.OrderType == 2 || obj.OrderType == 4 || obj.OrderType == 5)
            {
                result.code = 1;
                result.message = "该波次号不能二次分拣，请确认波次类型";
                return null;
            }


            if (obj.OrderType == 3 && obj.OrderStateNow != 12)
            {
                result.code = 1;
                result.message = $"该波次号当前状态为'{obj.AfterStateC}'，请联系文员处理";
                return null;
            }

            if (result.code == 0)
            {
                result.result = obj;
            }
            return obj;

        }


        /// <summary>
        /// 属性变更查询信息
        /// </summary>
        /// <param name="WavePick"></param>
        /// <returns></returns>
        public ResponseObject Attribute(string WavePick)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $"select * from acc_WavePick where WavePick = '{WavePick}'";

                var obj = connection.Query<AttributeObject>(Sql).Select(p => new AttributeObject { ID = p.ID, WavePick = p.WavePick, Order = p.Order, PCS = p.PCS, SKU = p.SKU, OrderStateNow = p.OrderStateNow }).FirstOrDefault();

                if (obj.OrderStateNow == 1)
                {
                    obj.OrderStateNowC = "未拣货";

                    result.result = obj;
                }

                else if (obj.OrderStateNow == 2)
                {
                    obj.OrderStateNowC = "拣货中";

                    result.result = obj;
                }

                else if (obj.OrderStateNow == 3)
                {
                    obj.OrderStateNowC = "未复核";

                    result.result = obj;
                }
                else
                {
                    result.code = 1;
                    result.message = "该波次号状态不支持属性变更";
                }

            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectOrderType()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = "SELECT a.ID,a.ItemName,a.`Value` from pub_Idreplace a where TypeName = '订单类型'";

                result.result = connection.Query<pub_Idreplace>(Sql);
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 强制完结查询信息
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ResponseObject EndOrder(EndObject end)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $@"SELECT
  a.OrderStateNow,
  b.ItemName AS 'OrderStateNowC',
  a.OrderType,
  c.ItemName AS 'OrderTypeC',
  a.*
FROM
  acc_WavePick a
  LEFT JOIN pub_Idreplace b ON a.OrderStateNow = b.`Value` 
  AND b.TypeName = '订单最新状态'
  LEFT JOIN pub_Idreplace c ON a.OrderType = c.`Value` 
  AND c.TypeName = '订单类型'
  where WavePick = '{end.WavePick}'";

                var obj = connection.Query<EndObject>(Sql).Select(p => new EndObject { ID = p.ID, WavePick = p.WavePick, Order = p.Order, PCS = p.PCS, SKU = p.SKU, OperateTime = p.OperateTime, State = p.State, OrderType = p.OrderType, OrderTypeC = p.OrderTypeC, OrderStateNowC = p.OrderStateNowC, OrderStateNow = p.OrderStateNow, CustomerID = p.CustomerID }).FirstOrDefault();

                if (obj == null)
                {
                    result.code = 1;
                    result.message = "该波次号有误";
                    return result;
                }

                if (obj.OrderType == 1)
                {
                    if (obj.OrderStateNow == 2 || obj.OrderStateNow == 4 || obj.OrderStateNow == 6)
                    {
                        obj.AfterState = obj.OrderStateNow + 1;


                        if (obj.AfterState == 3)
                        {
                            obj.AfterStateC = "未复核";
                        }

                        if (obj.AfterState == 5)
                        {
                            obj.AfterStateC = "未打包";
                        }
                        if (obj.AfterState == 7)
                        {
                            obj.AfterStateC = "未发运";
                        }

                        result.result = obj;
                    }
                    else
                    {
                        result.code = 1;
                        result.message = "该订单状态不支持强制完结";
                    }
                }
                if (obj.OrderType == 2)
                {
                    if (obj.OrderStateNow == 2)
                    {
                        obj.AfterState = 9;

                        obj.AfterStateC = "MPS未上架";

                        result.result = obj;
                    }

                    else if (obj.OrderStateNow == 10)
                    {
                        obj.AfterState = 11;

                        obj.AfterStateC = "MPS分拣完成";

                        result.result = obj;
                    }

                    else
                    {
                        result.code = 1;
                        result.message = "该订单状态不支持强制完结";
                    }
                }
                if (obj.OrderType == 3)
                {
                    if (obj.OrderStateNow == 2)
                    {
                        obj.AfterState = 12;

                        obj.AfterStateC = "未二次分拣";

                        result.result = obj;
                    }
                    else if (obj.OrderStateNow == 13)
                    {
                        obj.AfterState = 14;

                        obj.AfterStateC = "二次分拣完成";

                        result.result = obj;
                    }
                    else
                    {
                        result.code = 1;
                        result.message = "该订单状态不支持强制完结";
                    }
                }
                if (obj.OrderType == 4)
                {
                    if (obj.OrderStateNow == 2)
                    {
                        obj.AfterState = 17;

                        obj.AfterStateC = "拣货完成";

                        result.result = obj;
                    }
                    else
                    {
                        result.code = 1;
                        result.message = "该订单状态不支持强制完结";
                    }
                }
                if (obj.OrderType == 5)
                {
                    if (obj.OrderStateNow == 2)
                    {
                        obj.AfterState = 3;

                        obj.AfterStateC = "未复核";

                        result.result = obj;
                    }
                    else if (obj.OrderStateNow == 4)
                    {
                        obj.AfterState = 5;

                        obj.AfterStateC = "未打包";

                        result.result = obj;
                    }
                    else if (obj.OrderStateNow == 6)
                    {
                        obj.AfterState = 15;

                        obj.AfterStateC = "未上架";

                        result.result = obj;
                    }
                    else
                    {
                        result.code = 1;
                        result.message = "该订单状态不支持强制完结";
                    }
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 强制完结订单
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject QEndOder(OrderObject obj)
        {

            ResponseObject result = new ResponseObject();


            acc_WavePick _WavePick = new acc_WavePick();
            acc_WavePickDetails _WavePickDetails = new acc_WavePickDetails();

            try
            {
                if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == obj.WavePick).Count() == 0)
                {
                    result.code = 1;
                    result.message = "波次号不存在";
                    return result;
                }

                _WavePick.ID = obj.ID;
                _WavePick.WavePick = obj.WavePick;
                _WavePick.Order = obj.Order;
                _WavePick.PCS = obj.PCS;
                _WavePick.SKU = obj.SKU;
                _WavePick.OperateTime = DateTime.Now;
                _WavePick.OrderType = obj.OrderType;
                _WavePick.OrderStateNow = obj.OrderStateNow;
                _WavePick.CustomerID = obj.CustomerID;
                _WavePick.State = false;

                _WavePickDetails.WavePick = obj.WavePick;
                _WavePickDetails.Order = obj.Order;
                _WavePickDetails.PCS = obj.PCS;
                _WavePickDetails.SKU = obj.SKU;
                _WavePickDetails.CreateTime = DateTime.Now;
                _WavePickDetails.State = obj.State;
                _WavePickDetails.UserID = obj.UserID;

                _WavePickDetails.OrderState = 4000;

                WMS_Visualboard.Add(_WavePickDetails);
                WMS_Visualboard.UpdateNotNull(_WavePick);
                WMS_Visualboard.SaveChanges();

                result.result = _WavePick;

            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// 看板下拉列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SeeOrder()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = @"select  '全国'as 'ShipperName'
 union all 
SELECT  DISTINCT concat( WarehouseName, ShipperName )  as 'ShipperName' from  bi_Wms  where   DataFrom =0";

                result.result = connection.Query<bi_Wms>(Sql).Select(p => new { ShipperName = p.ShipperName }).ToArray();
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }



        /// <summary>
        /// 看板饼图
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject PieMap(IndexQueryObject obj)
        {
            ResponseObject result = new ResponseObject();
            String Sql = "";
            PieOrderObject data = new PieOrderObject();

            try
            {
                if (obj.CustomerID == "全国")
                {
                    Sql = @$"SELECT 
T.OrderType,ShipperName,T.OperateTime,CASE Shipping WHEN 0 THEN Cancel  ELSE  Shipping END  AS 'Shipping',Cancel,Shipping as 'EndPick'  from 
(SELECT OrderType,concat( OrderType ,'已发运') as 'ShipperName' ,  
  sum( Shipping)  as 'Shipping', /*已经发运*/
0  as 'Cancel',/*未完成*/ 
min(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)obj.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)obj.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom =0
GROUP BY   OrderType 
UNION all

SELECT  OrderType,concat( OrderType ,'未发运') as 'EndType' , 0 as 'Shipping',
SUM(OrderCount)- sum( Shipping)  as  'Cancel',/*未完成*/ 
min(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)obj.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)obj.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom =0
GROUP BY   OrderType )T  GROUP BY  T.OrderType,T.ShipperName,T.Shipping,T.Cancel,T.OperateTime
ORDER BY OrderType";
                }
                else
                {
                    Sql = @$"SELECT 
T.OrderType,ShipperName,T.OperateTime,CASE Shipping WHEN 0 THEN Cancel  ELSE  Shipping END  AS 'Shipping',Cancel,Shipping as 'EndPick'   from 
(SELECT OrderType,concat( OrderType ,'已发运') as 'ShipperName' ,  
  sum( Shipping)  as 'Shipping', /*已经发运*/
0  as 'Cancel',/*未完成*/ 
min(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)obj.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)obj.endTime:yyyy-MM-dd  HH:mm:ss}' 
and concat( WarehouseName, ShipperName )  ='{obj.CustomerID}'
AND  DataFrom = 0
GROUP BY   OrderType 
UNION all

SELECT  OrderType,concat( OrderType ,'未发运') as 'EndType' , 0 as 'Shipping',
SUM(OrderCount)- sum( Shipping)  as  'Cancel',/*未完成*/ 
min(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)obj.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)obj.endTime:yyyy-MM-dd  HH:mm:ss}' 
and concat( WarehouseName, ShipperName )  ='{obj.CustomerID}'
AND  DataFrom =0
GROUP BY   OrderType )T  GROUP BY  T.OrderType,T.ShipperName,T.Shipping,T.Cancel,T.OperateTime
ORDER BY OrderType";
                }
                var query = connection.Query<bi_Wms>(Sql);

                data.OrderData = query.Select(p => new Data { name = p.ShipperName, data = p.Shipping }).ToArray();

                data.WDespatchOrderNum = query.Select(p => p.Cancel).Sum();

                data.DespatchOrderNum = query.Select(p => p.EndPick).Sum();

                data.DataTime = query.Select(p => p.OperateTime).Min();

                result.result = data;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }



        //        /// <summary>
        //        /// 看板柱状图
        //        /// </summary>
        //        /// <param name="obj"></param>
        //        /// <returns></returns>
        //        public ResponseObject ColumnarMap(IndexQueryObject obj)
        //        {
        //            ResponseObject result = new ResponseObject();

        //            ColumnarObject col = new ColumnarObject();

        //            try
        //            {
        //                string s = obj.OrganizationID;

        //                s = s.Substring(0,s.Length - 3);

        //                string Sql = $@"SELECT 
        //	OrderType,
        //	SUM( WaitDistribution ) AS 'WaitDistribution',
        //	SUM( WavePicking ) AS 'WavePicking',
        //	SUM( WaitPick ) AS 'WaitPick',
        //	SUM( EndPick ) AS ' EndPick',
        //	sum( WaitPackage ) AS 'WaitPackage',
        //	sum( EndReview ) AS 'EndReview',
        //	sum( EndPresell ) AS 'EndPresell',
        //	sum( OrderWeight ) AS 'OrderWeight'

        //FROM
        //	bi_Wms 
        //WHERE
        //	createtime >= '{(DateTime)obj.startTime:yyyy-MM-dd  HH:mm:ss}' 
        //	AND createtime < '{(DateTime)obj.endTime:yyyy-MM-dd  HH:mm:ss}' 
        //	AND concat( WarehouseName, ShipperName ) = '{obj.CustomerID}' 
        //	AND OrderType = '{s}' 
        //	AND DataFrom = 0 
        //GROUP BY

        //	OrderType";

        //                var query = connection.Query<bi_Wms>(Sql).Select(p => new { Name = p.OrderType, Date = Convert.ToDecimal(p.) }).ToArray();

        //                col.Name = query.Select(p => p.Name).ToArray();
        //                col.Data = query.Select(p => p.Date).ToArray();

        //                result.result = col;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.code = 1;
        //                result.message = ex.Message;
        //            }

        //            return result;
        //        }


        /// <summary>
        /// 看板柱状图
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject NumberCount1(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();

            MixedLineAndBarObject mixed = new MixedLineAndBarObject();

            try
            {
                string s = indexQuery.OrganizationID;

                s = s.Substring(0, s.Length - 3);

                if (indexQuery.CustomerID == "全国")
                {
                    string Sql = $@"    SELECT   
  SUM(OrderCount) AS 'OrderCount', SUM(WaitDistribution) AS 'WaitDistribution', 
  SUM(WavePicking) AS 'WavePicking',SUM( WaitPick) AS 'WaitPick',SUM( EndPick)as ' EndPick',
  sum( WaitPackage) as 'WaitPackage', sum(EndReview) as 'EndReview',
  sum(EndPresell) as 'EndPresell',sum( OrderWeight) as 'OrderWeight' 
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
and OrderType ='{s}'
AND  DataFrom =0 
  ";
                    var query = connection.Query<bi_Wms>(Sql).FirstOrDefault();

                    string sql1 = @$"SELECT  
SUM(OrderCount) - sum(Shipping) as  'OrderCount'/*未完成*/
FROM bi_Wms
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
AND createtime< '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'
and OrderType ='{s}'
AND DataFrom = 0
";
                    mixed.target_cn = connection.Query<bi_Wms>(sql1).Select(p => p.OrderCount).FirstOrDefault().ToString();

                    mixed.day = new string[] { "待分配", "波次中", "待拣货", "拣货完成", "待包装", "复核完成", "预售完成", "活动单称重" };
                    mixed.Value = new decimal[] { (decimal)query.WaitDistribution, (decimal)query.WavePicking, (decimal)query.WaitPick, (decimal)query.EndPick, (decimal)query.WaitPackage, (decimal)query.EndReview, (decimal)query.EndPresell, (decimal)query.OrderWeight };
                }
                else
                {
                    string Sql = $@"    SELECT   
  SUM(OrderCount) AS 'OrderCount', SUM(WaitDistribution) AS 'WaitDistribution', 
  SUM(WavePicking) AS 'WavePicking',SUM( WaitPick) AS 'WaitPick',SUM( EndPick)as ' EndPick',
  sum( WaitPackage) as 'WaitPackage', sum(EndReview) as 'EndReview',
  sum(EndPresell) as 'EndPresell',sum( OrderWeight) as 'OrderWeight' 
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
and concat( WarehouseName, ShipperName )  ='{indexQuery.CustomerID}'
and OrderType ='{s}'
AND  DataFrom =0 
  ";
                    var query = connection.Query<bi_Wms>(Sql).FirstOrDefault();

                    string sql1 = @$"SELECT  
SUM(OrderCount) - sum(Shipping) as  'OrderCount'/*未完成*/
FROM bi_Wms
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
AND createtime< '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'
and concat(WarehouseName, ShipperName )  = '{indexQuery.CustomerID}'
and OrderType ='{s}'
AND DataFrom = 0
";
                    mixed.target_cn = connection.Query<bi_Wms>(sql1).Select(p => p.OrderCount).FirstOrDefault().ToString();

                    mixed.day = new string[] { "待分配", "波次中", "待拣货", "拣货完成", "待包装", "复核完成", "预售完成", "活动单称重" };
                    mixed.Value = new decimal[] { (decimal)query.WaitDistribution, (decimal)query.WavePicking, (decimal)query.WaitPick, (decimal)query.EndPick, (decimal)query.WaitPackage, (decimal)query.EndReview, (decimal)query.EndPresell, (decimal)query.OrderWeight };
                }

                result.result = mixed;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }


            return result;

        }


        public ResponseObject ManyJH(ManyJHObject obj)
        {
            ResponseObject result = new ResponseObject();

            acc_WavePickDetails acc_Details = new acc_WavePickDetails();

            try
            {
                var WavePicklist = obj.data.Select(p => new acc_WavePick
                {
                    ID = p.ID,
                    OrderStateNow = 2,
                    OperateTime = DateTime.Now,
                    WavePick = p.WavePick,
                    Order = p.Order,
                    PCS = p.PCS,
                    SKU = p.SKU,
                    OrderType = p.OrderType,
                    CustomerID = p.CustomerID
                }).ToList();


                foreach (var items in WavePicklist)
                {
                    #region MyRegion
                    string Sql = $@"	select a.*,b.ItemName as'AfterStateC' from acc_WavePick a 

    left join  pub_Idreplace b on a.OrderStateNow = b.Value and b.TypeName = '订单最新状态'
where WavePick = '{items.WavePick}' ";

                    var query = connection.Query<EndObject>(Sql).FirstOrDefault();


                    if (WMS_Visualboard.acc_WavePick.Where(p => p.WavePick == items.WavePick).Count() == 0)
                    {
                        result.code = 1;

                        result.message = "波次号不存在";
                    }
                    else
                    {
                        if (items.OrderType == 1)
                        {
                            if (obj.OrderStateNow == 200)
                            {
                                if (query.OrderStateNow >= 2)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 2;
                                }

                            }
                            else if (obj.OrderStateNow == 300)
                            {
                                if (query.OrderStateNow >= 3)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 3;
                                }

                            }
                            else if (obj.OrderStateNow == 600)
                            {
                                if (query.OrderStateNow >= 4)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 4;
                                }

                            }
                            else if (obj.OrderStateNow == 700)
                            {
                                if (query.OrderStateNow >= 5)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 5;
                                }

                            }
                            else if (obj.OrderStateNow == 800)
                            {
                                if (query.OrderStateNow >= 6)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 6;
                                }
                            }
                            else if (obj.OrderStateNow == 900)
                            {
                                if (query.OrderStateNow >= 7)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 7;
                                }
                            }
                            else if (obj.OrderStateNow == 1000)
                            {
                                if (query.OrderStateNow == 8)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 8;
                                }
                            }
                        }
                        else if (items.OrderType == 2)
                        {
                            if (obj.OrderStateNow == 200)
                            {
                                if (query.OrderStateNow >= 2)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 2;
                                }
                            }
                            else if (obj.OrderStateNow == 300)
                            {
                                if (query.OrderStateNow >= 9)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 9;
                                }
                            }
                            else if (obj.OrderStateNow == 301)
                            {
                                if (query.OrderStateNow >= 10)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 10;
                                }
                            }
                            else if (obj.OrderStateNow == 400)
                            {
                                if (query.OrderStateNow == 11)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 11;
                                }
                            }
                        }
                        else if (items.OrderType == 3)
                        {
                            if (obj.OrderStateNow == 200)
                            {
                                if (query.OrderStateNow >= 2)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 2;
                                }
                            }
                            else if (obj.OrderStateNow == 300)
                            {
                                if (query.OrderStateNow >= 12)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 12;
                                }
                            }
                            else if (obj.OrderStateNow == 401)
                            {
                                if (query.OrderStateNow >= 13)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 13;
                                }
                            }
                            else if (obj.OrderStateNow == 500)
                            {
                                if (query.OrderStateNow == 14)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 14;
                                }
                            }
                        }
                        else if (items.OrderType == 4)
                        {
                            if (obj.OrderStateNow == 200)
                            {
                                if (query.OrderStateNow >= 2)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 2;
                                }
                            }
                            else if (obj.OrderStateNow == 300)
                            {
                                if (query.OrderStateNow == 17)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 17;
                                }
                            }
                        }
                        else if (items.OrderType == 5)
                        {
                            if (obj.OrderStateNow == 200)
                            {
                                if (query.OrderStateNow >= 2)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 2;
                                }
                            }
                            else if (obj.OrderStateNow == 300)
                            {
                                if (query.OrderStateNow >= 3)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 3;
                                }
                            }
                            else if (obj.OrderStateNow == 600)
                            {
                                if (query.OrderStateNow >= 4)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 4;
                                }
                            }
                            else if (obj.OrderStateNow == 700)
                            {
                                if (query.OrderStateNow >= 5)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 5;
                                }
                            }
                            else if (obj.OrderStateNow == 800)
                            {
                                if (query.OrderStateNow >= 6)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 6;
                                }
                            }
                            else if (obj.OrderStateNow == 900)
                            {
                                if (query.OrderStateNow >= 15)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 15;
                                }
                            }
                            else if (obj.OrderStateNow == 950)
                            {
                                if (query.OrderStateNow == 16)
                                {
                                    result.code = 1;
                                    result.message = $"该波次号当前状态为'{query.AfterStateC}'，请联系文员处理";
                                }
                                else
                                {
                                    items.OrderStateNow = 16;
                                }
                            }
                        }

                    }
                    acc_Details.ID = 0;
                    acc_Details.WavePick = items.WavePick;
                    acc_Details.Order = items.Order;
                    acc_Details.PCS = items.PCS;
                    acc_Details.SKU = items.SKU;
                    acc_Details.State = false;
                    acc_Details.OrderState = obj.OrderStateNow;
                    acc_Details.CreateTime = DateTime.Now;
                    acc_Details.UserID = obj.UserID;

                    WMS_Visualboard.Add(acc_Details);
                    
                    WMS_Visualboard.UpdateNotNull(items);

                    WMS_Visualboard.SaveChanges();


                    #endregion
                }
                //var WavePickDetailslist = obj.data.Select(p => new acc_WavePickDetails
                //{
                //    WavePick = p.WavePick,
                //    Order = p.Order,
                //    PCS = p.PCS,
                //    SKU = p.SKU,
                //    State = false,
                //    OrderState = 300,
                //    CreateTime = DateTime.Now
                //}).ToList();

                //WavePickDetailslist.Select(p => p.UserID = obj.UserID).ToList();


                //WMS_Visualboard.AddRange(WavePickDetailslist);

                //foreach (var item in WavePicklist)
                //{
                //    WMS_Visualboard.UpdateNotNull(item);
                //}




                result.result = obj;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }
    }
}

