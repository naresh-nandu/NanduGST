
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the Flat Summary Dashboard
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_Summary_Dashboard_Pie 1,1	,'072017'


 */
 
CREATE PROCEDURE [usp_Retrieve_Summary_Dashboard_Pie]  
	@CustId int,
	@UserId int,
	@Period varchar(10)
	--@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

--Declare @Period varchar(10)
--set @Period ='072017'
	Set Nocount on

	
	SELECT CG.GSTINNo,CG.GSTINId,  0 as 'GSTR1_U', 0 as 'GSTR2_U',0 as 'GSTR3_U', 0 as 'GSTR1_F', 0 as 'GSTR2_F',0 as 'GSTR3_F' 
				into #tempGSTIN
				FROM   TBL_Cust_GSTIN CG INNER JOIN
						UserAccess_GSTIN UG  ON CG.GSTINId = UG .GSTINId
				where UG .Rowstatus=1 
					 And CG.Rowstatus =1
					 And UG.userid= @UserId 
					 And UG.custid= @CustId

				--update #tempGSTIN set CompanyName=(select distinct CompanyName from TBL_Cust_PAN where TBL_Cust_PAN.PANNo=#tempGSTIN.Panno and TBL_Cust_PAN.custid=@CustId and rowstatus=1)
				-- Upload Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_U =0 where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1' and fp = @Period)
				update #tempGstin set GSTR2_U =0 where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2' and fp = @Period)
				update #tempGstin set GSTR3_U =0 where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3' and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U =1 where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1' and fp = @Period)
				update #tempGstin set GSTR2_U =1 where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2' and fp = @Period)
				update #tempGstin set GSTR3_U =1 where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3' and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U =2 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1' and fp = @Period)
				update #tempGstin set GSTR2_U =2 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2' and fp = @Period)
				update #tempGstin set GSTR3_U =2 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3' and fp = @Period)


				-- Filing Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_F =0 where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1' and fp = @Period)
				update #tempGstin set GSTR2_F =0 where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2' and fp = @Period)
				update #tempGstin set GSTR3_F =0 where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3' and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_F =1 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1' and fp = @Period)
				update #tempGstin set GSTR2_F =1 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2' and fp = @Period)
				update #tempGstin set GSTR3_F =1 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3' and fp = @Period)

				
				--select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS into #temp2 from TBL_GSTR1_B2B_INV_ITMS_DET where gstinid in (select gstinid from #tempGstin) group by gstinid
				---- With out checking the flags
				----select gstinid,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #temp2 from (
				----select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
				----		from TBL_GSTR1_B2B_INV_ITMS_DET 
				----		where  gstinid in (select gstinid from #tempGstin) and 
				----			   gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))   
				----	    group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,sum(csamt)as OCESS  from TBL_GSTR1_B2CL_INV_ITMS_DET where gstinid in (select gstinid from #tempGstin) and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  from TBL_GSTR1_B2CS where gstinid in (select gstinid from #tempGstin) and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin))  group by gstinid union ALL
				----select gstinid,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,0 as OCESS  from TBL_GSTR1_EXP_INV_ITMS where gstinid in (select gstinid from #tempGstin)  and gstr1id in (select gstr1id from tbl_Gstr1 where fp = @Period and gstinid in (select gstinid from #tempGstin)) group by gstinid)  as AB
				----group by gstinid

				-- Checking flags for output tax liability

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

				--select ROW_NUMBER() OVER(ORDER BY gstinID ) AS 'S.No',* from #temp3

				select count(gstinID) as TotalGSTINs,sum(GSTR1_F) as TotalGSTR1Filed,(sum(GSTR1_F) * 100 /count(gstinID)) as GSTR1Filed_Percent,
						sum(GSTR2_F) as TotalGSTR2Filed,(sum(GSTR2_F) * 100 /count(gstinID)) as GSTR2Filed_Percent,
						sum(GSTR3_F) as TotalGSTR3Filed,(sum(GSTR3_F) * 100 /count(gstinID)) as GSTR3Filed_Percent, 
						--sum(OIGST) as TIGST,sum(OCGST) as TCGST, sum(OSGST) as TSGST, sum(OCESS) as TCESS , 
						(sum(OIGST) + sum(OCGST) + sum(OSGST) + sum(OCESS)) as TotalTXI, 0 as TotalITC, 0 as TotalLedger into #Temp4 from #temp3

				select * from #Temp4

	--Return 0

End