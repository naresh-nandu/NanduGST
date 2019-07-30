﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSTR1Download
{
    public class B2cl
    {
        public string state_cd { get; set; }
        public List<B2cl_Inv> inv { get; set; }
    }

    public class B2cl_Inv
    {
        public string chksum { get; set; }
        public string cname { get; set; }
        public string inum { get; set; }
        public string idt { get; set; }
        public double val { get; set; }
        public string pos { get; set; }
        public string prs { get; set; }
        public string od_num { get; set; }
        public string od_dt { get; set; }
        public string etin { get; set; }
        public List<B2cl_Itm> itms { get; set; }
    }

    public class B2cl_Itm
    {
        public int num { get; set; }
        public B2cl_ItmDet itm_det { get; set; }
    }

    public class B2cl_ItmDet
    {
        public string ty { get; set; }
        public string hsn_sc { get; set; }
        public int txval { get; set; }
        public int irt { get; set; }
        public double iamt { get; set; }
        public int csrt { get; set; }
        public int csamt { get; set; }
    }


}
