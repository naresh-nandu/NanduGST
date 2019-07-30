using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.Reconcilation
{
    public class ReconcilationDataAccess6
    {
        //


        public class ReconcilationViewModel6
        {
            public List<MissingInGstr6AAttributes> MissingInvoicesInGSTR6A { get; set; }

            public List<MissingInGstr6Attributes> MissingInvoicesInGSTR6 { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributes> InvoiceCorrectionMatchedInvoices { get; set; }

            public List<InvoiceDataCorrectionMatchedManyAttributes> InvoiceCorrectionMatchedInvoices_some { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributesIDT> InvoiceCorrectionMatchedInvoices_idt { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributesIDTMany> InvoiceCorrectionMatchedInvoices_idt_many { get; set; }
        }
        public class MissingInGstr6AAttributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin { get; set; }
            public string ctin { get; set; }
            public string CtinName { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }         
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
            public decimal TotaltaxAmount { get; set; }
        }

        public class MissingInGstr6Attributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string ctin { get; set; }
            public string cfs { get; set; }
            public string gstin { get; set; }
            public string CtinName { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }
            public decimal iamt { get; set; }
            public decimal camt { get; set; }
            public decimal samt { get; set; }
            public decimal csamt { get; set; }
            public decimal TotaltaxAmount { get; set; }

        }

        public class InvoiceDataCorrectionMatchedAttributes
        {
            public int invid6 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin6 { get; set; }
            public string gstin6a { get; set; }
            public string ctin6 { get; set; }
            public string CtinName6 { get; set; }
            public string nt_num6 { get; set; }
            public string nt_dt6 { get; set; }
            public string inum6 { get; set; }
            public string idt6 { get; set; }
            public string pos6 { get; set; }
            public string pos6a { get; set; }
            public decimal val6 { get; set; }
            public decimal txval6 { get; set; }
            public decimal TotaltaxAmount6 { get; set; }
            public decimal iamt6 { get; set; }
            public decimal camt6 { get; set; }
            public decimal samt6 { get; set; }
            public decimal csamt6 { get; set; }
            public string CtinName6a { get; set; }
            public string ctin6a { get; set; }
            public string ntid6a { get; set; }
            public string nt_num6a { get; set; }
            public string nt_dt6a { get; set; }
            public string inum6a { get; set; }
            public string idt6a { get; set; }
            public decimal iamt6a { get; set; }
            public decimal camt6a { get; set; }
            public decimal samt6a { get; set; }
            public decimal csamt6a { get; set; }
            public decimal val6a { get; set; }
            public decimal txval6a { get; set; }
            public decimal TotaltaxAmount6a { get; set; }

        }

        public class InvoiceDataCorrectionMatchedManyAttributes
        {
            public int invid6 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin6 { get; set; }
            public string gstin6a { get; set; }
            public string ctin6 { get; set; }
            public string CtinName6 { get; set; }
            public string nt_num6 { get; set; }
            public string nt_dt6 { get; set; }
            public string inum6 { get; set; }
            public string idt6 { get; set; }
            public decimal val6 { get; set; }
            public decimal txval6 { get; set; }
            public decimal TotaltaxAmount6 { get; set; }
            public decimal iamt6 { get; set; }
            public decimal camt6 { get; set; }
            public decimal samt6 { get; set; }
            public decimal csamt6 { get; set; }
            public string CtinName6a { get; set; }
            public string pos6 { get; set; }
            public string pos6a { get; set; }
            public string ctin6a { get; set; }
            public string ntid6a { get; set; }
            public string nt_num6a { get; set; }
            public string nt_dt6a { get; set; }
            public string inum6a { get; set; }
            public string idt6a { get; set; }
            public decimal iamt6a { get; set; }
            public decimal camt6a { get; set; }
            public decimal samt6a { get; set; }
            public decimal csamt6a { get; set; }
            public decimal val6a { get; set; }
            public decimal txval6a { get; set; }
            public decimal TotaltaxAmount6a { get; set; }

        }

        public class InvoiceDataCorrectionMatchedAttributesIDT
        {
            public int invid6 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin6 { get; set; }
            public string ctin6 { get; set; }
            public string nt_num6 { get; set; }
            public string nt_dt6 { get; set; }
            public string inum6 { get; set; }
            public string idt6 { get; set; }
            public decimal val6 { get; set; }
            public decimal txval6 { get; set; }
            public decimal iamt6 { get; set; }
            public decimal camt6 { get; set; }
            public decimal samt6 { get; set; }
            public string CtinName6 { get; set; }
            public decimal csamt6 { get; set; }
            public decimal TotaltaxAmount6 { get; set; }
            public int ntid6 { get; set; }
            public string CtinName6a { get; set; }
            public string ctin6a { get; set; }
            public string nt_num6a { get; set; }
            public string nt_dt6a { get; set; }
            public string inum6a { get; set; }
            public string idt6a { get; set; }
            public decimal val6a { get; set; }
            public decimal txval6a { get; set; }
            public decimal iamt6a { get; set; }
            public decimal camt6a { get; set; }
            public decimal samt6a { get; set; }
            public decimal csamt6a { get; set; }
            public decimal TotaltaxAmount6a { get; set; }
           

        }

        public class InvoiceDataCorrectionMatchedAttributesIDTMany
        {
            public int invid6 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin6 { get; set; }
            public string ctin6 { get; set; }
            public string nt_num6 { get; set; }
            public string nt_dt6 { get; set; }
            public string inum6 { get; set; }
            public string idt6 { get; set; }
            public decimal val6 { get; set; }
            public decimal txval6 { get; set; }
            public decimal iamt6 { get; set; }
            public decimal camt6 { get; set; }
            public decimal samt6 { get; set; }
            public string CtinName6 { get; set; }
            public decimal csamt6 { get; set; }
            public decimal TotaltaxAmount6 { get; set; }
            public int ntid6 { get; set; }
            public string CtinName6a { get; set; }
            public string ctin6a { get; set; }
            public string nt_num6a { get; set; }
            public string nt_dt6a { get; set; }
            public string inum6a { get; set; }
            public string idt6a { get; set; }
            public decimal val6a { get; set; }
            public decimal txval6a { get; set; }
            public decimal iamt6a { get; set; }
            public decimal camt6a { get; set; }
            public decimal samt6a { get; set; }
            public decimal csamt6a { get; set; }
            public decimal TotaltaxAmount6a { get; set; }

        }
    }
}