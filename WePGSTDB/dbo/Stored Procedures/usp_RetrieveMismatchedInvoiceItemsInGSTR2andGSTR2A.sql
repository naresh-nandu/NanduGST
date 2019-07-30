
--Purpose: To retrieve details of invoice items
-- exec [Retrieve_Reconcile_Mismatch_InvoiceItems_GSTR2andGSTR2A] 380,'B2B','GSTR2'
CREATE procedure [usp_RetrieveMismatchedInvoiceItemsInGSTR2andGSTR2A]
(
@invid int,
@action varchar(10)
)
as
begin
	if(@action='B2B')
		begin
			select c.*,d.* from TBL_GSTR2_B2B_INV a 
			inner join TBL_GSTR2_B2B_INV_ITMS b on a.invid=b.invid 
			inner join TBL_GSTR2_B2B_INV_ITMS_DET c on b.itmsid=c.itmsid
			inner join TBL_GSTR2_B2B_INV_ITMS_ITC d on b.itmsid=d.itmsid
			where a.invid=@invid

			select val from TBL_GSTR2_B2B_INV where invid=@invid
		end
		else if(@action='CDNR' or @action='CDN')
		begin
			select * from TBL_GSTR2_CDNR_NT
			where invid=@invid
			
			select cdnrid from TBL_GSTR2_CDNR_NT where invid=@invid
		end		
		else
		Begin
			select 'No data Found' as 'Message'
		End
	End