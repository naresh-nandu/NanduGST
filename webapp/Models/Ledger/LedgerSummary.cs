using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Ledger
{
    public class LedgerSummary
    {
        public LedgerSummary()
        {
            ITC_Ledger = new ItcLedger();
            LiabilityLedger = new LiabilityLedger();
            CashITCBalance = new CashItcBalance();
        }
        public ItcLedger ITC_Ledger { get; set; }
        public LiabilityLedger LiabilityLedger { get; set; }
        public CashItcBalance CashITCBalance { get; set; }
    }
}