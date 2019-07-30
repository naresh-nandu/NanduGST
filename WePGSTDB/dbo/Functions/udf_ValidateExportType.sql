
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Export Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateExportType('WPAY') 

 */

CREATE FUNCTION [udf_ValidateExportType](
 @ExportType      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @ExportType = Ltrim(Rtrim(IsNull(@ExportType,'')))
	   Select @RetValue = 1
	
	   if(@ExportType not in ('WPAY','WOPAY'))
	   Begin
			Select @RetValue = -1   -- Invalid Export Type
			return @RetValue
	   End

		return @RetValue
End