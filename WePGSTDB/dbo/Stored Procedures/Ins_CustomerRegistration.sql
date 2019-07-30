-- =============================================
-- Author:		KK
-- Create date: 06-Apr-17
-- Description:	Procedure to insert the Customer details into Tbl_Customer Table
-- exec [Ins_CustomerRegistration] 'NewCustomer','Manager','New Customer','GST10001','kk5@gmail.com','9008906454','CJTPK10001',12,'04/01/2017','04/25/2017',0,0
-- =============================================
CREATE PROCEDURE [Ins_CustomerRegistration] 
	@Name    varchar(50),
	@Designation    varchar(50),
	@Company    varchar(50),
	@GSTINNo    varchar(50),
	@Email    varchar(50),
	@MobileNo    varchar(50),
	@PANNo    varchar(50),
	@Statecode    varchar(50),
	@ValidFrom    datetime,
	@ValidTo    datetime,
	@AadharNo	varchar(12),
	@Referenceno varchar(50),
	@Address varchar(255),
	@RetValue	int Output,
	@CustId		int output
	

AS
BEGIN
	
	if not exists(select CustId from TBL_Customer where (GSTINNo=UPPER(@GSTINNo) or PANNO = UPPER(@PANNo) or MobileNo=@MobileNo or Email=@Email))
	begin

		if not exists(select 1 from Userlist where  MobileNo=@MobileNo or Email=@Email)
		Begin

		insert into TBL_Customer (Name,
							Designation,
							Company,
							GSTINNo,
							Email,
							MobileNo,
							PANNo,
							Statecode,
							ValidFrom,
							ValidTo,
							StatusCode,
							CreatedDate,
							RowStatus,
							AadharNo,
							[Address]
							) values(
							@Name,
							@Designation,
							@Company,
							UPPER(@GSTINNo),
							@Email,
							@MobileNo,
							@PANNo,
							@Statecode,
							@ValidFrom,
							@ValidTo,
							1,
							getdate(),
							1,
							@AadharNo,
							@Address
							)
			select @CustId = SCOPE_IDENTITY()

			if ltrim(rtrim(isnull(@Referenceno,''))) <> ''
			Begin
				update TBL_Customer set Referenceno=@Referenceno where custid =@CustId
			End
			Else
			Begin
				set @Referenceno = 'WEP00'+ CAST(@CustId AS VARCHAR(10))
				--set @Referenceno = 'WEP00'+ convert(varchar,10,@CustId)
				update TBL_Customer set Referenceno=@Referenceno where custid =@CustId
			End
			set @RetValue=1 -- GSTIN No not exist so inserting the customer details
			print @RetValue	
		End
		Else
		Begin
			set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
			set @CustId=0
			print @RetValue
		End
	End
	else 
		Begin
			set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
			set @CustId=0
			print @RetValue
		End
	
	return @RetValue
	return @CustId
END