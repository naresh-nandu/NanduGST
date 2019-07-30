
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the Invoice Flag for all GSTR1_D Records in the staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/21/2017	Seshadri 	Initial Version
08/30/2017	Seshadri	Modified to include Current and New Flag Values
*/

/* Sample Procedure Call

usp_Update_Invoice_Flag_GSTR1_D


 */
 
CREATE PROCEDURE [usp_Update_Invoice_Flag_GSTR1_D]  
	@Gstin varchar(15),
	@Fp varchar(10),
	@CurFlag varchar(1),
	@NewFlag varchar(1)

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Update TBL_GSTR1_D_B2B_INV
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 

	Update TBL_GSTR1_D_B2CL_INV
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 

	Update TBL_GSTR1_D_B2CS
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 

	Update TBL_GSTR1_D_EXP_INV
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 
	
	Update TBL_GSTR1_D_CDNR_NT
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	)
	
	Update TBL_GSTR1_CDNUR
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1id in
	(
		Select TBL_GSTR1.gstr1id 
		From TBL_GSTR1 
		Where TBL_GSTR1.gstin = @Gstin 
		And TBL_GSTR1.fp = @Fp
	)
	
	Update TBL_GSTR1_D_HSN
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	)
	
	Update TBL_GSTR1_D_NIL
	Set flag = @NewFlag
	Where flag = @CurFlag 
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	)  
	
	Update TBL_GSTR1_D_TXPD
	Set flag = @NewFlag
	Where flag = @CurFlag 
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 

	Update TBL_GSTR1_D_AT
	Set flag = @NewFlag
	Where flag = @CurFlag 
	And gstr1did in
	(
		Select TBL_GSTR1_D.gstr1did 
		From TBL_GSTR1_D 
		Where TBL_GSTR1_D.gstin = @Gstin 
		And TBL_GSTR1_D.fp = @Fp
	) 

	Update TBL_GSTR1_DOC_ISSUE
	Set flag = @NewFlag
	Where flag = @CurFlag
	And gstr1id in
	(
		Select TBL_GSTR1.gstr1id 
		From TBL_GSTR1 
		Where TBL_GSTR1.gstin = @Gstin 
		And TBL_GSTR1.fp = @Fp
	) 
		
	Return 0

End