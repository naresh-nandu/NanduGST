
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate ITC Eligibility
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepindia.com

Date		Who			Decription 
10/05/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateItcEligibility('IP')

 */

CREATE FUNCTION [udf_ValidateItcEligibility](
@Itc_Elg varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int
		
	Select @Itc_Elg = Ltrim(Rtrim(IsNull(@Itc_Elg,'')))
	Select @RetValue = 1

	if @Itc_Elg = '' 
		return @RetValue

	if(@Itc_Elg  not in ('IP','CP','IS','NO'))
	Begin
		Select @RetValue = -1   -- Invalid ITC Eligibility
		return @RetValue
	End

	return @RetValue

End