using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class Gstr3b
    {
        public Gstr3b()
        {
            sup_details = new SupDetails();
            InterSup = new Inter_Sup();
            ItcElg = new ItcElg();
            IntrLtfee = new IntrLtfee();
            Inward_sup = new InwardSup();
        }
        public string gstinid { get; set; }
        public string fp { get; set; }
        public SupDetails sup_details { get; set; }
        public Inter_Sup InterSup { get; set; }
        public ItcElg ItcElg { get; set; }
        public IntrLtfee IntrLtfee { get; set; }
        public InwardSup Inward_sup { get; set; }
    }
    public class SupDetails
    {
      
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }

    }
    public class Inter_Sup
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
    }
    public class ItcElg
    {
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }

    }
    public class IntrLtfee
    {
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }

    }
    public class InwardSup
    {
        public decimal inter { get; set; }
        public decimal intra { get; set; }
    }
}