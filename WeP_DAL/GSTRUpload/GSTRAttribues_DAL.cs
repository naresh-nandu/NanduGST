using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.GSTRUpload
{
    public partial class GSTRAttribues_DAL
    {
        public GSTRAttribues_DAL()
        {

        }

        public int sno { get; set; }
        public int invid { get; set; }
        public int b2csid { get; set; }
        public int cdnurid { get; set; }
        public int dataid { get; set; }
        public int ntid { get; set; }
        public int atid { get; set; }
        public int txpid { get; set; }
        public int nilid { get; set; }
        public int docissueid { get; set; }
        public string gstin { get; set; }
        public string fp { get; set; }
        public Nullable<decimal> gt { get; set; }
        public Nullable<decimal> cur_gt { get; set; }
        public string ctin { get; set; }
        public string flag { get; set; }
        public string chksum { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public string ntty { get; set; }
        public string nt_num { get; set; }
        public string nt_dt { get; set; }
        public Nullable<decimal> val { get; set; }
        public string pos { get; set; }
        public string rchrg { get; set; }
        public string etin { get; set; }
        public string inv_typ { get; set; }
        public Nullable<decimal> rt { get; set; }
        public Nullable<decimal> txval { get; set; }
        public Nullable<decimal> iamt { get; set; }
        public Nullable<decimal> camt { get; set; }
        public Nullable<decimal> samt { get; set; }
        public Nullable<decimal> csamt { get; set; }
        public Nullable<decimal> ad_amt { get; set; }
        public Nullable<decimal> nil_amt { get; set; }
        public Nullable<decimal> expt_amt { get; set; }
        public Nullable<decimal> ngsup_amt { get; set; }
        public string hsn_sc { get; set; }
        public string descs { get; set; }
        public Nullable<decimal> qty { get; set; }
        public string uqc { get; set; }
        public string ex_tp { get; set; }
        public string sply_ty { get; set; }
        public string sbnum { get; set; }
        public string sbdt { get; set; }
        public string doc_num { get; set; }
        public string froms { get; set; }
        public string tos { get; set; }
        public string totnum { get; set; }
        public string cancel { get; set; }
        public string net_issue { get; set; }
        public string omon { get; set; }
        public Nullable<decimal> diff_percent { get; set; }
        public string ont_dt { get; set; }
        public string ont_num { get; set; }
        public string oinum { get; set; }
        public string oidt { get; set; }

    }
}
