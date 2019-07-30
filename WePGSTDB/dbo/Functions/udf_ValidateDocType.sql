
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Doc Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 		Initial Version
08/10/2017	Seshadri	Reintroduced HSN Doc Type 
10/27/2017	Seshadri	Modified to handle extra document types because of template defect
03/06/2018	Seshadri	Modified the code to support Amendments

*/

/* Sample Function Call

 Select dbo.udf_ValidateDocType('B2B') 

 */

CREATE FUNCTION [udf_ValidateDocType](
 @DocType      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @DocType = Ltrim(Rtrim(IsNull(@DocType,'')))
	   Select @RetValue = 1
	
	   if(@DocType not in ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR','HSN','NIL','TXP','AT','DOCISSUE',
							'B2BUR','IMPG','IMPS','TXI','ITCRVSL',
							'TXPD','TAXL','HSN SUMMARY','ITC REV',
							'B2BA','B2CLA','B2CSA','EXPA','CDNRA','CDNURA',
							'TXPA','ATA'))
	   Begin
			Select @RetValue = -1   -- Invalid Doc Type
			return @RetValue
	   End

		return @RetValue
End