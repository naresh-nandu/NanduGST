/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Import raw .txt file data bulk copy to TBL_METRO_TRADE_OUTWARD Table
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
01/13/2018	Karthik		Initial Version


*/

/* Sample Procedure Call  

exec USP_BulkCopyMetroData_Trade

 */

Create PROCEDURE [USP_BulkCopyMetroData_Trade]
	@FileName nvarchar(MAX)
	
AS
BEGIN
	Declare @Recordcount int
	Declare @ErrorFileName nvarchar(max)
	SELECT @ErrorFileName = REPLACE(UPPER(@FileName),'.TXT','_ErrorFile.TXT')
	
	DECLARE @sql NVARCHAR(4000) = 'BULK INSERT View_TBL_METRO_TRADE_OUTWARD FROM ''' + @FileName + ''' WITH (FIRSTROW=2, MAXERRORS = 5000, FIELDTERMINATOR =''\t'', ROWTERMINATOR = ''0x0a'',ERRORFILE = '''+ @ErrorFileName +''')';
	EXEC(@sql);
	set @Recordcount = @@ROWCOUNT
	--insert into TBL_METRO_DATA_IMPORTED_Status(FilePath,NoOfRecords)values(@FileName,@Recordcount)

	
END