using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Save
{    
    public class CdnrItmDet
    {        
        public decimal? rt { get; set; }
                
        public decimal? txval { get; set; }
                
        public decimal? iamt { get; set; }
                
        public decimal? camt { get; set; }
                
        public decimal? samt { get; set; }
                
        public decimal? csamt { get; set; }
    }

    public class CdnrItm
    {        
        public int num { get; set; }

        public CdnrItmDet itm_det { get; set; }
    }

    public class CdnrNt
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ntty { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nt_num { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nt_dt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string p_gst { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string inum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string idt { get; set; }
                
        public decimal? val { get; set; }
                
        public decimal? diff_percent { get; set; }

        public List<CdnrItm> itms { get; set; }
    }

    public class Cdnr
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ctin { get; set; }
        public List<CdnrNt> nt { get; set; }
    }
    
    //---------------------

    public class CdnrBatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string gstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fp { get; set; }
                
        public decimal gt { get; set; }
                
        public decimal cur_gt { get; set; }

        public List<Cdnr> cdnr { get; set; }
    }
    
}
