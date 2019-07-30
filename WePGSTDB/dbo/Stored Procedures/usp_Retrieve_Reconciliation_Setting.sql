
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve Reconciliation GSTIN wise Setting details
				
Written by  : Karthik 

Date		Who			Decription 
07/31/2017	Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_Reconciliation_Setting 1
 */

CREATE PROCEDURE [usp_Retrieve_Reconciliation_Setting]
	@custid int
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

			select GSTINNO,'N' as 'Adjust_0','N' as 'Adjust_1','N'as 'Adjust_2','N' as'Adjust_5','N' as 'Adjust_10' into #temp1 
			from TBL_Settings_Reconciliation where custid=@custid and rowstatus=1 group by gstinno

			update #temp1 set Adjust_0 = 'Y' where #temp1.gstinno in (select gstinno from TBL_Settings_Reconciliation where reconvalueAdjust = 0 and custid=@custid)
			update #temp1 set Adjust_1 = 'Y' where #temp1.gstinno in (select gstinno from TBL_Settings_Reconciliation where reconvalueAdjust = 1 and custid=@custid)
			update #temp1 set Adjust_2 = 'Y' where #temp1.gstinno in (select gstinno from TBL_Settings_Reconciliation where reconvalueAdjust = 2 and custid=@custid)
			update #temp1 set Adjust_5 = 'Y' where #temp1.gstinno in (select gstinno from TBL_Settings_Reconciliation where reconvalueAdjust = 5 and custid=@custid)
			update #temp1 set Adjust_10= 'Y' where #temp1.gstinno in (select gstinno from TBL_Settings_Reconciliation where reconvalueAdjust = 10 and custid=@custid)
			select * from #temp1
	Return 0
End