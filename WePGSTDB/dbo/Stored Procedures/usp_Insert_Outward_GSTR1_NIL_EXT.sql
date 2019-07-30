
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert EXP Invoice Details
				
Written by  : nareshn@wepindia.com

Date		Who			Decription 
12/12/2017	Karthik		Initial Version


*/

/* Sample Procedure Call

exec usp_Insert_Outward_GSTR1_NIL_EXT '27GSPMH0802G1ZY','022018',12.00,0.0,0.0,'INTRB2B','WEP001',1
 */

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_NIL_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@nil_amt decimal(18,2),
	@expt_amt decimal(18,2),
	@ngsup_amt decimal(18,2),
	@sply_ty varchar(50),
	@ReferenceNo varchar(50),
	@CreatedBy int,
	@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @Gstinid int, @Gstr1id int, @nilid int, @Invid int, @Flag varchar(1)	
	Begin Try

		Select  @Gstinid = t1.gstinid,
				@Gstr1id = t1.gstr1id,
				--@nilid = t2.nilid,
				@Flag = flag
		FROM TBL_GSTR1 t1,
				TBL_GSTR1_NIL t2,
				TBL_GSTR1_NIL_inv t3
		Where t2.gstr1id = t1.gstr1id
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.sply_ty = @sply_ty

		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be inserted.
			Return
		End

		Insert into TBL_EXT_GSTR1_NIL
		(	gstin, fp, gt, cur_gt, nil_amt,expt_amt, ngsup_amt, sply_ty,
			rowstatus, sourcetype, referenceno, createddate,createdby)
		Values(	UPPER(@Gstin), @Fp, null, null,@nil_amt,@expt_amt,@ngsup_amt, @sply_ty,
				1 ,'Manual',@ReferenceNo,GetDate(),@CreatedBy)	

		if @@rowcount = 1 
		Begin
		if IsNull(@Flag,'') = ''
			Begin

				Delete From TBL_GSTR1_NIL_Inv
				Where invid = @Invid

				Delete From TBL_GSTR1_NIL
				Where nilid = @nilid

			End
					   
		End
		Select @Retval = 1 -- Nil data save successfully
		Return
	End Try
	Begin Catch
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select 'NIL -' + error_message()
		End				
	End Catch 
	Return 0

End