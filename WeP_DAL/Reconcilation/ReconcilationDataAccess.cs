using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_DAL.Reconcilation
{
    public class ReconcilationDataAccess
    {
        public ReconcilationDataAccess()
        {
            //
        }

        public class ReconcilationViewModel
        {
            public List<MatchedInvoicesAttributes> MatchedInvoices { get; set; }

            public List<MissingInGstr2AAttributes> MissingInvoicesInGSTR2A { get; set; }

            public List<MissingInGstr2Attributes> MissingInvoicesInGSTR2 { get; set; }

            public List<MismatchInvoicesAttributes> MismatchInvoices { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributes> InvoiceCorrectionMatchedInvoices { get; set; }

            public List<InvoiceDataCorrectionMatchedSomeAttributes> InvoiceCorrectionMatchedInvoices_some { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributesIDT> InvoiceCorrectionMatchedInvoices_idt { get; set; }

            public List<InvoiceDataCorrectionMatchedAttributesIDTDuplicate> InvoiceCorrectionMatchedInvoices_idt_duplicate { get; set; }

            public List<InvoiceDataCorrectionAllGSTR2ttributes> InvoiceCorrectionAllInvoicesGSTR2 { get; set; }

            public List<InvoiceDataCorrectionAllGSTR2Attributes> InvoiceCorrectionAllInvoicesGSTR2A { get; set; }

            public List<ReconciliationSummaryAttributes> ReconciliationSummary { get; set; }

            public List<ReconciliationAmendmentGstr2_2a> AmendmentGstr2_2a { get; set; }

            public List<ReconciliationAmendmentGstr2a> AmendmentGstr2a { get; set; }

            public List<ReconciliationAmendment_Inum> Amendment_Inum { get; set; }

            public List<ReconciliationAmendment_Idt> Amendment_Idt { get; set; }

            public List<ReconciliationAmendment_Inum_Many> Amendment_Inum_Many { get; set; }

            public List<ReconciliationAmendment_Idt_Many> Amendment_Idt_Many { get; set; }

            public List<Reconciliation_MatchedHold> MatchedHold { get; set; }

        }

        public class MatchedInvoicesAttributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
        }

        public class MissingInGstr2AAttributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin { get; set; }
            public string ctin { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }
            public decimal TotaltaxAmount { get; set; }
            public string pos { get; set; }
            public string inv_typ { get; set; }
        }

        public class MissingInGstr2Attributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin { get; set; }
            public string ctin { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }
            public decimal TotaltaxAmount { get; set; }
            public string pos { get; set; }
            public string inv_typ { get; set; }
        }

        public class MismatchInvoicesAttributes
        {
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }

            public string ctin2a { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

        }

        public class InvoiceDataCorrectionMatchedAttributes
        {
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public string pos2 { get; set; }
            public string inv_typ2 { get; set; }
            public string ctin2a { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal TotaltaxAmount2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }

        }

        public class InvoiceDataCorrectionMatchedSomeAttributes
        {
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public string pos2 { get; set; }
            public string inv_typ2 { get; set; }
            public string ctin2a { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal TotaltaxAmount2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }

        }

        public class InvoiceDataCorrectionAllGSTR2ttributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin { get; set; }
            public string ctin { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }
            public decimal TotaltaxAmount { get; set; }
            public string pos { get; set; }
            public string inv_typ { get; set; }
        }

        public class InvoiceDataCorrectionAllGSTR2Attributes
        {
            public int invid { get; set; }
            public string ctin_inum { get; set; }
            public string gstin { get; set; }
            public string ctin { get; set; }
            public string nt_num { get; set; }
            public string nt_dt { get; set; }
            public string inum { get; set; }
            public string idt { get; set; }
            public decimal val { get; set; }
            public decimal txval { get; set; }
            public decimal TotaltaxAmount { get; set; }
            public string pos { get; set; }
            public string inv_typ { get; set; }
        }

        public class ReconciliationSummaryAttributes
        {
          
            public string gstin { get; set; }
            public string SupplierGstin { get; set; }
            public string SupplierName { get; set; }
            public decimal GSTR2Val { get; set; }
            public decimal GSTR2Taxamount { get; set; }
            public decimal GSTR2taxVal { get; set; }
            public decimal GSTR2AVal { get; set; }
            public decimal GSTR2ATaxamount { get; set; }
            public decimal GSTR2AtaxVal { get; set; }
            public decimal TaxDiff { get; set; }
            public decimal TaxDiffPercent { get; set; }
            public string supplierdetails { get; set; }
        }

        public class InvoiceDataCorrectionMatchedAttributesIDT
        {
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public string pos2 { get; set; }
            public string inv_typ2 { get; set; }
            public string ctin2a { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal TotaltaxAmount2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }

        }

        public class InvoiceDataCorrectionMatchedAttributesIDTDuplicate
        {
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public string pos2 { get; set; }
            public string inv_typ2 { get; set; }
            public string ctin2a { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal TotaltaxAmount2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }

        }
         
        public class ReconciliationAmendmentGstr2_2a
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class ReconciliationAmendmentGstr2a
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            public string inum2a { get; set; }
            public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class ReconciliationAmendment_Inum
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            //public string inum2a { get; set; }
            //public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            //public string nt_num2 { get; set; }
            //public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class ReconciliationAmendment_Idt
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            //public string inum2a { get; set; }
            //public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            //public string nt_num2 { get; set; }
            //public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class ReconciliationAmendment_Inum_Many
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            //public string inum2a { get; set; }
            //public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            //public string nt_num2 { get; set; }
            //public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class ReconciliationAmendment_Idt_Many
        {
            public string gstin2 { get; set; }
            public int invid2 { get; set; }
            public string ctin_inum { get; set; }
            public string ctin2 { get; set; }
            public string CtinName2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal iamt2 { get; set; }
            public decimal camt2 { get; set; }
            public decimal Samt2 { get; set; }
            public decimal Csamt2 { get; set; }
            public string pos2 { get; set; }
            public string gstin2a { get; set; }
            public int invid2a { get; set; }
            public string ctin2a { get; set; }
            public string CtinName2a { get; set; }
            //public string inum2a { get; set; }
            //public string idt2a { get; set; }
            public string oinum2a { get; set; }
            public string oidt2a { get; set; }
            public decimal val2a { get; set; }
            public decimal txval2a { get; set; }
            public decimal iamt2a { get; set; }
            public decimal camt2a { get; set; }
            public decimal Samt2a { get; set; }
            public decimal Csamt2a { get; set; }
            public string pos2a { get; set; }
            public string inv_typ2a { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

            //public string nt_num2 { get; set; }
            //public string nt_dt2 { get; set; }
            public string inv_typ2 { get; set; }
            public string nt_num2a { get; set; }
            public string nt_dt2a { get; set; }
            public string ont_num2a { get; set; }
            public string ont_dt2a { get; set; }
        }

        public class Reconciliation_MatchedHold
        {
            public int invid2a { get; set; }

            public int ntid2a { get; set; }
            public string ctin_inum { get; set; }
            public string gstin2 { get; set; }
            public string fp2 { get; set; }
            public string ctin2 { get; set; }
            public string nt_num2 { get; set; }
            public string nt_dt2 { get; set; }
            public string inum2 { get; set; }
            public string idt2 { get; set; }
            public decimal val2 { get; set; }
            public decimal txval2 { get; set; }
            public decimal TotaltaxAmount2 { get; set; }
            public decimal TotaltaxAmount2a { get; set; }

        }

    }
}
