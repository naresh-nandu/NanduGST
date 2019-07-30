
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete GSTR2 data from SA & EXT
				
Written by  : Karthik

Date		Who			Decription 
11/10/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Admin_GSTR2_Delete_SA_EXT '27AAFCD5562Q1Z0','072017' 
*/
CREATE PROCEDURE [usp_Admin_GSTR2_Delete_SA_EXT]   
@Gstin varchar(15), 
@Fp varchar(10)  
  
AS  
BEGIN  
		Select a.* Into #GstrIds
		From
		(Select gstr2id as gstr2id from TBL_GSTR2 where gstin = @Gstin and fp = @Fp
		) a

		-- Deletion of SA Tables
		if exists(select 1 from #GstrIds)
		Begin
			-- B2B Deltion
				Delete from TBL_GSTR2_B2B_INV_ITMS_ITC where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2B_INV_ITMS_DET where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2B_INV_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2B_INV where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2B where gstr2id in(Select gstr2id From #GstrIds)
			-- B2BUR Deltion
				Delete from TBL_GSTR2_B2BUR_INV_ITMS_ITC where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2BUR_INV_ITMS_DET where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2BUR_INV_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2BUR_INV where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_B2BUR where gstr2id in(Select gstr2id From #GstrIds)
			-- IMPG Deltion
				Delete from TBL_GSTR2_IMPG_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_IMPG where gstr2id in(Select gstr2id From #GstrIds)
			-- IMPS Deltion
				Delete from TBL_GSTR2_IMPS_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_IMPS where gstr2id in(Select gstr2id From #GstrIds)
			-- CDNR Deltion
				Delete from TBL_GSTR2_CDNR_NT_ITMS_ITC where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNR_NT_ITMS_DET where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNR_NT_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNR_NT where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNR where gstr2id in(Select gstr2id From #GstrIds)
			-- CDNUR Deltion
				Delete from TBL_GSTR2_CDNUR_ITMS_DET where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNUR_ITMS_ITC where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNUR_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_CDNUR where gstr2id in(Select gstr2id From #GstrIds)
			-- HSN SUM Deltion
				Delete from TBL_GSTR2_HSNSUM_DET where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_HSNSUM where gstr2id in(Select gstr2id From #GstrIds)
			-- NIL Deltion
				Delete from TBL_GSTR2_NIL_INTER where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_NIL_INTRA where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_NIL where gstr2id in(Select gstr2id From #GstrIds)
			-- TXPD Deltion
				Delete from TBL_GSTR2_TXPD_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_TXPD where gstr2id in(Select gstr2id From #GstrIds)
			-- TXI Deltion
				Delete from TBL_GSTR2_TXI_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_TXI where gstr2id in(Select gstr2id From #GstrIds)
			-- ITC RVSL Deltion
				Delete from TBL_GSTR2_ITCRVSL_ITMS where gstr2id in(Select gstr2id From #GstrIds)
				Delete from TBL_GSTR2_ITCRVSL where gstr2id in(Select gstr2id From #GstrIds)
		End

		-- Deletion of EXT Tables
				Delete from TBL_EXT_GSTR2_B2B_INV where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_B2BUR_INV where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_IMPG_INV where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_IMPS_INV where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_CDN where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_CDNUR where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_HSN where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_NIL where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_TXPD where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_TXI where gstin = @Gstin and fp = @Fp
				Delete from TBL_EXT_GSTR2_ITCRVSL where gstin = @Gstin and fp = @Fp

							

 END