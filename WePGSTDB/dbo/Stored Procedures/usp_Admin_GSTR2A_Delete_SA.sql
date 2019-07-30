
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete GSTR2A data from SA
				
Written by  : Karthik

Date		Who			Decription 
11/10/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Admin_GSTR2A_Delete_SA '27AAFCD5562Q1Z0','072017' 
*/
CREATE PROCEDURE [usp_Admin_GSTR2A_Delete_SA]  
@Gstin varchar(15), 
@Fp varchar(10)  
  
AS  
BEGIN  
		
		Select a.* Into #GstrIds
		From
		(Select gstr2aid as gstr2aid from TBL_GSTR2A where gstin = @Gstin and fp = @Fp
		) a

		-- Deletion of SA Tables
		if exists(select 1 from #GstrIds)
		Begin
			-- B2B Deltion
				
				Delete from TBL_GSTR2a_B2B_INV_ITMS_DET where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_B2B_INV_ITMS where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_B2B_INV where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_B2B where gstr2aid in(Select gstr2aid From #GstrIds)

		
			-- CDNR Deltion
				Delete from TBL_GSTR2a_CDNR_NT_ITMS_DET where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_CDNR_NT_ITMS where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_CDNR_NT where gstr2aid in(Select gstr2aid From #GstrIds)
				Delete from TBL_GSTR2a_CDNR where gstr2aid in(Select gstr2aid From #GstrIds)

			
		End

									

 END