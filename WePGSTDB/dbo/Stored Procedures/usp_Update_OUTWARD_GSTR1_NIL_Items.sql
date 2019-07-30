
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update NIL Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_NIL_Items 
 */
 
CREATE PROCEDURE [usp_Update_OUTWARD_GSTR1_NIL_Items]
		@NILid    int,
		@nil_amt	decimal(18,2),
		@expt_amt	decimal(18,2),
		@ngsup_amt	decimal(18,2),
		@sply_ty	varchar(50),
		@Createdby int,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	
	Set Nocount on
	
	Declare @Gstin varchar(15),@Fp varchar(10),@Flag varchar(1)
	if Exists(Select 1 from TBL_EXT_GSTR1_NIL 
			  Where nilid = @NILid and sply_ty = @sply_ty
			  And rowstatus in (0,1)
			 )
	Begin
	
				Select  @Gstin = gstin,
						@Fp = fp
				From TBL_EXT_GSTR1_NIL
				Where nilid = @NILid and sply_ty = @sply_ty

				Select @Flag = flag
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_NIL t2,
					 TBL_GSTR1_NIL_INV  t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.nilid = t3.nilid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And sply_ty = @sply_ty
	
				If IsNull(@Flag,'') <> ''
				Begin
					Select @Retval = -2 -- Item details cannot be updated.
					Return
				End
				
				Update TBL_EXT_GSTR1_NIL 
				Set nil_amt = @nil_amt,
					expt_amt = @expt_amt,
					ngsup_amt= @ngsup_amt
				where nilid = @NILid
				And sply_ty = @sply_ty
					
				-- Update the Values in Staging Area

				Update TBL_GSTR1_NIL_INV
				SET TBL_GSTR1_NIL_INV.nil_amt = @nil_amt,
					TBL_GSTR1_NIL_INV.expt_amt = @expt_amt,
					TBL_GSTR1_NIL_INV.ngsup_amt = @ngsup_amt
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_NIL t2,
					 TBL_GSTR1_NIL_INV t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.nilid = t3.nilid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And sply_ty = @sply_ty

			
				Select @Retval = 1 -- Item details updated and also the Invoice Value updated for the entire Invoice
	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End
End