

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retreive DocIssue for GSTR1 Records from the corresponding external tables
				
Written by  : nareshn@wepindia.com 

Date		Who			Decription 
12/12/2017	Naresh 	Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_Outward_DocIssue_EXT '37AWJPN5651K1Z4','112017','Manual',71

 */
 
CREATE PROCEDURE [usp_Retrieve_Outward_DocIssue_EXT]
	@GSTIN varchar(15), 
	@FP varchar(50),
	@SourceType varchar(15), -- Manual 
	@CreatedBy int

-- /*mssx*/ With Encryption 
As 
Begin

	Begin

	select * from TBL_EXT_GSTR1_DOC where gstin=@GSTIN and fp=@FP and sourcetype=@SourceType and createdby=@CreatedBy and rowstatus != 2

	End
	
	Return 0

End