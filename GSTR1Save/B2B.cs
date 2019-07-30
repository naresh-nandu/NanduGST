using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Save
{
    public class B2BItmDet
    {        
        public decimal? rt { get; set; }
                
        public decimal? txval { get; set; }
                
        public decimal? iamt { get; set; }
                
        public decimal? camt { get; set; }
                
        public decimal? samt { get; set; }
                
        public decimal? csamt { get; set; }
    }

    public class B2BItm
    {        
        public int num { get; set; }

        public B2BItmDet itm_det { get; set; }
    }

    public class B2BInv
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string inum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string idt { get; set; }

        public decimal? val { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string pos { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string rchrg { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string etin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string inv_typ { get; set; }
                
        public decimal? diff_percent { get; set; }

        public List<B2BItm> itms { get; set; }
    }

    public class B2B
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ctin { get; set; }

        public List<B2BInv> inv { get; set; }
    }
    
    //---------------------

    public class B2BBatch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string gstin { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fp { get; set; }
                
        public decimal gt { get; set; }
                
        public decimal cur_gt { get; set; }

        public List<B2B> b2b { get; set; }
    }
    

}
