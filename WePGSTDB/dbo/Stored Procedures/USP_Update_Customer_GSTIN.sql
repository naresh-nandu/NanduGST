/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update GSTIN Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
17/11/2017	Karthik			Initial Version
*/

/* Sample Procedure Call  

exec USP_Update_Customer_GSTIN 1,1,'M',1,'33GSPTN0801G1ZM','33','GSPTN0801G','WeP.TN.1',''

 */
CREATE PROCEDURE [USP_Update_Customer_GSTIN]
	@custid    int,
	@gstinid	int,
	@Action varchar(2),
	@CreatedBy	int,
	@GstinNo	varchar(15) = NULL,	
	@statecode  varchar(2) = NULL,
	@PANNo  varchar(10) = NULL,
	@GstinUserName varchar(50) = NULL,
	@Address varchar(255) = NULL,
	@RetValue int=null Out,
	@RetMessage varchar(250) = NULL Out

AS
BEGIN

	if @Action = 'M'
		Begin
			if  Ltrim(Rtrim(IsNull(@GstinNo,''))) <> '' and Ltrim(Rtrim(IsNull(@statecode,''))) <> '' and Ltrim(Rtrim(IsNull(@GstinUserName,''))) <> ''
				Begin
				Declare @GStinCount int
				select @GStinCount= count(*) from TBL_Cust_Gstin where gstinno = @GstinNo and rowstatus =1 and  gstinid <> @gstinid

					if @GStinCount = 0
						Begin	
							if exists(select 1 from Tbl_Customer where custId = @CustId and rowstatus =1 and GstinNo = (select GstinNo from Tbl_Cust_Gstin where CustId = @CustId and Rowstatus =1 and GstinId = @gstinid))
							Begin
								Update Tbl_Customer set GstinNo = @GstinNo,PANNo = @PANNo,statecode = @statecode,ModifiedBy =@CreatedBy, ModifiedDate = getdate()
								where custId = @CustId and rowstatus =1 and GstinNo = (select GstinNo from Tbl_Cust_Gstin where CustId = @CustId and Rowstatus =1 and GstinId = @gstinid)
							 End													
							update TBL_Cust_Gstin set GSTINNO =@GstinNo,
													  Statecode = @statecode,
													  PanNo=@PANNo,
													  GSTINUserName =@GstinUserName,
													  [Address] = @Address,
													  ModifiedBy = @CreatedBy,
													  ModifiedDate = getdate()
									Where gstinid = @gstinid and custid =@CustId and rowstatus =1
							set @RetValue = 1
							set @RetMessage = 'GSTIN Details Modified Successfully.'
													
						End
					Else
						Begin
							set @RetValue = -1
							set @RetMessage = 'New GSTIN No is already exists. So unable to Modify the GSTIN No'
						End
				End
			Else
				Begin
					set @RetValue = -2
					set @RetMessage = 'GSTIN Details should not be empty.'
						
				End
		End
	Else If @Action = 'D'
		Begin
				if Not exists(select * from Tbl_Customer where custId = @CustId and rowstatus =1 and GstinNo = (select GstinNo from Tbl_Cust_Gstin where CustId = @CustId and Rowstatus =1 and GstinId = @gstinid))
					Begin
							update TBL_Cust_Gstin set rowstatus = 0,
												ModifiedBy = @CreatedBy,
												ModifiedDate = getdate()
							Where gstinid = @gstinid and custid =@CustId and rowstatus =1
							set @RetValue = 1
							set @RetMessage = 'GSTIN Deleted Successfully.'						
					End
				Else
					Begin
							set @RetValue = -3
							set @RetMessage = 'Access Denied! Master GSTIN cannot be deleted.'	
					End
		End

	Select convert(varchar(10), @RetValue) + ' : ' + @RetMessage as 'Output Result'
End