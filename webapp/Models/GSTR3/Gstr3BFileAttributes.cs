using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartAdminMvc.Models.GSTR3
{

    public class Gstr3BOsupDet
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BOsupZero
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BOsupNilExmp
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BIsupRev
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BOsupNongst
    {
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BSupDetails
    {
        public Gstr3BOsupDet osup_det { get; set; }
        public Gstr3BOsupZero osup_zero { get; set; }
        public Gstr3BOsupNilExmp osup_nil_exmp { get; set; }
        public Gstr3BIsupRev isup_rev { get; set; }
        public Gstr3BOsupNongst osup_nongst { get; set; }
    }

    public class Gstr3BUnregDetail
    {
        public string pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
    }

    public class Gstr3BCompDetail
    {
        public string pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
    }

    public class Gstr3BUinDetail
    {
        public string pos { get; set; }
        public decimal txval { get; set; }
        public decimal iamt { get; set; }
    }

    public class Gstr3BInterSup
    {
        public List<Gstr3BUnregDetail> unreg_details { get; set; }
        public List<Gstr3BCompDetail> comp_details { get; set; }
        public List<Gstr3BUinDetail> uin_details { get; set; }
    }

    public class Gstr3BItcAvl
    {
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BItcRev
    {
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BItcNet
    {
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BItcInelg
    {
        public string ty { get; set; }
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BItcElg
    {
        public List<Gstr3BItcAvl> itc_avl { get; set; }
        public List<Gstr3BItcRev> itc_rev { get; set; }
        public Gstr3BItcNet itc_net { get; set; }
        public List<Gstr3BItcInelg> itc_inelg { get; set; }
    }

    public class Gstr3BIsupDetail
    {
        public string ty { get; set; }
        public decimal inter { get; set; }
        public decimal intra { get; set; }
    }

    public class Gstr3BInwardSup
    {
        public List<Gstr3BIsupDetail> isup_details { get; set; }
    }

    public class Gstr3BIntrDetails
    {
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BLtfeeDetails
    {
        public decimal iamt { get; set; }
        public decimal camt { get; set; }
        public decimal samt { get; set; }
        public decimal csamt { get; set; }
    }

    public class Gstr3BIntrLtfee
    {
        public Gstr3BIntrDetails intr_details { get; set; }
        public Gstr3BLtfeeDetails ltfee_details { get; set; }
    }

    public class Gstr3BIgst
    {
        public decimal tx { get; set; }
        public decimal intr { get; set; }
        public decimal fee { get; set; }
    }

    public class Gstr3BCgst
    {
        public decimal tx { get; set; }
        public decimal intr { get; set; }
        public decimal fee { get; set; }
    }

    public class Gstr3BSgst
    {
        public decimal tx { get; set; }
        public decimal intr { get; set; }
        public decimal fee { get; set; }
    }

    public class Gstr3BCess
    {
        public decimal tx { get; set; }
        public decimal intr { get; set; }
        public decimal fee { get; set; }
    }

    public class Gstr3BTxPy
    {
        public string tran_desc { get; set; }
        public int liab_ldg_id { get; set; }
        public int trans_typ { get; set; }
        public Gstr3BIgst igst { get; set; }
        public Gstr3BCgst cgst { get; set; }
        public Gstr3BSgst sgst { get; set; }
        public Gstr3BCess cess { get; set; }
        public string trans_desc { get; set; }
    }

    public class Gstr3BPdcash
    {
        public int liab_ldg_id { get; set; }
        public int trans_typ { get; set; }
        public decimal ipd { get; set; }
        public decimal cpd { get; set; }
        public decimal spd { get; set; }
        public decimal cspd { get; set; }
        public decimal i_intrpd { get; set; }
        public decimal c_intrpd { get; set; }
        public decimal s_intrpd { get; set; }
        public decimal cs_intrpd { get; set; }
        public decimal c_lfeepd { get; set; }
        public decimal s_lfeepd { get; set; }
    }

    public class Gstr3BPditc
    {
        public int liab_ldg_id { get; set; }
        public int trans_typ { get; set; }
        public decimal i_pdi { get; set; }
        public decimal i_pdc { get; set; }
        public decimal i_pds { get; set; }
        public decimal c_pdi { get; set; }
        public decimal c_pdc { get; set; }
        public decimal s_pdi { get; set; }
        public decimal s_pds { get; set; }
        public decimal cs_pdcs { get; set; }
    }

    public class Gstr3BTxPmt
    {
        public List<Gstr3BTxPy> tx_py { get; set; }
        public List<Gstr3BPdcash> pdcash { get; set; }
        public Gstr3BPditc pditc { get; set; }
    }

    public class Gstr3BFileAttributesPartA
    {
        public string gstin { get; set; }
        public string ret_period { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Gstr3BSupDetails sup_details { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Gstr3BInterSup inter_sup { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Gstr3BItcElg itc_elg { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Gstr3BInwardSup inward_sup { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Gstr3BIntrLtfee intr_ltfee { get; set; }
    }

    public class Gstr3BFileAttributesPartB
    {
        public Gstr3BTxPmt tx_pmt { get; set; }
    }
}