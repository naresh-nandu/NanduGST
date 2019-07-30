using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Ledger
{
    public class ItcLedger
    {
        public ItcLedger()
        {
            itcLdgDtls = new ItcLdgDtls();
            provCrdBalList = new ProvCrdBalList();
        }
        public ItcLdgDtls itcLdgDtls { get; set; }
        public ProvCrdBalList provCrdBalList { get; set; }
    }
    public class OpBal
    {
        public string desc { get; set; }
        public int igstTaxBal { get; set; }
        public int cgstTaxBal { get; set; }
        public int sgstTaxBal { get; set; }
        public int cessTaxBal { get; set; }
        public int tot_rng_bal { get; set; }
    }

    public class Trr
    {
        public string dt { get; set; }
        public string desc { get; set; }
        public string ref_no { get; set; }
        public string ret_period { get; set; }
        public int sgstTaxAmt { get; set; }
        public int cgstTaxAmt { get; set; }
        public int igstTaxAmt { get; set; }
        public int cessTaxAmt { get; set; }
        public int igstTaxBal { get; set; }
        public int cgstTaxBal { get; set; }
        public int sgstTaxBal { get; set; }
        public int cessTaxBal { get; set; }
        public int tot_rng_bal { get; set; }
        public int tot_tr_amt { get; set; }
        public string tr_typ { get; set; }
    }

    public class ClBall
    {
        public string desc { get; set; }
        public int igstTaxBal { get; set; }
        public int cgstTaxBal { get; set; }
        public int sgstTaxBal { get; set; }
        public int cessTaxBal { get; set; }
        public int tot_rng_bal { get; set; }
    }

    public class ItcLdgDtls
    {
        public ItcLdgDtls()
        {
            tr = new List<Trr>();
            cl_bal = new ClBall();
            op_bal = new OpBal();
        }
        public string gstin { get; set; }
        public string fr_dt { get; set; }
        public string to_dt { get; set; }
        public OpBal op_bal { get; set; }
        public List<Trr> tr { get; set; }
        public ClBall cl_bal { get; set; }
    }

    public class ProvCrdBal
    {
        public string ret_period { get; set; }
        public int cgstProCrBal { get; set; }
        public int igstProCrBal { get; set; }
        public int sgstProCrBal { get; set; }
        public int cessProCrBal { get; set; }
        public int totProCrBal { get; set; }
    }

    public class ProvCrdBalList
    {
        public ProvCrdBalList()
        {
            provCrdBal = new List<ProvCrdBal>();
        }
        public List<ProvCrdBal> provCrdBal { get; set; }
    }

    
}