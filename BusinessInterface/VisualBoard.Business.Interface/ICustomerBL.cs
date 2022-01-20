using System;
using System.Collections.Generic;
using System.Text;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using VisualBoard.Models.Request;

namespace VisualBoard.Business.Interface
{
    public interface ICustomerBL
    {


        /// <summary>
        /// 新增客户
        /// </summary>
        /// <returns></returns>
        public ResponseObject InsterCustomer(bsc_Customer _Customer);
        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject UpdateCustomer(bsc_Customer _Customer);
        /// <summary>
        /// 查询一条客户信息
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectOneCustomer(bsc_Customer _Customer);

        /// <summary>
        /// 查询客户信息列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject SelectListCustomer(CustmoerSearchObject _Customer);

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        public ResponseObject DelCustomer(bsc_Customer _Customer);
    }
}
