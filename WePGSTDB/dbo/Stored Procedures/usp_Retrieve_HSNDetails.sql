
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the Flat Summary Dashboard
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_Pie_Summary_Dashboard 12,26


 */
 
CREATE PROCEDURE [usp_Retrieve_HSNDetails]  
	@hsnSearchText varchar(max)
	--@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

-- + ' - '+ convert(varchar(5), rate)

select hsnid, (hsncode +' - '+hsndescription) as 'HSNColl' into #temp1 from TBL_HSN_MASTER 

select HSNCode,Hsndescription as 'Item Description',rate from tbl_hsn_Master where hsnid in (select hsnid from #temp1 where HSNColl like '%'+@hsnSearchText+'%')

End