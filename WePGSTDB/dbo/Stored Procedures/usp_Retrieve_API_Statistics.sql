

-- =============================================
-- Author:		KK
-- Create date: 03-08-2018
-- Description:	To Fetch the GSTIN wise API count for give start and end date

-- exec [usp_Retrieve_API_Statistics] '03-01-2017','03-20-2018','33GSPTN0801G1ZM'

-- =============================================
CREATE PROCEDURE [usp_Retrieve_API_Statistics] 
@FromDate varchar(20),
@ToDate varchar(20),
@Gstin nvarchar(max)
	
AS
BEGIN

	   Declare @Delimiter char(1)
       DECLARE @TBL_Values TABLE
       (
	   Gstin varchar(15)
	   )
       Select @Delimiter = ','
       Insert Into @TBL_Values
       (Gstin)
       Select Value From string_split( @Gstin,@Delimiter)


	SET NOCOUNT ON;
		SELECT GSTINNO,FORMAT(Createddate, 'MM-yyyy') as [Period],APIName, COUNT(*) as [NoOfAPICalls]
				FROM TBL_ASP_API_Transactions 
				Where 
				--environment = 'production' AND 
				Createddate >= convert(datetime,@FromDate) and  Createddate<= convert(datetime,@ToDate) and 
				GSTINNO IN (select Gstin from  @TBL_Values) 
				GROUP BY GSTINNO,FORMAT(Createddate, 'MM-yyyy'),APIName

END