
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update Matched Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri	Initial Version
10/30/2017	Seshadri	Updated the ITC Values
10/31/2017	Seshadri	Updated the ITC Values for CDNR
11/9/2017	Seshadri	Fixed the ITC Updation and Invoice Mismatch Issue
11/9/2017	Seshadri	Reverted the fix
11/9/2017	Seshadri &  Refixed the ITC Updation and Invoice Mismatch Issue
			Karthik	
*/

/* Sample Procedure Call

exec usp_Update_Matched_Invoices
 */

CREATE PROCEDURE [usp_Update_Matched_Invoices_999]
	@Gstin varchar(15)
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @ActionType varchar(15)
	Declare	@Activity varchar(2)

	Declare @Delimiter char(1)

	Declare @TotalRows int 

	Set @ActionType = 'B2B'
	Set @Activity = 'A'

	DECLARE @TBL_Values TABLE 
	(
		Slno int,
		RefId int
	)

	Select @ActionType = Ltrim(Rtrim(IsNull(@ActionType,'')))
	Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	Select @Activity = Ltrim(Rtrim(IsNull(@Activity,'')))
	Select @Delimiter = ','



	Insert Into @TBL_Values
	(	Slno ,
		RefId 
	)
	Select  Row_Number() OVER(Order by invid ASC) as slno,
			invid
	From Tbl_Invids_Shes_Kar
	where gstin = @Gstin 

	Declare @AdjAmt decimal(18,2)

	Select @AdjAmt = Convert(dec(18,2),IsNull(ReconValueAdjust,0)) 
	From TBL_Settings_Reconciliation 
	Where gstinno = @Gstin
	And @Gstin <> 'ALL'

	if IsNull(@AdjAmt,0) <= 0
	Begin
		Set @AdjAmt = 1
	End


	if @ActionType = 'B2B'
	Begin

	
		Select t1.invid as invid2a,
		       t2.invid as invid2
		Into #TBL_InvIds
		From	TBL_GSTR2A_B2B_INV t1,
				TBL_GSTR2_B2B_INV t2,
				@TBL_Values t3
		Where t2.invid = t3.refid
		And t1.inum = t2.inum
		And t1.idt = t2.idt
		And t1.pos = t2.pos
		And t1.inv_typ = t2.inv_typ
		And Not ( (t1.val < (t2.val - @AdjAmt))
				or
				(t1.val > (t2.val + @AdjAmt))
			)

		if Exists(Select 1 From #TBL_InvIds)
		Begin

			Select	t1.invid2 as invid2,
					space(50) as itmsid2,
					t1.invid2a as invid2a,
					t3.num as num,
					t4.rt as rt,
					t4.txval as txval,
					t4.iamt as iamt,
					t4.camt as camt,
					t4.samt as samt,
					t4.csamt as csamt
			Into #TBL_GSTR2A_B2B_INV_ITMS_DET
			From #TBL_InvIds t1,
				 TBL_GSTR2A_B2B_INV t2,
				 TBL_GSTR2A_B2B_INV_ITMS t3,
				 TBL_GSTR2A_B2B_INV_ITMS_DET t4
			Where t2.invid = t1.invid2a
			And t3.invid = t2.invid
			And t4.itmsid = t3.itmsid

			Update #TBL_GSTR2A_B2B_INV_ITMS_DET 
			SET #TBL_GSTR2A_B2B_INV_ITMS_DET.itmsid2 = t2.itmsid
			FROM #TBL_GSTR2A_B2B_INV_ITMS_DET  t1,
				TBL_GSTR2_B2B_INV_ITMS t2, 
				TBL_GSTR2_B2B_INV_ITMS_DET t3 
			WHERE t2.invid= t1.invid2 
			And t3.itmsid = t2.itmsid
			And t3.rt = t1.rt
	
			if Exists(Select 1 From #TBL_GSTR2A_B2B_INV_ITMS_DET)
			Begin

				Update TBL_GSTR2_B2B_INV_ITMS
				Set	num = t1.num
				From #TBL_GSTR2A_B2B_INV_ITMS_DET t1,
			 		 TBL_GSTR2_B2B_INV t2,
					 TBL_GSTR2_B2B_INV_ITMS t3
				Where  t2.invid = t1.invid2	
				And t3.invid = t2.invid
				And t3.itmsid = t1.itmsid2
			
				Update TBL_GSTR2_B2B_INV_ITMS_ITC
				Set	tx_i = t1.iamt,
					tx_c = t1.camt,
					tx_s = t1.samt,
					tx_cs = t1.csamt
				From #TBL_GSTR2A_B2B_INV_ITMS_DET t1,
					 TBL_GSTR2_B2B_INV t2,
					 TBL_GSTR2_B2B_INV_ITMS t3,
					 TBL_GSTR2_B2B_INV_ITMS_ITC t4
				Where  t2.invid = t1.invid2	
				And t3.invid = t2.invid
				And t4.itmsid = t3.itmsid
				And t4.itmsid = t1.itmsid2
				And IsNull(t4.elg,'') Not In ('no','cp')


				Select @TotalRows  = @@rowcount

			End
			
		End

	End




End