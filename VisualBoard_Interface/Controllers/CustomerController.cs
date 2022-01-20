using Microsoft.AspNetCore.Authorization;
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
using VisualBoard_Interface.Common;

namespace VisualBoard_Interface.Controllers
{
    /// <summary>
    /// 顾客信息控制器
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerBL customerBL;

        public CustomerController(ICustomerBL customerBL)
        {
            this.customerBL = customerBL;
        }


        /// <summary>
        /// 添加客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject InsterCustomer(bsc_Customer _Customer)
        {
            return customerBL.InsterCustomer(_Customer);
        }


        /// <summary>
        /// 修改客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject UpdateCustomer(bsc_Customer _Customer)
        {
            return customerBL.UpdateCustomer(_Customer);
        }


        /// <summary>
        /// 查询一条客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        [HttpPost]
        [NoLogin]
        public ResponseObject SelectOneCustomer(bsc_Customer _Customer)
        {
            return customerBL.SelectOneCustomer(_Customer);
        }


        /// <summary>
        /// 查询客户信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseObject SelectListCustomer(CustmoerSearchObject _Customer)
        {
            return customerBL.SelectListCustomer(_Customer);
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="_Customer"></param>
        /// <returns></returns>
        [HttpDelete]
        public ResponseObject DelCustomer(bsc_Customer _Customer)
        {
            return customerBL.DelCustomer(_Customer);
        }

    }
}
