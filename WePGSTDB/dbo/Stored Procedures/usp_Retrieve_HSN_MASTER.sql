
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the HSN List
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_HSN_MASTER 1


 */
 
CREATE PROCEDURE [usp_Retrieve_HSN_MASTER]  
	@CustId int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
			select ROW_NUMBER() OVER(ORDER BY hsnid desc ) AS 'S_No',hsnid, HSNCode as 'HSN Code', HSNDescription as 'Item Description', UnitPrice as 'Unit Price', Rate ,hsnType as 'HSN Type'
			From TBL_HSN_MASTER where rowstatus=1 and CustomerId =@CustId order by hsnid desc

	Return 0

End