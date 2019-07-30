
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B Dashboard from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/06/2017	Karthik 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_Dashboard 1,1,'072017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR3B_Dashboard] 
	@CustId int,
	@UserId int,
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

				Create Table #TBL_GSTR3B_Dashboard
				(
					GSTINNo varchar(15),
					GSTINId int,
					PANNo varchar(10),
					CompanyName nvarchar(250),
					GSTR3B_U int,
					GSTR3B_F int,
					Otxval decimal(18,2),
					Oiamt decimal(18,2),
					Ocamt decimal(18,2),
					Osamt decimal(18,2),
					Ocsamt decimal(18,2),
					Itxval decimal(18,2),
					Iiamt decimal(18,2),
					Icamt decimal(18,2),
					Isamt decimal(18,2),
					Icsamt decimal(18,2),
					Ltxval decimal(18,2),
					Liamt decimal(18,2),
					Lcamt decimal(18,2),
					Lsamt decimal(18,2),
					Lcsamt decimal(18,2)
				)

				Insert Into #TBL_GSTR3B_Dashboard(GSTINNo,GSTINId,PANNo)				
				SELECT CG.GSTINNo,CG.GSTINId,CG.PANNo
				FROM   TBL_Cust_GSTIN CG INNER JOIN
					   UserAccess_GSTIN UG  ON CG.GSTINId = UG .GSTINId
				where UG .Rowstatus=1 
						And CG.Rowstatus =1
						And UG.userid= @UserId 
						And UG.custid= @CustId
										
				update #TBL_GSTR3B_Dashboard set CompanyName=(select distinct CompanyName from TBL_Cust_PAN where TBL_Cust_PAN.PANNo=#TBL_GSTR3B_Dashboard.Panno and TBL_Cust_PAN.custid=@CustId and rowstatus=1)

				-- Upload Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #TBL_GSTR3B_Dashboard set GSTR3B_U =0 where gstinno not in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3B'  and fp = @Fp)
				-- Checking If GSTIN is available in GSTR Save response table
				update #TBL_GSTR3B_Dashboard set GSTR3B_U =1 where gstinno in (select GSTINNo from TBL_GSTR_SAVE_Response where  GSTRName = 'GSTR3B'  and fp = @Fp)
				-- Checking If GSTIN is available in GSTR Save response table
				update #TBL_GSTR3B_Dashboard set GSTR3B_U =2 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3B'  and fp = @Fp)
				-- Filing Status
				-- Checking GSTIN (Is available in GSTR Save response).
				update #TBL_GSTR3B_Dashboard set GSTR3B_F =0 where gstinno not in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3B'  and fp = @Fp)
				-- Checking If GSTIN is available in GSTR Save response table
				update #TBL_GSTR3B_Dashboard set GSTR3B_F =1 where gstinno in (select GSTINNo from TBL_GSTR_FILE_Response where  GSTRName = 'GSTR3B'  and fp = @Fp)
			
				Create Table #TBL_GSTR3B_RECS
				(
					gstinid int,
					supply_num int,
					natureofsupplies varchar(100),
					txval decimal(18,2),
					iamt decimal(18,2),
					camt decimal(18,2),
					samt decimal(18,2),
					csamt decimal(18,2),
				)


				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,txval,iamt,camt,samt,csamt)
				Select 
					t1.gstinid,
					1 as supply_num,
					sum(IsNull(txval,0)), 
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_sup_det_osup_det t3 On t3.sup_detid = t2.sup_detid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				Group By t1.gstinid
				
				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,txval,iamt,csamt)
				Select
					t1.gstinid,
					2 as supply_num,
					sum(IsNull(txval,0)),  
					sum(IsNull(iamt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_sup_det_osup_zero t3 On t3.sup_detid = t2.sup_detid
				Where  t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				Group By t1.gstinid
				
				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,txval)
				Select
					t1.gstinid,
					3 as supply_num,
					sum(IsNull(txval,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_sup_det_osup_nil_exmp t3 On t3.sup_detid = t2.sup_detid
				Where  t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,txval,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					4 as supply_num,
					sum(IsNull(txval,0)), 
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_sup_det_isup_rev t3 On t3.sup_detid = t2.sup_detid
				Where  t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,natureofsupplies,txval,iamt,camt,samt,csamt)
				Select
					gstinid,
					99 as supply_num,
					'Total Outward Tax Liability' , 
					sum(IsNull(txval,0)), 
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From #TBL_GSTR3B_RECS 
				Where supply_num in (1,2,3,4) group by gstinid

				-- Supply_num 6,7,8 Not Required for This dashboard.
			
				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,csamt)
				Select
					t1.gstinid,
					9 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'IMPG'
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,csamt)
				Select
					t1.gstinid,
					10 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'IMPS'
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					11 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'ISRC'
				Group By t1.gstinid


				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					13 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'OTH'
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					14 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'RUL'
				Group By t1.gstinid


				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					15 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = 'OTH'
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,iamt,camt,samt,csamt)
				Select
					t1.gstinid,
					16 as supply_num,
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From TBL_GSTR3B t1
				Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
				Inner Join TBL_GSTR3B_itc_elg_itc_net t3 On t3.itc_elgid = t2.itc_elgid
				Where t1.gstinid in (select gstinid from #TBL_GSTR3B_Dashboard)
				And ret_period = @Fp
				And t3.ty = ''
				Group By t1.gstinid

				Insert Into #TBL_GSTR3B_RECS (gstinid,supply_num,natureofsupplies,txval,iamt,camt,samt,csamt)
				Select
					gstinid,
					100 as supply_num,
					'Total Input Tax Credit', 
					sum(IsNull(txval,0)), 
					sum(IsNull(iamt,0)),
					sum(IsNull(camt,0)),
					sum(IsNull(samt,0)),
					sum(IsNull(csamt,0))
				From #TBL_GSTR3B_RECS 
				Where supply_num in (9,10,11,13) 
				group by gstinid

			-- Output Put Tax Liability Updates
				
				Update #TBL_GSTR3B_Dashboard 
						SET #TBL_GSTR3B_Dashboard.Otxval = t2.txval,
						#TBL_GSTR3B_Dashboard.Oiamt = t2.iamt,
						#TBL_GSTR3B_Dashboard.Ocamt = t2.camt,
						#TBL_GSTR3B_Dashboard.Osamt = t2.samt,
						#TBL_GSTR3B_Dashboard.Ocsamt = t2.csamt
						FROM #TBL_GSTR3B_Dashboard t1,
								#TBL_GSTR3B_RECS t2 
						WHERE t1.gstinid = t2.gstinid
						And t2.natureofsupplies = 'Total Outward Tax Liability'

			-- Input Tax Credit Updates
				
				Update #TBL_GSTR3B_Dashboard 
						SET #TBL_GSTR3B_Dashboard.Itxval = t2.txval,
						#TBL_GSTR3B_Dashboard.Iiamt = t2.iamt,
						#TBL_GSTR3B_Dashboard.Icamt = t2.camt,
						#TBL_GSTR3B_Dashboard.Isamt = t2.samt,
						#TBL_GSTR3B_Dashboard.Icsamt = t2.csamt
						FROM #TBL_GSTR3B_Dashboard t1,
								#TBL_GSTR3B_RECS t2 
						WHERE t1.gstinid = t2.gstinid
						And t2.natureofsupplies = 'Total Input Tax Credit'

			-- Ledger Updates
					Select GSTINNo,GSTINId,PANNo,CompanyName,GSTR3B_U,GSTR3B_F,
					IsNull(Otxval,0) as 'OTaxable',
					IsNull(Oiamt,0) as 'OIGST',
					IsNull(Ocamt,0) as 'OCGST',
					IsNull(Osamt,0) as 'OSGST',
					IsNull(Ocsamt,0) as 'OCESS',
					IsNull(Itxval,0) as 'ITaxable',
					IsNull(Iiamt,0) as 'IIGST',
					IsNull(Icamt,0) as 'ICGST',
					IsNull(Isamt,0) as 'ISGST',
					IsNull(Icsamt,0) as 'ICESS',
					IsNull(Ltxval,0) as 'LTaxable',
					IsNull(Liamt,0) as 'LIGST',
					IsNull(Lcamt,0) as 'LCGST',
					IsNull(Lsamt,0) as 'LSGST',
					IsNull(Lcsamt,0) as 'LCESS'
					From  #TBL_GSTR3B_Dashboard
				


	Return 0

End