using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR2Download
{
    public class B2B_ItmDet
    {
    public string ty { get; set; }
    public string hsn_sc { get; set; }
    public double txval { get; set; }
    public int irt { get; set; }
    public int iamt { get; set; }
    public double crt { get; set; }
    public double camt { get; set; }
    public double srt { get; set; }
    public double samt { get; set; }
    public double csrt { get; set; }
    public double csamt { get; set; }
    public string elg { get; set; }
    }

	public class B2B_Itc
	{
    public double tx_i { get; set; }
    public double tx_s { get; set; }
    public double tx_c { get; set; }
    public int tx_cs { get; set; }
    public int tc_c { get; set; }
    public double tc_i { get; set; }
    public double tc_s { get; set; }
    public int tc_cs { get; set; }
	}
    public class B2B_Itm
    {
    public int num { get; set; }
    public B2B_ItmDet itm_det { get; set; }
    public B2B_Itc itc { get; set; }
    }

    public class B2B_Inv
    {
    public string flag { get; set; }
    public string chksum { get; set; }
    public string inum { get; set; }
    public string idt { get; set; }
    public double val { get; set; }
    public string pos { get; set; }
    public string rchrg { get; set; }
    public string updby { get; set; }
    public List<B2B_Itm> itms { get; set; }
    }

    public class B2B
    {
    public string ctin { get; set; }
    public string cfs { get; set; }
    public List<B2B_Inv> inv { get; set; }
    }
    
}
