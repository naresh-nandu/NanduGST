/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Vehicle NO
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
28/05/2018	Karthik 		Initial Version
01/06/2018  Karthik         Included Additionalformats of vehicle no.
*/

/* Sample Function Call
-- Formats
AB12ABC1234	char(11)
AB12AB1234	char(10)
AB123A1234	char(10)
AB12A1234	char(9)
AB121234	char(8)
ABC1234	char(7)
select dbo.udf_ValidateVehicleNo('123ABCD')

 */  

CREATE FUNCTION [dbo].[udf_ValidateVehicleNo](
@VehicleNo	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int
	Select @VehicleNo = Ltrim(Rtrim(IsNull(@VehicleNo,'')))
	Select @RetValue = 1

	if len(@VehicleNo) = 11
	Begin
		Select @VehicleNo = Substring(@VehicleNo,1,11)
		if(@VehicleNo Not Like '[A-Z][A-Z][0-9][0-9][A-Z][A-Z][A-Z][0-9][0-9][0-9][0-9]') -- format AB12ABC1234	char(11)
		Begin
			Select @RetValue = -11  -- Invalid Vehicle Number
			return @RetValue
		End
	End	
	Else if len(@VehicleNo) = 10
	Begin
		Select @VehicleNo = Substring(@VehicleNo,1,10)
		if(@VehicleNo Not Like '[A-Z][A-Z][0-9][0-9][A-Z][A-Z][0-9][0-9][0-9][0-9]' and @VehicleNo Not Like '[A-Z][A-Z][0-9][0-9][0-9][A-Z][0-9][0-9][0-9][0-9]') -- format AB12AB1234	char(10) or AB123A1234	char(10)
		Begin
			Select @RetValue = -10  -- Invalid Vehicle Number	
			return @RetValue	
		End
	End	
	Else if len(@VehicleNo) = 9
	Begin
		Select @VehicleNo = Substring(@VehicleNo,1,9)
		if(@VehicleNo Not Like '[A-Z][A-Z][0-9][0-9][A-Z][0-9][0-9][0-9][0-9]')-- Format AB12A1234	char(9)
		Begin
			Select @RetValue = -9  -- Invalid Vehicle Number
			return @RetValue
		End
	End	
	Else if len(@VehicleNo) = 8
	Begin
		Select @VehicleNo = Substring(@VehicleNo,1,8)
		if(@VehicleNo Not Like '[A-Z][A-Z][0-9][0-9][0-9][0-9][0-9][0-9]')-- Format AB121234	char(8)
		Begin
			Select @RetValue = -8  -- Invalid Vehicle Number
			return @RetValue
		End
	End	
	Else if len(@VehicleNo) = 7
	Begin
		Select @VehicleNo = Substring(@VehicleNo,1,7)
		if(@VehicleNo Not Like '[A-Z][A-Z][A-Z][0-9][0-9][0-9][0-9]')-- Format ABC1234	char(7)
		Begin
			Select @RetValue = -7  -- Invalid Vehicle Number
			return @RetValue
		End
	End	
	Else If len(@VehicleNo) not in (11,10,9,8,7)
	Begin
		Select @RetValue = -1  -- Invalid Vehicle Number
		return @RetValue
	End
	return @RetValue
End