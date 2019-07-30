using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Ledger
{
    public class CashItcBalance
    {
        public CashItcBalance()
        {
            cash_bal = new CashBal();
            itc_bal = new ItcBal();
        }
        public string gstin { get; set; }
        public CashBal cash_bal { get; set; }
        public ItcBal itc_bal { get; set; }
    }
    public class Igstt
    {
        public int tx { get; set; }
        public int intr { get; set; }
        public int pen { get; set; }
        public int fee { get; set; }
        public int oth { get; set; }
    }

    public class Cgstt
    {
        public int tx { get; set; }
        public int intr { get; set; }
        public int pen { get; set; }
        public int fee { get; set; }
        public int oth { get; set; }
    }

    public class Sgstt
    {
        public int tx { get; set; }
        public int intr { get; set; }
        public int pen { get; set; }
        public int fee { get; set; }
        public int oth { get; set; }
    }

    public class Cesss
    {
        public int tx { get; set; }
        public int intr { get; set; }
        public int pen { get; set; }
        public int fee { get; set; }
        public int oth { get; set; }
    }

    public class CashBal
    {
        public CashBal()
        {
            igst = new Igstt();
            cgst = new Cgstt();
            sgst = new Sgstt();
            cess = new Cesss();
        }
        public int igst_tot_bal { get; set; }
        public Igstt igst { get; set; }
        public int cgst_tot_bal { get; set; }
        public Cgstt cgst { get; set; }
        public int sgst_tot_bal { get; set; }
        public Sgstt sgst { get; set; }
        public int cess_tot_bal { get; set; }
        public Cesss cess { get; set; }
    }

    public class ItcBal
    {
        public int cgst_bal { get; set; }
        public int sgst_bal { get; set; }
        public int igst_bal { get; set; }
        public int cess_bal { get; set; }
    }

    
}