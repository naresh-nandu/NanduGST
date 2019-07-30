
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the GSTR2 invoice items and ITC details
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
10/29/2017	Karthik 	Initial Version
10/29/2017	Seshadri	Finetuned the code
*/

/* Sample Procedure Call

exec usp_Update_GSTR2_Inv_Itms_ITC


 */
CREATE PROCEDURE [usp_Update_GSTR2_Inv_Itms_ITC]
(
	@RefId int,
	@ActionType varchar(15),
	@TxVal decimal=0,
	@Rt decimal,
	@Iamt decimal,
	@Camt decimal,
	@Samt decimal,
	@Csamt decimal,
	@Elg   varchar(2),
	@Tx_I  decimal(18,2),
	@Tx_C  decimal(18,2),
	@Tx_S  decimal(18,2),
	@Tx_Cs  decimal(18,2),
	@UserId int,
	@CustId int,
	@UpdateType varchar(20)

)
as
Begin
		
		Declare @Gstin varchar(15), @Invnid int

		if @actiontype='B2B'
		begin
				update TBL_GSTR2_B2B_INV_ITMS_DET
				set txval=@txval,rt=@rt,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt
				where itmsid = @refid

				update TBL_GSTR2_B2B_INV_ITMS_ITC
				set elg = @elg, tx_i = @tx_i, tx_c = @tx_c, tx_s = @tx_s, tx_cs = @tx_cs
				where itmsid = @refid

				if @UpdateType = 'MISMATCH'
				Begin

					update TBL_GSTR2_B2B_INV
					set val = (select sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt) from TBL_GSTR2_B2B_INV_ITMS_DET 
					where itmsid in (select itmsid from TBL_GSTR2_B2B_INV_ITMS where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@refid))) 
					where invid=(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@refid)

					update TBL_GSTR2_B2B_INV set flag='M' where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@refid) 


					select @Gstin = gstin from TBL_GSTR2 where gstinid =(Select b2bid from TBL_GSTR2_B2B_INV where invid =(select invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@refid))
					select @Invnid = invid from TBL_GSTR2_B2B_INV_ITMS where itmsid=@refid

					insert into TBL_RECONCILIATION_LOGS(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
											values(@Gstin,@Invnid,'GSTR2','B2B','M','',@userId,@custId,GETDATE(),1)
				End

		end
		Else If @actiontype = 'CDNR' or @actiontype = 'CDN'
		Begin
			
				update TBL_GSTR2_CDNR_NT_ITMS_DET
				set txval=@txval,rt=@rt,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt
				where itmsid = @refid

				update TBL_GSTR2_CDNR_NT_ITMS_ITC
				set elg = @elg, tx_i = @tx_i, tx_c = @tx_c, tx_s = @tx_s, tx_cs = @tx_cs
				where itmsid = @refid

				if @UpdateType = 'MISMATCH'
				Begin
					update TBL_GSTR2_CDNR_NT
					set val = (select sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt) from TBL_GSTR2_CDNR_NT_ITMS_DET 
					where itmsid in (select itmsid from TBL_GSTR2_CDNR_NT_ITMS where invid =(select invid from TBL_GSTR2_CDNR_NT_ITMS where itmsid=@refid))) 
					where invid=(select invid from TBL_GSTR2_CDNR_NT_ITMS where itmsid=@refid)

					update TBL_GSTR2_CDNR_NT set flag='M' where invid =(select invid from TBL_GSTR2_CDNR_NT_ITMS where itmsid=@refid) 


					select @Gstin = gstin from TBL_GSTR2 where gstinid =(Select cdnrid from TBL_GSTR2_CDNR_NT where invid =(select invid from TBL_GSTR2_CDNR_NT_ITMS where itmsid=@refid))
					select @Invnid = invid from TBL_GSTR2_CDNR_NT_ITMS where itmsid=@refid

					insert into TBL_RECONCILIATION_LOGS(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
											values(@Gstin,@Invnid,'GSTR2','CDNR','M','',@userId,@custId,GETDATE(),1)
				End


		End
		Else
			Begin
				select 'No Data Found' as 'Message'
			End
End