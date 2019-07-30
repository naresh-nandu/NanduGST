/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version
11/24/2017	Seshadri	Modified the code to display appropriate customer data

*/

/* Sample Procedure Call  

exec usp_Retrieve_TRP_Summary_Dashboard_Flat 1,1,'072017'

 */
 
CREATE PROCEDURE [usp_Retrieve_TRP_Summary_Dashboard_Flat]  
	@TRPId int,
	@TRPUserId int,
	@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select role_id,role_name 
	Into #TrpRoles 
	From Tbl_trp_roles 
	Where Trpid = @TrpId

	Create Table  #CustIds
	(
		custid int,
	 )

	if Exists(Select 1 From TBL_TRP_Userlist t1,
							#TrpRoles t2		 
				Where t1.trpuserid = @TrpUserId
				And	t1.trpcustid = @TrpId
				And t1.roleid = t2.role_id
				And(t2.role_name = 'Admin' Or t2.role_name = 'Super Admin') )
	Begin

		Insert Into  #CustIds
		Select custid 
		From TBL_Customer 
		Where TRPId = @TRPId
		And statuscode Not In (6,3)
		And rowstatus =1
	
	End
	else
	Begin

		Insert Into  #CustIds
		Select custid  
		From TBL_TRP_UserAccess_Customer 
		Where trpid = @TrpId
		And (trpuserId = @TrpUserId Or createdby = @TrpUserId)
		And rowstatus = 1 

	End 


		
			
				SELECT CG.GSTINNo,CG.CustId,CG.GSTINId,CG.PANNo, 0 as 'GSTR1_U', 0 as 'GSTR2_U',0 as 'GSTR3_U', 0 as 'GSTR1_F', 0 as 'GSTR2_F',0 as 'GSTR3_F' into #tempGSTIN
				FROM   TBL_Cust_GSTIN CG INNER JOIN
						UserAccess_GSTIN UG  ON CG.GSTINId = UG .GSTINId
				where UG .Rowstatus=1 
					 And CG.Rowstatus =1 
					 And UG.custid in (Select custid From #CustIds)


				-- Upload Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_U =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_U =2 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_U =2 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_U =2 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)


				-- Filing Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #tempGstin set GSTR1_F =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_F =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_F =0 where gstinno not in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)

				-- Checking If GSTIN is available in GSTR Save response table
				update #tempGstin set GSTR1_F =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR1'  and fp = @Period)
				update #tempGstin set GSTR2_F =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR2'  and fp = @Period)
				update #tempGstin set GSTR3_F =1 where gstinno in (select distinct GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3'  and fp = @Period)


					-- Checking flags for output tax liability
					-- Adding B2b, B2Cl,B2Cs,EXP for output tax liability.
					select AB.gstinid,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS, 0 AS IIGST, 0 AS ICGST, 0 AS ISGST,0 AS ICESS into #temp2 from (
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

				-- Taking CDNR and CDNUR i.e need to add with Outputliability if ntty is 'D' or else need to minus from outputliability.
					select ACD.gstinid,ACD.ntty,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #tmpCDN from (
					select t3.gstinid,t3.ntty,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid,t3.ntty
							Union All
							select t3.gstinid,t2.ntty,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid,t2.ntty
							)as ACD
				group by ACD.gstinid,ACD.ntty

				-- Add/Minus Credit/Debit data in output tax liability based on 'ntty- note Type'
				Update #temp2 
				SET #temp2.OIGST =  t1.OIGST + t2.OIGST, 
					#temp2.OCGST =  t1.OCGST + t2.OCGST,
					#temp2.OSGST =  t1.OSGST + t2.OSGST,
					#temp2.OCESS =  t1.OCESS + t2.OCESS
				FROM #temp2 t1,
						#tmpCDN t2 
				WHERE t1.gstinid = t2.gstinid 
				And t2.ntty = 'C'

				Update #temp2 
				SET #temp2.OIGST =  t1.OIGST - t2.OIGST, 
					#temp2.OCGST =  t1.OCGST - t2.OCGST,
					#temp2.OSGST =  t1.OSGST - t2.OSGST,
					#temp2.OCESS =  t1.OCESS - t2.OCESS
				FROM #temp2 t1,
						#tmpCDN t2 
				WHERE t1.gstinid = t2.gstinid 
				And t2.ntty = 'D'



				-- Getting ITC data from GSTR2 all action type from itms_Itc tables of all action.
					select AB.gstinid,sum(IIGST) as IIGST,sum(ICGST) as ICGST,sum(ISGST) as ISGST,sum(ICESS)as ICESS into #tmpITC from (
				select t3.gstinid,sum(tx_i)as IIGST,sum(tx_c)as ICGST,sum(tx_s) as ISGST,sum(tx_cs)as ICESS  
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid
							UNION ALL
					   select t3.gstinid,sum(tx_i)as IIGST,sum(tx_c)as ICGST,sum(tx_s) as ISGST,sum(tx_cs)as ICESS  
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2BUR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2BUR_INV t3 On t3.b2burid = t2.b2burid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS_ITC t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid
							)as AB
				group by AB.gstinid

				-- Taking CDNR and CDNUR i.e need to add with Outputliability if ntty is 'D' or else need to minus from outputliability.
					select ACD.gstinid,ACD.ntty,sum(IIGST) as IIGST,sum(ICGST) as ICGST,sum(ISGST) as ISGST,sum(ICESS)as ICESS  into #tmpCDNITC from (
					select t3.gstinid,t3.ntty,sum(tx_i)as IIGST,sum(tx_c)as ICGST,sum(tx_s) as ISGST,sum(tx_cs)as ICESS    
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t5 On t5.itmsid = t4.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid,t3.ntty
							Union All
							select t3.gstinid,t2.ntty,sum(tx_i)as IIGST,sum(tx_c)as ICGST,sum(tx_s) as ISGST,sum(tx_cs)as ICESS   
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_CDNUR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR2_CDNUR_ITMS_ITC t4 On t4.itmsid = t3.itmsid
							Where t3.gstinid in (select #tempGstin.gstinid from #tempGstin)
							And fp = @Period 
							And flag = 'U'
							group by t3.gstinid,t2.ntty
							)as ACD
				group by ACD.gstinid,ACD.ntty

				Update #temp2 
				SET #temp2.IIGST =  t2.IIGST, 
					#temp2.ICGST =  t2.ICGST,
					#temp2.ISGST =  t2.ISGST,
					#temp2.ICESS =  t2.ICESS
				FROM #temp2 t1,
						#tmpITC t2 
				WHERE t1.gstinid = t2.gstinid 

					-- Add/Minus Credit/Debit data in ITC tax liability based on 'ntty- note Type'
				Update #temp2 
				SET #temp2.IIGST =  t1.IIGST + t2.IIGST, 
					#temp2.ICGST =  t1.ICGST + t2.ICGST,
					#temp2.ISGST =  t1.ISGST + t2.ISGST,
					#temp2.ICESS =  t1.ICESS + t2.ICESS
				FROM #temp2 t1,
						#tmpCDNITC t2 
				WHERE t1.gstinid = t2.gstinid 
				And t2.ntty = 'D'

				Update #temp2 
				SET #temp2.IIGST =  t1.IIGST - t2.IIGST, 
					#temp2.ICGST =  t1.ICGST - t2.ICGST,
					#temp2.ISGST =  t1.ISGST - t2.ISGST,
					#temp2.ICESS =  t1.ICESS - t2.ICESS
				FROM #temp2 t1,
						#tmpCDNITC t2 
				WHERE t1.gstinid = t2.gstinid 
				And t2.ntty = 'C'

				select #tempGstin.GSTINNo,#tempGstin.CustId,#tempGstin.GSTINId,#tempGstin.PANNo,GSTR1_U,GSTR2_U,GSTR3_U,GSTR1_F,GSTR2_F,GSTR3_F,OIGST,OCGST,OSGST,OCESS,IIGST,ICGST,ISGST,ICESS,0 AS LIGST, 0 AS LCGST, 0 AS LSGST,0 AS LCESS into #temp3  from #tempGstin left outer join #temp2 on #tempGstin.gstinid=#temp2.gstinid

				update #temp3 set OIGST = 0 where OIGST is NULL
				update #temp3 set OCGST = 0 where OCGST is NULL
				update #temp3 set OSGST = 0 where OSGST is NULL
				update #temp3 set OCESS = 0 where OCESS is NULL

				update #temp3 set IIGST = 0 where IIGST is NULL
				update #temp3 set ICGST = 0 where ICGST is NULL
				update #temp3 set ISGST = 0 where ISGST is NULL
				update #temp3 set ICESS = 0 where ICESS is NULL

				--select ROW_NUMBER() OVER(ORDER BY gstinID ) AS 'S.No',* from #temp3


				select CustId, Space(250) as CompanyName, Space(100) as Email, Space(100) as [password],
						Sum(GSTR1_U) as GSTR1_U,
						Sum(GSTR2_U) as GSTR2_U,
						Sum(GSTR3_U) as GSTR3_U,
						Sum(GSTR1_F) as GSTR1_F,
						Sum(GSTR2_F) as GSTR2_F,
						Sum(GSTR3_F) as GSTR3_F,
						Sum(OIGST) as OIGST,
						Sum(OCGST) as OCGST,
						Sum(OSGST) as OSGST,
						Sum(OCESS) as OCESS,
						Sum(IIGST) as IIGST,
						Sum(ICGST) as ICGST,
						Sum(ISGST) as ISGST,
						Sum(ICESS) as ICESS,
						Sum( LIGST) as  LIGST,
						Sum( LCGST) as  LCGST,
						Sum( LSGST) as  LSGST,
						Sum( LCESS) as  LCESS into #tbl_DashboardResult
						 from #temp3 
						Group By CustId


						Update #tbl_DashboardResult 
						SET #tbl_DashboardResult.CompanyName = t2.company,
							#tbl_DashboardResult.Email = t2.Email
						FROM #tbl_DashboardResult t1,TBL_Customer t2 
						WHERE t1.custid = t2.custid  and t2.rowstatus =1

						Update #tbl_DashboardResult 
						SET #tbl_DashboardResult.[Password] = t2.[password] 
						FROM #tbl_DashboardResult t1,UserList t2 
						WHERE t1.custid = t2.custid
							  And t1.Email=t2.Email and t2.rowstatus =1

						Select * from #tbl_DashboardResult
				--select * from #tempGstin

	-- Drop Temp Tables

	 Drop Table  #CustIds


	Return 0

End