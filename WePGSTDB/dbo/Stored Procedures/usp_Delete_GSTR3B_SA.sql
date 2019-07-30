
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR3B Records in the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/11/2017	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_GSTR3B_SA


 */
 
CREATE PROCEDURE [usp_Delete_GSTR3B_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@SupplyNum int,
	@DetId int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @GstinId int,@Gstr3bId int 
	Declare @Sup_DetId int,@Inter_SupId int
	Declare @Inward_SupId int
	Declare @Intr_LtFeeId int

	Select @GstinId = gstinid from TBL_Cust_GSTIN where gstinno = @GstinNo

	if @SupplyNum = 1
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_sup_det_osup_det
			Where osup_detid = @DetId
		End
	End
	else if @SupplyNum = 2
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_sup_det_osup_zero
			Where osup_zeroid = @DetId
		End
	
	End
	else if @SupplyNum = 3
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_sup_det_osup_nil_exmp
			Where osup_nil_exmpid = @DetId
		End

	End
	else if @SupplyNum = 4
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_sup_det_isup_rev
			Where isup_rev_id = @DetId
		End

	End
	else if @SupplyNum = 5
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_sup_det_osup_nongst
			Where osup_nongstid = @DetId
		End

	End
	else if @SupplyNum = 6
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_inter_sup_unreg_det
			Where unreg_detid = @DetId
		End

	End
	else if @SupplyNum = 7
	Begin

		if (@DetId > 0)
		Begin
			Delete TBL_GSTR3B_inter_sup_comp_det
			Where comp_detid = @DetId
		End

	End
	else if @SupplyNum = 8
	Begin

		if (@DetId > 0)
		Begin
			Delete From TBL_GSTR3B_inter_sup_uin_det
			Where uin_detid = @DetId
		End

	End
	/*
	else if @SupplyNum = 19
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_inward_sup_isup_details
			Set	inter = @InterStateSupplies, 
				intra = @IntraStateSupplies
			Where isup_detid = @DetId
		End
		else
		Begin

			if Not Exists (Select 1 From TBL_GSTR3B  Where gstin = @GstinNo and ret_period = @Fp)
			Begin
				Insert TBL_GSTR3B (gstin,gstinId,ret_period)
				Select @GstinNo,@GstinId,@Fp
			End
			
			Select @Gstr3bId = gstr3bid 
			From TBL_GSTR3B
			Where gstin = @GstinNo
			And ret_period = @Fp

			if (@Gstr3bId > 0)
			Begin
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_inward_sup Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_inward_sup(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Inward_SupId  = inward_supid 
				From TBL_GSTR3B_inward_sup
				Where gstr3bid = @Gstr3bId

				if(@Inward_SupId > 0)
				Begin
					Insert TBL_GSTR3B_inward_sup_isup_details
					(inward_supid,ty,inter,intra,gstinid,gstr3bid)
					Select @Inward_SupId,'GST',@InterStateSupplies,@IntraStateSupplies,@GstinId, @Gstr3bId
				End
			End
			 
		End

		
	End
	else if @SupplyNum = 20
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_inward_sup_isup_details
			Set	inter = @InterStateSupplies, 
				intra = @IntraStateSupplies
			Where isup_detid = @DetId
		End
		else
		Begin

			if Not Exists (Select 1 From TBL_GSTR3B  Where gstin = @GstinNo and ret_period = @Fp)
			Begin
				Insert TBL_GSTR3B (gstin,gstinId,ret_period)
				Select @GstinNo,@GstinId,@Fp
			End
			
			Select @Gstr3bId = gstr3bid 
			From TBL_GSTR3B
			Where gstin = @GstinNo
			And ret_period = @Fp

			if (@Gstr3bId > 0)
			Begin
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_inward_sup Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_inward_sup(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Inward_SupId  = inward_supid 
				From TBL_GSTR3B_inward_sup
				Where gstr3bid = @Gstr3bId

				if(@Inward_SupId > 0)
				Begin
					Insert TBL_GSTR3B_inward_sup_isup_details
					(inward_supid,ty,inter,intra,gstinid,gstr3bid)
					Select @Inward_SupId,'NONGST',@InterStateSupplies,@IntraStateSupplies,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 9
	Begin
		Update TBL_GSTR3B_itc_elg_itc_avl
		Set iamt = @Iamt,
			csamt = @Csamt
		Where itc_avlid = @DetId
	End
	else if @SupplyNum = 10
	Begin
		Update TBL_GSTR3B_itc_elg_itc_avl
		Set iamt = @Iamt,
			csamt = @Csamt
		Where itc_avlid = @DetId
	End
	else if @SupplyNum = 11
	Begin
		Update TBL_GSTR3B_itc_elg_itc_avl
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_avlid = @DetId
	End
	else if @SupplyNum = 12
	Begin
		Update TBL_GSTR3B_itc_elg_itc_avl
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_avlid = @DetId
	End
	else if @SupplyNum = 13
	Begin
		Update TBL_GSTR3B_itc_elg_itc_avl
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_avlid = @DetId
	End
	else if @SupplyNum = 14
	Begin
		Update TBL_GSTR3B_itc_elg_itc_rev
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_revid = @DetId
	End
	else if @SupplyNum = 15
	Begin
		Update TBL_GSTR3B_itc_elg_itc_rev
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_revid = @DetId
	End
	else if @SupplyNum = 16
	Begin
		Update TBL_GSTR3B_itc_elg_itc_net
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_netid = @DetId
	End
	else if @SupplyNum = 17
	Begin
		Update TBL_GSTR3B_itc_elg_itc_inelg
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_inelgid = @DetId
	End
	else if @SupplyNum = 18
	Begin
		Update TBL_GSTR3B_itc_elg_itc_inelg 
		Set iamt = @Iamt,
			camt = @Camt,
			samt = @Samt,
			csamt = @Csamt
		Where itc_inelgid = @DetId
	End
	else if @SupplyNum = 21
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_intr_ltfee_intr_det 
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where intr_ltfeeid = @DetId
		End
		else
		Begin

			if Not Exists (Select 1 From TBL_GSTR3B  Where gstin = @GstinNo and ret_period = @Fp)
			Begin
				Insert TBL_GSTR3B (gstin,gstinId,ret_period)
				Select @GstinNo,@GstinId,@Fp
			End
			
			Select @Gstr3bId = gstr3bid 
			From TBL_GSTR3B
			Where gstin = @GstinNo
			And ret_period = @Fp

			if (@Gstr3bId > 0)
			Begin
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_intr_ltfee Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_intr_ltfee(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Intr_LtFeeId = intr_ltfeeid 
				From TBL_GSTR3B_intr_ltfee
				Where gstr3bid = @Gstr3bId

				if(@Intr_LtFeeId > 0)
				Begin
					Insert TBL_GSTR3B_intr_ltfee_intr_det 
					(intr_ltfeeid,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Intr_LtFeeId,@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End */

	Return 0

End