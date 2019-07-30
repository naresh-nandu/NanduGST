

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate EWB Doc Date
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
24/05/2018	Karthik 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateNoteDate('22/05/2017','052017')

 */

CREATE FUNCTION [dbo].[udf_Validate_EWB_DocDate](
@DocDate	 varchar(50),
@TransporterDate varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@DcDate Date,
			@TransDate Date
		

	Select @DocDate = Ltrim(Rtrim(IsNull(@DocDate,'')))
	Select @RetValue = 1

	if len(@DocDate) < 8 or len(@DocDate) > 10
	Begin
		Select @RetValue = -1  -- Length of the Note Date should be between 8 and 10 
		return @RetValue
	End

	Select @DcDate = Try_Convert(date,@DocDate,103)
	if @DcDate Is null
	Begin
		Select @RetValue = -2  -- Invalid Note Date Format
		return @RetValue
	End
	
	Select @TransDate = Try_Convert(date,@TransporterDate,103)

	if @TransDate < @DcDate
	Begin
		Select @RetValue = -4  -- Transporter Date should not be less than Doc date
		return @RetValue
	End	


	return @RetValue

End

GO