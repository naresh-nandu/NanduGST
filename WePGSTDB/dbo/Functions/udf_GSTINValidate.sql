
-- select [dbo].[udf_GSTINValidate]('34AABFA9A870CMT')

CREATE FUNCTION [udf_GSTINValidate](@GSTIN nvarchar(50) )
returns int
As
Begin

Declare @Retval int

	 if len(@GSTIN)=15
	 Begin
			IF(ISNUMERIC(left(@GSTIN,2)) = 1)
				Begin
					if( convert(int, left(@GSTIN,2)))<=35
						Begin
							--print 'State code numeric and valid'
								IF (SUBSTRING(@GSTIN, 3, 10) LIKE '%[^a-zA-Z0-9]%')
								--PRINT 'Invalid PAN Number'
								set @Retval=2
								ELSE
								--PRINT 'Valid PAN Number' -- This is expected result
								set @Retval=1
						End
						Else
						Begin
							--print 'State code numeric but not valid'
							set @Retval=3
						End
				End
				Else
				Begin
					--print 'State code Not numeric'
					set @Retval=4
				End	
	End
Else
	Begin
		--print 'Invalid GSTIN'
		set @Retval=5
	End


 return @Retval

end