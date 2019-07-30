
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the Details Summary Dashboard
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_Summary_Dashboard_Details 1,1,'072017','33GSPTN0801G1ZM'


 */
 
CREATE PROCEDURE [usp_Retrieve_Summary_Dashboard_Details]  
	@CustId int,
	@UserId int,
	@Period varchar(10),
	@Gstinno varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
				Declare @MonthName varchar(25)		 
				select @MonthName = DateName(mm,DATEADD(mm,convert(int,substring(@Period,1,2)) - 1,0))
				CREATE TABLE #tempGSTIN(
						GSTINNo nvarchar(15) NULL,
						GSTINId [int] NULL,
						GSTR1_U [nvarchar](100) NULL,
						GSTR2_U [nvarchar](100) NULL,
						GSTR3_U [nvarchar](100) NULL,
						GSTR1_F [nvarchar](100) NULL,
						GSTR2_F [nvarchar](100) NULL,
						GSTR3_F [nvarchar](100) NULL
						)
				insert #tempGSTIN(GSTINNo,GSTINId,GSTR1_U,GSTR2_U,GSTR3_U,GSTR1_F,GSTR2_F,GSTR3_F )
				SELECT CG.GSTINNo,CG.GSTINId,  '', '' , '','','','' 
				FROM   TBL_Cust_GSTIN CG INNER JOIN
						UserAccess_GSTIN UG  ON CG.GSTINId = UG .GSTINId
				where UG .Rowstatus=1 
					 And CG.Rowstatus =1
					 And UG.userid= @UserId 
					 And UG.custid= @CustId
					 And CG.GSTINNO = @Gstinno
				-- Upload Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_U ='GSTR1 Not yet uploaded for '+ @MonthName + ' Month' where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U ='GSTR2 Not yet uploaded for '+ @MonthName + ' Month' where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U ='GSTR3 Not yet uploaded for '+ @MonthName + ' Month' where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U ='GSTR1 Uploaded till : '+ (select  convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and fp = @Period) where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U ='GSTR2 Uploaded till : '+ (select convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)  where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U ='GSTR3 Uploaded till : '+ (select convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)  where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U =(select convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)  where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U =(select convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)  where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U =(select convert(varchar(15), format(max(createddate),'dd-MM-yyyy')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)  where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)


				-- Filing Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_F = 'Pending for GSTR1 filing. Due Date is 10th ' + (select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month'  where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_F ='Pending for GSTR2 filing. Due Date is 10th ' + (select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month'  where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_F ='Pending for GSTR3 filing. Due Date is 10th ' + (select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month'  where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				update #tempGstin set GSTR1_F = 'Pending for GSTR1 filing. Due Date is 10th ' + @MonthName  where GSTR1_F is NULL
				update #tempGstin set GSTR2_F ='Pending for GSTR2 filing. Due Date is 10th ' + @MonthName where  GSTR2_F is NULL
				update #tempGstin set GSTR3_F ='Pending for GSTR3 filing. Due Date is 10th ' + @MonthName  where  GSTR3_F is NULL


				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_F ='GSTR1 Filed Successfully for  ' +(select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month'  where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_F ='GSTR2 Filed Successfully for ' + (select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month' where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_F ='GSTR3 Filed Successfully for ' + (select convert(varchar(15), format(max(createddate),'MMM')) from TBL_GSTR_SAVE_Response  where gstinno = @Gstinno and  fp = @Period)+ ' Month' where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)


				--select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS into #temp2 from TBL_GSTR1_B2B_INV_ITMS_DET where gstinid in (select gstinid from #tempGstin) group by gstinid
				---- Checking without Flags
				----select gstinid,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #temp2 from (
				----select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  from TBL_GSTR1_B2B_INV_ITMS_DET where gstinid in (select gstinid from #tempGstin)  and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,sum(csamt)as OCESS  from TBL_GSTR1_B2CL_INV_ITMS_DET where gstinid in (select gstinid from #tempGstin) and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  from TBL_GSTR1_B2CS where gstinid in (select gstinid from #tempGstin) and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,0 as OCESS  from TBL_GSTR1_EXP_INV_ITMS where gstinid in (select gstinid from #tempGstin) and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid ) as AB
				----group by gstinid

					-- Checking flags

				select AB.gstinid,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #temp2 from (
				select t3.gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR1_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid 
							union ALL
				select t3.gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,sum(csamt)as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'  
							group by t3.gstinid 
							union ALL
				select t2.gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
							Where t2.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'  
							group by t2.gstinid 
							union ALL
				select t3.gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,0 as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
							Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'  
							group by t3.gstinid)  as AB
				group by AB.gstinid




				select #tempGstin.*,OIGST,OCGST,OSGST,OCESS, 0 AS IIGST, 0 AS ICGST, 0 AS ISGST,0 AS ICESS, 0 AS LIGST, 0 AS LCGST, 0 AS LSGST,0 AS LCESS into #temp3  from #tempGstin left outer join #temp2 on #tempGstin.gstinid=#temp2.gstinid

				update #temp3 set OIGST = 0 where OIGST is NULL
				update #temp3 set OCGST = 0 where OCGST is NULL
				update #temp3 set OSGST = 0 where OSGST is NULL
				update #temp3 set OCESS = 0 where OCESS is NULL

				select ROW_NUMBER() OVER(ORDER BY gstinID ) AS 'S.No',* from #temp3


				--select * from #tempGstin

	Return 0

End