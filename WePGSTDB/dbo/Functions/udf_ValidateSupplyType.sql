
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Supply Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/26/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateSupplyType() 

 */

CREATE FUNCTION [udf_ValidateSupplyType](
 @SupplyType      varchar(50),
 @Gstin varchar(50),
 @Pos varchar(50)

)
Returns int
as
Begin
       Declare @RetValue int,
			   @SupplierStateCode tinyint,
			   @RecipientStateCode tinyint,
			   @InterStateSupply bit
	
	   Select @SupplyType = Ltrim(Rtrim(IsNull(@SupplyType,'')))
       Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	   Select @Pos = Ltrim(Rtrim(IsNull(@Pos,'')))

	   Select @RetValue = 1
	
	   if(@SupplyType not in ('INTER','INTRA'))
	   Begin
			Select @RetValue = -1   -- Invalid Supply Type
			return @RetValue
	   End

	   	if @Gstin <> ''
		Begin
			Select @SupplierStateCode = Convert(int,Substring(@Gstin,1,2))
		End
		if @Pos <> ''
		Begin
			Select @RecipientStateCode = Convert(int,@Pos)
		End
		if @SupplierStateCode != @RecipientStateCode
			Select @InterStateSupply = 1
		else
			Select @InterStateSupply = 0

		If @InterStateSupply = 1 and @SupplyType <> 'INTER'
		Begin
			Select @RetValue = -1   -- Invalid Supply Type
			return @RetValue
		End

		If @InterStateSupply = 0 and @SupplyType <> 'INTRA'
		Begin
			Select @RetValue = -1   -- Invalid Supply Type
			return @RetValue
		End

		return @RetValue
End