
/*
(c) Copyright 2018 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the EWAYBill Dashboard PIE
				
Written by  : nareshn@wepindia.com 

Date		Who			 Decription 
05/04/2018	Naresh N	 Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_EWB_Dashboard_SUMMARY 'ALL','052018',1,1


 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_EWB_Dashboard_SUMMARY]
	@userGstin varchar(15),
	@FP varchar(6),  
	@CustId int, 
	@UserId int
	

-- /*mssx*/ With Encryption 
AS
Begin

    Set Nocount on
	--Drop table if exists #gstins
	--Declare @userGSTIN varchar(15)

   --Set @userGSTIN ='ALL'
   IF @userGSTIN='ALL'
   Begin
   Set @userGSTIN = '%'
   End

   Select CG.GSTINNo,
	       CG.GSTINId,
	       CG.PANNo,
		   space(250) as CompanyName, 
		   0 as 'Generate', 
		   0 as 'Received',
		   0 as 'Cancelled', 
		   0 as 'RejectedByMe', 
		   0 as 'RejectedByCounterParty' into #tempGSTIN
				          FROM   TBL_Cust_GSTIN CG 
				          where 
					      CG.Rowstatus =1
					      AND CG.custid= 1
						  AND CG.GSTINNo like @userGSTIN
						  AND (Ltrim(Rtrim(IsNull(EWBUserName,''))) <> ''  OR Ltrim(Rtrim(IsNull(EWBPassword,''))) <> '')

        Update #tempGSTIN 
		SET CompanyName=(select distinct CompanyName 
		                 from TBL_Cust_PAN where TBL_Cust_PAN.PANNo=#tempGSTIN.Panno 
						 AND TBL_Cust_PAN.custid=1 AND rowstatus=1)


		Select userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) as FP,count(*) as GEN_Cnt
		                 into #tbl_GEN
		                 FROM TBL_EWB_GENERATION  
						 where CustId=1  and  Ltrim(Rtrim(IsNull(ewaybillDate,''))) <> '' 
						 and flag is null
						 and RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) = @FP
						    group by userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) 

						
		Update #tempGSTIN 
		SET #tempGSTIN.Generate = t2.GEN_Cnt
		FROM #tempGSTIN t1,
			 #tbl_GEN t2 
		WHERE t1.gstinno = t2.userGSTIN 

		Select userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,cancelDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,cancelDate,103)) AS [CHAR](4)) as FP,count(*) as CANEL_Cnt
		                 into #tbl_CANCEL
		                 FROM TBL_EWB_GENERATION  
						 where CustId=1  and  Ltrim(Rtrim(IsNull(cancelDate,''))) <> '' and status='CNL' and (flag is null OR flag='G')
						 and RIGHT('0' + CAST(DATEPART(M,convert(date,cancelDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,cancelDate,103)) AS [CHAR](4)) = @FP
						    group by userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,cancelDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,cancelDate,103)) AS [CHAR](4)) 


		Update #tempGSTIN 
		SET #tempGSTIN.Cancelled = t2.CANEL_Cnt
		FROM #tempGSTIN t1,
			 #tbl_CANCEL t2 
		WHERE t1.gstinno = t2.userGSTIN 

		Select userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) as FP,count(*) as REJECT_Cnt
		                 into #tbl_REJECT
		                 FROM TBL_EWB_GENERATION  
						 where CustId=1  and  Ltrim(Rtrim(IsNull(ewbRejectedDate,''))) <> '' and rejectStatus='Y' and flag='O'
						 and RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) = @FP
						    group by userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) 
		
		

		Update #tempGSTIN 
		SET #tempGSTIN.RejectedByMe = t2.REJECT_Cnt
		FROM #tempGSTIN t1,
			 #tbl_REJECT t2 
		WHERE t1.gstinno = t2.userGSTIN 


		Select userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) as FP,count(*) as CounterREJECT_Cnt
		                 into #tbl_REJECT_Conter
		                 FROM TBL_EWB_GENERATION  
						 where CustId=1  and  Ltrim(Rtrim(IsNull(ewbRejectedDate,''))) <> '' and rejectStatus='Y' and flag='G'
						 and RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) = @FP
						    group by userGSTIN,RIGHT('0' + CAST(DATEPART(M,convert(date,ewbRejectedDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewbRejectedDate,103)) AS [CHAR](4)) 
		
		

		Update #tempGSTIN 
		SET #tempGSTIN.RejectedByCounterParty = t2.CounterREJECT_Cnt
		FROM #tempGSTIN t1,
			 #tbl_REJECT_Conter t2 
		WHERE t1.gstinno = t2.userGSTIN 



	     Select togstin,RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) as FP,count(*) as RECEIVE_Cnt
		                 into #tbl_RECEIVE
		                 FROM TBL_EWB_GENERATION 
						 where CustId=1  and  Ltrim(Rtrim(IsNull(ewaybillDate,''))) <> '' and flag='G'
						 and RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) = @FP
						    group by togstin,RIGHT('0' + CAST(DATEPART(M,convert(date,ewaybillDate,103)) AS [VARCHAR](2)),2) + CAST(DATEPART(YYYY,convert(date,ewaybillDate,103)) AS [CHAR](4)) 


		Update #tempGSTIN 
		SET #tempGSTIN.Received = t2.RECEIVE_Cnt
		FROM #tempGSTIN t1,
			 #tbl_RECEIVE t2 
		WHERE t1.gstinno = t2.togstin 

		Select sum(Generate) as Total_Generate,
		       sum(Received) as Total_Received,
			   sum(Cancelled) as Total_Cancelled,
			   sum(RejectedByMe) as Total_RejectedByMe,
			   sum(RejectedByCounterParty) as Total_RejectedByCounterParty into #Total FROM #tempGSTIN 
        
		Select * From #tempGSTIN
	    Select * From #Total 

	    Drop Table #Total
	    Drop Table #tbl_GEN
	    Drop Table #tbl_CANCEL
	    Drop Table #tbl_REJECT
	    Drop Table #tbl_RECEIVE
	    Drop Table #tempGSTIN


End