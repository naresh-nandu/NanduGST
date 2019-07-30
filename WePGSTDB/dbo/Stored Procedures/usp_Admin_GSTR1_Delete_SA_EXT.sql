
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete GSTR1 data from SA & EXT
				
Written by  : Karthik

Date		Who			Decription 
11/10/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Admin_GSTR1_Delete_SA_EXT '27AAFCD5562Q1Z0','072017' 
*/
CREATE PROCEDURE [usp_Admin_GSTR1_Delete_SA_EXT]   
@Gstin varchar(15), 
@Fp varchar(10)  
  
AS  
BEGIN  
		Select a.* Into #GstrIds
		From
		(Select gstr1id as gstr1id from TBL_GSTR1 where gstin = @Gstin and fp = @Fp
		) a

		-- Deletion of SA Tables
		if exists(select 1 from #GstrIds)
		Begin
		-- Deletion Of B2B
			delete from TBL_GSTR1_B2B_INV_ITMS_DET where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2B_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2B_INV where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2B where gstr1id in(Select gstr1id From #GstrIds)
	   -- Deletion Of B2CL
			delete from TBL_GSTR1_B2CL_INV_ITMS_DET where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2CL_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2CL_INV where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_B2CL where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of B2CS
			delete from TBL_GSTR1_B2CS where gstr1id in(Select gstr1id From #GstrIds)
	    -- Deletion Of EXP
			delete from TBL_GSTR1_EXP_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_EXP_INV where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_EXP where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of CDNR
			delete from TBL_GSTR1_CDNR_NT_ITMS_DET where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_CDNR_NT_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_CDNR_NT where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_CDNR where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of CDNUR
			delete from TBL_GSTR1_CDNUR_ITMS_DET where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_CDNUR_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_CDNUR where gstr1id in(Select gstr1id From #GstrIds)
	    -- Deletion Of HSN
			delete from TBL_GSTR1_HSN_DATA where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_HSN where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of NIL
			delete from TBL_GSTR1_NIL_INV where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_NIL where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of TXP
			delete from TBL_GSTR1_TXP_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_TXP where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of AT
			delete from TBL_GSTR1_AT_ITMS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_AT where gstr1id in(Select gstr1id From #GstrIds)
		-- Deletion Of DOC ISSUE
			delete from TBL_GSTR1_DOCS where gstr1id in(Select gstr1id From #GstrIds)
			delete from TBL_GSTR1_DOC_ISSUE where gstr1id in(Select gstr1id From #GstrIds)

			delete from TBL_GSTR1 where gstr1id in(Select gstr1id From #GstrIds)
		End

		-- Deletion of EXT Tables
				delete From TBL_EXT_GSTR1_B2B_INV  where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_B2CL_INV where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_B2CS where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_EXP_INV where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_CDNR where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_CDNUR where gstin = @Gstin and fp = @Fp
				delete From Tbl_ext_gstr1_hsn  where gstin = @Gstin and fp = @Fp
				delete From TBL_EXT_GSTR1_NIL  where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_TXP   where gstin = @Gstin and fp = @Fp
				delete from TBL_EXT_GSTR1_AT   where gstin = @Gstin and fp = @Fp
				delete From TBL_EXT_GSTR1_Doc  where gstin = @Gstin and fp = @Fp

							

 END