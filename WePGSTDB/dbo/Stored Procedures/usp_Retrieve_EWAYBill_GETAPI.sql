-- =============================================
-- Author:		Naresh N
-- Create date: 30-March-18
-- Description:	Procedure to Retrieve eway bill get api data
-- exec usp_Retrieve_EWAYBill_GETAPI 1,'','','CAN','',''
-- =============================================
CREATE PROCEDURE [dbo].[usp_Retrieve_EWAYBill_GETAPI]
@CustomerId int,
@toGSTIN varchar(15) NuLL,
@EWAYBillDate varchar(30) NuLL,
@Flag varchar(3),
@EWAYBillNo varchar(12) Null,
@userGSTIN varchar(15) Null
	
AS
if @Flag = 'O'
BEGIN
	select * from TBL_EWB_GENERATION_OTHERPARTY where CustId = @CustomerId and ewayBillDate like '%' + @EWAYBillDate + '%' and toGstin = @toGSTIN and flag='O'
END
else if @Flag = 'E'
BEGIN
	select * from TBL_EWB_GENERATION_GET where CustId = @CustomerId and ewayBillNo = @EWAYBillNo
END
else if @Flag = 'C'
BEGIN
	select * from TBL_EWB_GEN_CONSOLIDATED_GET where CustId = @CustomerId and cEwbNo = @EWAYBillNo 
END

else if @Flag = 'T'
BEGIN
	select * from TBL_EWB_Transporter where CustId = @CustomerId and ewbDate like '%' + @EWAYBillDate + '%' and flag = @Flag and userGstin = @userGSTIN 
END

else if @Flag = 'G'
BEGIN
	select * from TBL_EWB_Transporter where CustId = @CustomerId and ewbDate like '%' + @EWAYBillDate + '%' and flag = @Flag and genGstin = @userGSTIN 
END

else if @Flag = 'CAN'
BEGIN
	select * from TBL_EWB_GENERATION where flag in('E','1') and CustId = @CustomerId
END