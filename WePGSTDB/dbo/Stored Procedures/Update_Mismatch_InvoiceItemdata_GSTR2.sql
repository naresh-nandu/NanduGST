--Created By: Ganesh Patil
--Purpose: To update invoice details
CREATE procedure [Update_Mismatch_InvoiceItemdata_GSTR2]
(
	@id int,
	@action varchar(10),
	@GstrType varchar(15),
	@txval decimal=0,
	@rt decimal,
	@iamt decimal,
	@camt decimal,
	@samt decimal,
	@csamt decimal

)
as
begin
	if(@GstrType ='GSTR2')
	begin
		
		if(@action='B2B')
		begin
				update TBL_GSTR2_B2B_inv_itms_det
				set txval=@txval,rt=@rt,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt
				where itmsid=@id

				update TBL_GSTR2_B2B_INV
				set val = (select sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt) from TBL_GSTR2_B2B_INV_ITMS_DET 
				where itmsid in (select itmsid from TBL_GSTR2_B2B_INV_ITMS where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@id))) 
				where invid=(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@id)

				update TBL_GSTR2_B2B_INV set flag='M' where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@id) 


				Declare @Gstin varchar(15), @Invnid int
				select @Gstin = gstin from TBL_GSTR2 where gstinid =(Select b2bid from TBL_GSTR2_B2B_INV where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@id))
				select @Invnid = invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@id

				insert into TBL_RECONCILIATION_LOGS(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
										values(@Gstin,@Invnid,'GSTR2','B2B','M','',4,1,GETDATE(),1)

		end
		Else
		Begin
			return 0
		End
		--else if(@action='CDN')
		--begin
		--        update TBL_GSTR2_CDNR_NT
		--		set irt=@irt,iamt=@iamt,crt=@crt,camt=@camt,srt=@srt,samt=@samt,
		--		csrt=@csrt,csamt=@csamt
		--		where invid=@id
		--end
	
	end
	else if(@GstrType ='GSTR2D')
	begin
		if(@action='B2B')
		begin
				update TBL_GSTR2A_B2B_inv_itms_det
				set txval=@txval,rt=@rt,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt
				where itmsid=@id

				update TBL_GSTR2A_B2B_INV
				set val = (select sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt) from TBL_GSTR2A_B2B_INV_ITMS_DET 
				where itmsid in (select itmsid from TBL_GSTR2A_B2B_INV_ITMS where invid =(select invid from TBL_GSTR2A_B2B_INV_ITMS where itmsid=@id))) 
				where invid=(select invid from TBL_GSTR2A_B2B_INV_ITMS where itmsid=@id)

		end
		Else
		Begin
			return 0
		End
		--else if(@action='CDNR')
		--begin		
		--		update TBL_GSTR2_D_CDN_CDN
		--		set irt=@irt,iamt=@iamt,crt=@crt,camt=@camt,srt=@srt,samt=@samt,
		--		csrt=@csrt,csamt=@csamt
		--		where cdnntid=@id
		--end
		
	end
end