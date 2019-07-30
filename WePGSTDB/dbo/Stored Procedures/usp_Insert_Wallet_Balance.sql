/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert Customer Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
24/04/2018	Raja M		Initial Version

*/

/* Sample Procedure Call  

exec usp_Insert_Wallet_Balance 'raja.m@wepindia.com','8123925996','FILING',5,10,50

 */

CREATE PROCEDURE [dbo].[usp_Insert_Wallet_Balance]
	@CustEmail    varchar(100) = NULL,
	@MobileNo		varchar(15) = NULL,
	--@ProductType    varchar(100)= NULL,
	--@Qty    int = NULL,
	--@Value    decimal(18, 2) = NULL,
	@TotalValue    decimal(18, 2) = NULL,
	@RetValue	int = Null Out,
	@RetMessage		varchar(255)= NULL Out,
	@RetBalance		decimal(18, 2)= NULL Out
AS

BEGIN
	--Declare @w_trpid int, @w_qty int, @w_value decimal(18, 2), @w_totalvalue decimal(18, 2)
	Declare @w_trpid int, @w_totalvalue decimal(18, 2)
	--IF (ISNULL(@ProductType,'')<>'' and ISNULL(@Qty,0)<>0 and ISNULL(@Value,0)<>0 and ISNULL(@TotalValue,0)<>0)
	IF (ISNULL(@TotalValue,0) <> 0)
	BEGIN	
		IF exists(Select TOP 1 * from TBL_TRP_Customer where Email = @CustEmail and MobileNo = @MobileNo and rowstatus = 1)
		BEGIN
			Select @w_trpid = trpCustId from TBL_TRP_Customer where Email = @CustEmail and MobileNo = @MobileNo and rowstatus = 1
			--IF ((@Qty * @Value) = @TotalValue)
			--BEGIN
				IF not exists(SELECT TOP 1 * FROM TBL_WEP_WALLET WHERE CustEmail = @CustEmail and MobileNo = @MobileNo)-- and ProductType = UPPER(@ProductType))
				BEGIN
					Insert into TBL_WEP_WALLET(TRPId,
						CustEmail,
						MobileNo,
						--ProductType,
						--Qty,
						--Value,
						TotalValue,
						CreatedDate)
					values(@w_trpid,
						@CustEmail,
						@MobileNo,
						--UPPER(@ProductType),
						--@Qty,
						--@Value,
						@TotalValue,
						DATEADD (mi, 330, GETDATE()))	

					-- INSERT WALLET TRANSACTIONS STARTS
					INSERT into TBL_WEP_WALLET_TRANSACTIONS(TRPId, 
						CustEmail, 
						MobileNo, 
						--ProductType, 
						TransactionType, 
						Amount, 
						CreatedDate)
					Values(@w_trpid, 
						@CustEmail,
						@MobileNo,
						--UPPER(@ProductType),
						'CREDIT',
						@TotalValue,
						DATEADD (mi, 330, GETDATE()))
					-- INSERT WALLET TRANSACTIONS STARTS

					set @RetValue = 1
					set @RetMessage = 'Wallet Balance is Updated'
					set @RetBalance = (select TOP(1) TotalValue from TBL_WEP_WALLET WHERE CustEmail = @CustEmail and MobileNo = @MobileNo)-- and ProductType = UPPER(@ProductType))
				END
				ELSE
				BEGIN
					SELECT TOP(1) --@w_qty = Qty, @w_value = Value, 
					@w_totalvalue = TotalValue FROM TBL_WEP_WALLET 
						WHERE CustEmail = @CustEmail and MobileNo = @MobileNo --and ProductType = @ProductType

					Update TBL_WEP_WALLET SET --Qty = @Qty + @w_qty, 
						--Value = @Value, 
						TotalValue = @TotalValue + @w_totalvalue
					 WHERE CustEmail = @CustEmail and MobileNo = @MobileNo --and ProductType = @ProductType

					-- INSERT WALLET TRANSACTIONS STARTS
					INSERT into TBL_WEP_WALLET_TRANSACTIONS(TRPId, 
						CustEmail, 
						MobileNo, 
						--ProductType, 
						TransactionType, 
						Amount, 
						CreatedDate)
					Values(@w_trpid, 
						@CustEmail,
						@MobileNo,
						--UPPER(@ProductType),
						'CREDIT',
						@TotalValue,
						DATEADD (mi, 330, GETDATE()))
					-- INSERT WALLET TRANSACTIONS STARTS

					set @RetValue = 1
					set @RetMessage = 'Wallet Balance is Updated'
					set @RetBalance = (select TOP(1) TotalValue from TBL_WEP_WALLET WHERE CustEmail = @CustEmail and MobileNo = @MobileNo)-- and ProductType = UPPER(@ProductType))
				END
			--END
			--ELSE
			--BEGIN
			--	set @RetValue = 0
			--	set @RetMessage = 'Current TotalValue is not correct. So wallet balance is not updated'
			--	IF EXISTS (select TOP(1) TotalValue from TBL_WEP_WALLET WHERE CustEmail = @CustEmail and MobileNo = @MobileNo and ProductType = UPPER(@ProductType))
			--	BEGIN
			--		set @RetBalance = (select TOP(1) TotalValue from TBL_WEP_WALLET WHERE CustEmail = @CustEmail and MobileNo = @MobileNo and ProductType = UPPER(@ProductType))
			--	END
			--	ELSE
			--	BEGIN
			--		set @RetBalance = 0
			--	END
			--END
		END
		ELSE
		BEGIN
			set @RetValue = 0
			set @RetMessage = 'Customer is not registered...'
			set @RetBalance = 0
		END
	END	
	ELSE
	BEGIN
		set @RetValue = 0
		set @RetMessage = 'All Parameters are mandatory'
		set @RetBalance = 0
	END
	Return 0

END