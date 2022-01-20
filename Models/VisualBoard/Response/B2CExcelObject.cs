using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class B2CExcelObject
    {

        public string Filiale { get; set; }
        public string Project { get; set; }
        public DateTime? Date { get; set; }
        public int? IncomingD { get; set; }
        public int? IncomingJ { get; set; }
        public int? SaleJ { get; set; }
        public int? SaleSKU { get; set; }
        public int? PoorSKU { get; set; }
        public int? PoorJ { get; set; }
        public int? StorageNum { get; set; }
        public int? StorageUse { get; set; }
        public decimal? StorageLV { get; set; }
        public int? OrderReception { get; set; }
        public int? OrderAccomplish { get; set; }
        public int? OrderAccomplishJ { get; set; }
        public int? ShipmentsDsku { get; set; }
        public decimal? ShipmentsJDLV { get; set; }
        public decimal? ShipmentsLV { get; set; }
        public int? DutiableD { get; set; } 
        public int? ReallyShipmentsD { get; set; }
        public int? ReallySKU { get; set; }
        public int? ReallyShipmentsJ { get; set; }
        public decimal? DutiableLV { get; set; }
        public int? ZTO { get; set; }
        public int? STO { get; set; }
        public int? YTO { get; set; }
        public int? YD { get; set; }
        public int? EMS { get; set; }
        public int? SF { get; set; }
        public string HD_D { get; set; }
        public string HD_J { get; set; }
        public string XN_D { get; set; }
        public string XN_J { get; set; }
        public string MPS_D { get; set; }
        public string MPS_J { get; set; }
        public string Single { get; set; }
        public string Piece { get; set; }
        public int? CancelPiece { get; set; }
        public int? SalesSingle { get; set; }
        public int? SalesPiece { get; set; }
        public int? OwnStaff { get; set; }
        public decimal? OwnStaffNum { get; set; }
        public int? LabourStaff { get; set; }
        public decimal? LabourStaffNum { get; set; }
        public int? SupportStaff { get; set; }
        public decimal? SupportStaffNum { get; set; }
        public decimal? LaborProductivity { get; set; }
        public decimal? Aging { get; set; }

    }
}
