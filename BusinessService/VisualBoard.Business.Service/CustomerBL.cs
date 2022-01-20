using Dapper;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Service
{
    public class CustomerBL : ICustomerBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IDbConnection connection;
        public CustomerBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection)
        {
            WMS_Visualboard = _WMS_Visualboard;
            connection = _connection;
        }



        /// <summary>
        /// 添加客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        public ResponseObject InsterCustomer(bsc_Customer _Customer)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                _Customer.CreateTime = DateTime.Now;
                _Customer.State = false;
                WMS_Visualboard.Add(_Customer);
                WMS_Visualboard.SaveChanges();

                var obj = _Customer;
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
        /// 修改客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        public ResponseObject UpdateCustomer(bsc_Customer _Customer)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                WMS_Visualboard.UpdateNotNull(_Customer);
                WMS_Visualboard.SaveChanges();

                var obj = _Customer;
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
        /// 查询一条客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        public ResponseObject SelectOneCustomer(bsc_Customer _Customer)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_Customer>(LambdaHelper.CreateWhere(_Customer)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 查询客户信息列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectListCustomer(CustmoerSearchObject _Customer)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_Customer>(LambdaHelper.CreateWhere(_Customer,TableName: "bsc_Customer"));
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        public ResponseObject DelCustomer(bsc_Customer _Customer)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                WMS_Visualboard.Remove(_Customer);
                WMS_Visualboard.SaveChanges();
                result.result=("删除成功");
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
