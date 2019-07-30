
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Place Of Supply
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepindia.com

Date		Who			Decription 
06/07/2017	Seshadri 		Initial Version
09/12/2017	Seshadri	Corrected the state code for Other Territory
*/

/* Sample Function Call

select dbo.udf_ValidatePlaceOfSupply('33')

 */

CREATE FUNCTION [udf_ValidatePlaceOfSupply](
@PlaceOfSupply	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@StateCode tinyint
		
	Select @PlaceOfSupply = Ltrim(Rtrim(IsNull(@PlaceOfSupply,'')))
	Select @RetValue = 1

	if @PlaceOfSupply = '' or @PlaceOfSupply = '0'
		return @RetValue

	if len(@PlaceOfSupply) != 2
	Begin
		Select @RetValue = -1  -- Length of Place of Supply is not equal to 2
		return @RetValue
	End

	if IsNumeric(@PlaceOfSupply) <> 1
	Begin
		Select @RetValue = -2  -- Place of Supply is not numeric
		return @RetValue
	End
	else
	Begin
		Select @StateCode = Convert(tinyint,@PlaceOfSupply)
	End

	if (@StateCode <= 0 or @StateCode  > 37) and (@StateCode <> 97)
	Begin
		Select @RetValue = -3  -- Place of Supply is not Valid
		return @RetValue
	End

								
	return @RetValue

End