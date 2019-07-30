
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the GSTR3B Records in the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/29/2017	Seshadri 	Initial Version
12/12/2017	Seshadri	Fixed Integration Testing defects
12/13/2017	Seshadri	Fixed ITC Testing defects

*/

/* Sample Procedure Call

exec usp_Update_GSTR3B_SA


 */
 
CREATE PROCEDURE [usp_Update_GSTR3B_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@SupplyNum int,
	@DetId int,
	@Pos varchar(2),
	@Txval decimal(18,2),
	@Iamt decimal(18,2),
	@Camt decimal(18,2),
	@Samt decimal(18,2),
	@Csamt decimal(18,2),
	@InterStateSupplies decimal(18,2),
	@IntraStateSupplies decimal(18,2)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @GstinId int,@Gstr3bId int 
	Declare @Sup_DetId int,@Inter_SupId int
	Declare @Inward_SupId int
	Declare @Itc_ElgId int
	Declare @Intr_LtFeeId int

	Select @GstinId = gstinid from TBL_Cust_GSTIN where gstinno = @GstinNo

	if @SupplyNum = 1
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_sup_det_osup_det
			Set txval = @Txval,
				iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where osup_detid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_sup_det Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Sup_DetId = sup_detid 
				From TBL_GSTR3B_sup_det
				Where gstr3bid = @Gstr3bId

				if(@Sup_DetId > 0)
				Begin
					Insert TBL_GSTR3B_sup_det_osup_det
					(sup_detid,	txval,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Sup_DetId,@Txval,@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End
	End
	else if @SupplyNum = 2
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_sup_det_osup_zero
			Set txval = @Txval,
				iamt = @Iamt,
				csamt = @Csamt
			Where osup_zeroid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_sup_det Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Sup_DetId = sup_detid 
				From TBL_GSTR3B_sup_det
				Where gstr3bid = @Gstr3bId

				if(@Sup_DetId > 0)
				Begin
					Insert TBL_GSTR3B_sup_det_osup_zero
					(sup_detid,	txval,iamt,csamt,gstinid,gstr3bid)
					Select @Sup_DetId,@Txval,@Iamt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 3
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_sup_det_osup_nil_exmp
			Set txval = @Txval
			Where osup_nil_exmpid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_sup_det Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Sup_DetId = sup_detid 
				From TBL_GSTR3B_sup_det
				Where gstr3bid = @Gstr3bId

				if(@Sup_DetId > 0)
				Begin
					Insert TBL_GSTR3B_sup_det_osup_nil_exmp
					(sup_detid,	txval,gstinid,gstr3bid)
					Select @Sup_DetId,@Txval,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 4
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_sup_det_isup_rev
			Set txval = @Txval,
				iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where isup_rev_id = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_sup_det Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Sup_DetId = sup_detid 
				From TBL_GSTR3B_sup_det
				Where gstr3bid = @Gstr3bId

				if(@Sup_DetId > 0)
				Begin
					Insert TBL_GSTR3B_sup_det_isup_rev
					(sup_detid,	txval,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Sup_DetId,@Txval,@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 5
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_sup_det_osup_nongst
			Set txval = @Txval
			Where osup_nongstid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_sup_det Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Sup_DetId = sup_detid 
				From TBL_GSTR3B_sup_det
				Where gstr3bid = @Gstr3bId

				if(@Sup_DetId > 0)
				Begin
					Insert TBL_GSTR3B_sup_det_osup_nongst
					(sup_detid,	txval,gstinid,gstr3bid)
					Select @Sup_DetId,@Txval,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 6
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_inter_sup_unreg_det
			Set	pos = @Pos, 
				txval = @Txval,
				iamt = @Iamt
			Where unreg_detid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_inter_sup Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_inter_sup(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Inter_SupId = inter_supid 
				From TBL_GSTR3B_inter_sup
				Where gstr3bid = @Gstr3bId

				if(@Inter_SupId > 0)
				Begin
					Insert TBL_GSTR3B_inter_sup_unreg_det
					(inter_supid,pos,txval,iamt,gstinid,gstr3bid)
					Select @Inter_SupId,@Pos,@Txval,@Iamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 7
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_inter_sup_comp_det
			Set	pos = @Pos, 
				txval = @Txval,
				iamt = @Iamt
			Where comp_detid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_inter_sup Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_inter_sup(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Inter_SupId = inter_supid 
				From TBL_GSTR3B_inter_sup
				Where gstr3bid = @Gstr3bId

				if(@Inter_SupId > 0)
				Begin
					Insert TBL_GSTR3B_inter_sup_comp_det
					(inter_supid,pos,txval,iamt,gstinid,gstr3bid)
					Select @Inter_SupId,@Pos,@Txval,@Iamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 8
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_inter_sup_uin_det
			Set	pos = @Pos, 
				txval = @Txval,
				iamt = @Iamt
			Where uin_detid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_inter_sup Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_inter_sup(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Inter_SupId = inter_supid 
				From TBL_GSTR3B_inter_sup
				Where gstr3bid = @Gstr3bId

				if(@Inter_SupId > 0)
				Begin
					Insert TBL_GSTR3B_inter_sup_uin_det
					(inter_supid,pos,txval,iamt,gstinid,gstr3bid)
					Select @Inter_SupId,@Pos,@Txval,@Iamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
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

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_avl
			Set iamt = @Iamt,
				csamt = @Csamt
			Where itc_avlid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_avl
					(itc_elgid,ty,iamt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'IMPG',@Iamt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 10
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_avl
			Set iamt = @Iamt,
				csamt = @Csamt
			Where itc_avlid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_avl
					(itc_elgid,ty,iamt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'IMPS',@Iamt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 11
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_avl
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_avlid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_avl
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'ISRC',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 12
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_avl
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_avlid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_avl
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'ISD',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 13
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_avl
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_avlid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_avl
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'OTH',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 14
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_rev
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_revid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_rev
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'RUL',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 15
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_rev
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_revid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_rev
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'OTH',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 16
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_net
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_netid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_net
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 17
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_inelg
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_inelgid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_inelg
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'RUL',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

	End
	else if @SupplyNum = 18
	Begin

		if (@DetId > 0)
		Begin
			Update TBL_GSTR3B_itc_elg_itc_inelg 
			Set iamt = @Iamt,
				camt = @Camt,
				samt = @Samt,
				csamt = @Csamt
			Where itc_inelgid = @DetId
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
				if Not Exists(SELECT 1 FROM TBL_GSTR3B_itc_elg Where gstr3bid = @Gstr3bId)
				Begin
					Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
					Select @Gstr3bId,@GstinId
				End

				Select @Itc_ElgId = itc_elgid 
				From TBL_GSTR3B_itc_elg
				Where gstr3bid = @Gstr3bId

				if(@Itc_ElgId > 0)
				Begin
					Insert TBL_GSTR3B_itc_elg_itc_inelg
					(itc_elgid,ty,iamt,camt,samt,csamt,gstinid,gstr3bid)
					Select @Itc_ElgId,'OTH',@Iamt,@Camt,@Samt,@Csamt,@GstinId, @Gstr3bId
				End
			End
			 
		End

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

	End

	Return 0

End