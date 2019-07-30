/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert EXP Invoice Details
				
Written by  : nareshn@wepindia.com

Date		Who			Decription 
11/12/2017	Naresh		Initial Version
13/12/2017  Karthik		Validations are inculded


*/

/* Sample Procedure Call

exec usp_Insert_Outward_GSTR1_AT_TXP_EXT 
 */

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_AT_TXP_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Action varchar(15),
	@Pos varchar(50),
	@Rt decimal(18,2),
	@AdvanceAmount decimal(18,2),
	@Igst decimal(18,2),
	@Cgst decimal(18,2),
	@Sgst decimal(18,2),
	@Cess decimal(18,2),
	@ReferenceNo varchar(50),
	@CreatedBy int,
	@RetVal varchar(50)= null Out

-- /*mssx*/ With Encryption 
as 
Begin
	Set Nocount on
	Declare @Gstinid int, @Gstr1id int, @Atid int, @TXPId int, @itmsid int, @Flag varchar(1)	

	Declare @SourceType varchar(15),@SupplyType varchar(10),@TPpos varchar(2)

	Select @SourceType = 'Manual'
	select @TPpos= convert(varchar(2),substring(@Gstin,1,2))
	if @TPpos <> @Pos
		Begin
			Set @SupplyType = 'INTER'
		End
		Else
		Begin
			Set @SupplyType = 'INTRA'
		End

	if @Action='AT' 
	 Begin
		
		if not exists(select 1 from TBL_EXT_GSTR1_AT where gstin = @Gstin and fp=@Fp and pos = @Pos and sply_ty = @SupplyType and rt = @rt and SourceType = @SourceType and referenceNo = @ReferenceNo and rowstatus =1)
			Begin
				Insert into TBL_EXT_GSTR1_AT
				(	gstin,fp,pos,sply_ty,rt,ad_amt,iamt,camt,samt,csamt,
					rowstatus, sourcetype, referenceno,createdby,createddate)
					Values(UPPER(@Gstin), @Fp,@Pos,@SupplyType,@Rt,@AdvanceAmount,@Igst,@Cgst,@Sgst,@Cess,
					1 ,@SourceType,@ReferenceNo,@CreatedBy,GetDate())
			End
		Else
			Begin
					Update TBL_EXT_GSTR1_AT 
					Set	ad_amt = IsNull(t1.ad_amt,0) + IsNull(@AdvanceAmount,0), 
						iamt = IsNull(t1.iamt,0) + IsNull(@Igst,0),
						camt = IsNull(t1.camt,0) + IsNull(@Cgst,0),
						samt = IsNull(t1.samt,0) + IsNull(@Sgst,0), 
						csamt = IsNull(t1.csamt,0) + IsNull(@Cess,0) 
					From TBL_EXT_GSTR1_AT t1
					Where  t1.gstin = @Gstin
						And t1.fp = @Fp
						And t1.pos = @Pos
						And t1.sply_ty = @SupplyType
						And t1.rt = @Rt
						And t1.SourceType = @SourceType 
						And t1.referenceNo = @ReferenceNo 
						And rowstatus =1	
			End
			
			Select @Retval = 1 -- AT issue data save successfully
		Return
     END
	
   Else If @Action ='TXP'
	Begin

		if not exists(select 1 from TBL_EXT_GSTR1_TXP where gstin = @Gstin and fp=@Fp and pos = @Pos and sply_ty = @SupplyType and rt = @rt and SourceType = @SourceType and referenceNo = @ReferenceNo and rowstatus =1)
			Begin
				Insert into TBL_EXT_GSTR1_TXP
				(	gstin,fp,pos,sply_ty,rt,ad_amt,iamt,camt,samt,csamt,
					rowstatus, sourcetype, referenceno,createdby,createddate)
					Values(UPPER(@Gstin), @Fp,@Pos,@SupplyType,@Rt,@AdvanceAmount,@Igst,@Cgst,@Sgst,@Cess,
					1 ,@SourceType,@ReferenceNo,@CreatedBy,GetDate())
			End
		Else
			Begin
					Update TBL_EXT_GSTR1_TXP 
					Set	ad_amt = IsNull(t1.ad_amt,0) + IsNull(@AdvanceAmount,0), 
						iamt = IsNull(t1.iamt,0) + IsNull(@Igst,0),
						camt = IsNull(t1.camt,0) + IsNull(@Cgst,0),
						samt = IsNull(t1.samt,0) + IsNull(@Sgst,0), 
						csamt = IsNull(t1.csamt,0) + IsNull(@Cess,0) 
					From TBL_EXT_GSTR1_TXP t1
					Where  t1.gstin = @Gstin
						And t1.fp = @Fp
						And t1.pos = @Pos
						And t1.sply_ty = @SupplyType
						And t1.rt = @Rt
						And t1.SourceType = @SourceType 
						And t1.referenceNo = @ReferenceNo 
						And rowstatus =1	
			End
	

		Select @Retval = 1 -- TXP data save successfully
		Return
     END
End