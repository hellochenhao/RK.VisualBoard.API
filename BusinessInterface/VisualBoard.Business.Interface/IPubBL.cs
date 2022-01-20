using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Business.Interface
{
    public interface IPubBL
    {
        /// <summary>
        /// 获取映射关系表
        /// </summary>
        /// <param name="_Idreplace"></param>
        /// <returns></returns>
        public ResponseObject GetIDreplaceList(pub_Idreplace _Idreplace);
    }
}
