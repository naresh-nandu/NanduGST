
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Split the GSTR Record Data
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 		Initial Version
*/

/* Sample Function Call


 */

CREATE FUNCTION [udf_SplitGSTRRecordData](
 @Text      varchar(8000)
,@Column    tinyint
,@Separator char(1)
)
Returns varchar(8000)
as
Begin
       Declare @StartPos  int = 1
       Declare @EndPos    int = CHARINDEX(@Separator, @Text, @StartPos)

       WHILE (@COLUMN >1 AND @EndPos> 0)
       Begin
             SET @StartPos = @EndPos + 1
             SET @EndPos = CHARINDEX(@Separator, @Text, @StartPos)
             SET @Column = @Column - 1
       End 

       IF @Column > 1  SET @StartPos = LEN(@Text) + 1
       IF @EndPos = 0 SET @EndPos = LEN(@Text) + 1 

       Return SUBSTRING (@Text, @StartPos, @EndPos - @StartPos)
End