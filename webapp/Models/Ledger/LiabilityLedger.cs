using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Ledger
{
    public class LiabilityLedger
    {
        public LiabilityLedger()
        {
            tr = new List<Tr>();
            cl_bal = new ClBal();
        }
        public string gstin { get; set; }
        public string ret_period { get; set; }
        public List<Tr> tr { get; set; }
        public ClBal cl_bal { get; set; }
    }
    public class Igst
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Sgst
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cgst
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cess
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Igstbal
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Sgstbal
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cgstbal
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cessbal
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Tr
    {
        
        public string dt { get; set; }
        public string refNo { get; set; }
        public int tot_rng_bal { get; set; }
        public int tot_tr_amt { get; set; }
        public string tr_typ { get; set; }
        public string desc { get; set; }
        public string dschrg_typ { get; set; }
        public Igst igst { get; set; }
        public Sgst sgst { get; set; }
        public Cgst cgst { get; set; }
        public Cess cess { get; set; }
        public Igstbal igstbal { get; set; }
        public Sgstbal sgstbal { get; set; }
        public Cgstbal cgstbal { get; set; }
        public Cessbal cessbal { get; set; }
    }

    public class Igstbal2
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Sgstbal2
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cgstbal2
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class Cessbal2
    {
        public int intr { get; set; }
        public int oth { get; set; }
        public int tx { get; set; }
        public int fee { get; set; }
        public int pen { get; set; }
        public int tot { get; set; }
    }

    public class ClBal
    {
        public ClBal()
        {
            igstbal = new Igstbal2();
            sgstbal = new Sgstbal2();
            cgstbal = new Cgstbal2();
            cessbal = new Cessbal2();


        }
        public int tot_rng_bal { get; set; }
        public string desc { get; set; }
        public Igstbal2 igstbal { get; set; }
        public Sgstbal2 sgstbal { get; set; }
        public Cgstbal2 cgstbal { get; set; }
        public Cessbal2 cessbal { get; set; }
    }

   

}