
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the GSTR2 invoice Flag and Check sum updation for mismatched data
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
10/29/2017	Karthik 	Initial Version
10/29/2017	Seshadri	Finetuned the code
*/

/* Sample Procedure Call

exec usp_Update_GSTR2_MisMatch_Inv


 */
Create PROCEDURE [usp_Update_Mismatched_Invoices]
(
	
	@ActionType varchar(15),
	@GSTR2invid int,
	@GSTR2Ainvid int,
	@UserId int,
	@CustId int
)
as
Begin

Declare @chksum varchar(100),@Gstin varchar(15)
	
		if @ActionType='B2B'
		begin
				if exists (select invid from tbl_gstr2A_b2b_Inv where invid = @GSTR2Ainvid)
				Begin
				select @chksum=chksum from tbl_gstr2A_b2b_Inv  where invid = @GSTR2Ainvid
				update tbl_gstr2A_b2b_Inv set flag ='M'  where invid = @GSTR2Ainvid
				update tbl_gstr2_b2b_Inv set flag ='M',chksum = @chksum  where invid =@GSTR2invid

				select @Gstin = gstin from TBL_GSTR2 where gstinid = (Select gstinid from TBL_GSTR2_B2B_INV where invid = @GSTR2invid)

				insert into TBL_RECONCILIATION_LOGS(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
										values(@Gstin,@GSTR2invid,'GSTR2','B2B','M','',@userId,@custId,GETDATE(),1)
				
				End				
			End
		Else If @actiontype = 'CDNR' or @actiontype = 'CDN'
		Begin
			
			if exists (select ntid from TBL_GSTR2A_CDNR_NT where ntid = @GSTR2Ainvid)
				Begin
				select @chksum=chksum from TBL_GSTR2A_CDNR_NT  where ntid = @GSTR2Ainvid
				update TBL_GSTR2A_CDNR_NT set flag ='M'  where ntid = @GSTR2Ainvid
				update TBL_GSTR2_CDNR_NT set flag ='M',chksum = @chksum  where invid =@GSTR2invid

				select @Gstin = gstin from TBL_GSTR2 where gstinid =(Select gstinid from TBL_GSTR2_CDNR_NT where invid = @GSTR2invid)

				insert into TBL_RECONCILIATION_LOGS(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
										values(@Gstin,@GSTR2invid,'GSTR2','CDNR','M','',@userId,@custId,GETDATE(),1)
				
				End
		End
		Else
			Begin
				select 'No Data Found' as 'Message'
			End
End