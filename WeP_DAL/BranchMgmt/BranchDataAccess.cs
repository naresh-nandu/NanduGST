using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.BranchMgmt
{
    public class BranchDataAccess
    {
        public BranchDataAccess()
        {

        }
        public class BrachAttributes
        {
            public int branchId { get; set; }
            public string company { get; set; }
            public string gstinNo { get; set; }
            public string branchName { get; set; }
            public string email { get; set; }
        }

        public class UserAccessLocation
        {
            public int UserId { get; set; }
            public string Name { get; set; }
        }

    }

    public class BranchViewModel
    {
        public List<BranchDataAccess.BrachAttributes> BranchList { get; set; }

        public List<BranchDataAccess.UserAccessLocation> UALList { get; set; }
    }
}
