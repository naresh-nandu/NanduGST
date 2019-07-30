

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve The GSTR1 Bulk Data Processed Summary 
				
Written by  : sheshadri.mk@wepdigital.com

Date		Who			Decription 
02/16/2018	Seshadri	Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_Bulkdata_Processed_Summary '03AACCM4684P1Z1', '082017'
 */

CREATE PROCEDURE [usp_Retrieve_GSTR1_Bulkdata_Processed_Summary]  
	@Gstin varchar(15),
	@Fp	varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Create Table #TBL_Summary_Data
	(
	 actiontype varchar(15),
	 gstin varchar(15),
	 txval dec(18,2),
	 iamt dec(18,2),
	 camt dec(18,2),
	 samt dec(18,2),
	 csamt dec(18,2)
	)


	Select gstr1id,gstin,fp,gt,cur_gt
	Into #GstrIds
	from TBL_GSTR1 
	where gstin = @Gstin
	And fp = @Fp

	Select gstr1id,sum(txval) as txval, 
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_B2B_Data
	from TBL_GSTR1_B2B_INV_ITMS_DET 
	where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Select gstr1id,sum(txval) as txval,
	sum(iamt) as iamt,0 as camt,0 as samt,sum(csamt) as csamt 
	Into #TBL_B2CL_Data
	from TBL_GSTR1_B2CL_INV_ITMS_DET 
	where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Select gstr1id,sum(txval) as txval,
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_B2CS_INV_Data
	from TBL_GSTR1_B2CS_INV_ITMS_DET 
	where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Select gstin,sum(txval) as txval,
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_CDNUR_Data
	From TBL_EXT_GSTR1_CDNUR
	Where gstin in (Select gstin From #GstrIds) and fp = @Fp
	group by gstin

	Select gstr1id,sum(txval) as txval,
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_B2CS_Data
	From TBL_GSTR1_B2CS
	Where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Select gstr1id,sum(txval) as txval,
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_CDNR_Data
	From TBL_GSTR1_CDNR_NT_ITMS_DET 
	Where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Select gstr1id,sum(txval) as txval,
	sum(iamt) as iamt,sum(camt) as camt,sum(samt) as samt,sum(csamt) as csamt 
	Into #TBL_HSN_Data
	From TBL_GSTR1_HSN_DATA
	Where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id


	Select gstr1id,sum(NIL_amt) as txval,
	0 as iamt,0 as camt,0 as samt,0 as csamt 
	Into #TBL_NIL_Data
	from TBL_GSTR1_NIL_INV 
	where gstr1id in(Select gstr1id From #GstrIds)
	group by gstr1id

	Insert Into #TBL_Summary_Data
	Select 'B2B',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_B2B_Data t2 On t2.gstr1id = t1.gstr1id 

	Insert Into #TBL_Summary_Data
	Select 'B2CL',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_B2CL_Data t2 On t2.gstr1id = t1.gstr1id

	Insert Into #TBL_Summary_Data
	Select 'B2CSINV',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_B2CS_INV_Data t2 On t2.gstr1id = t1.gstr1id

	Insert Into #TBL_Summary_Data
	Select 'CDNUR',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_CDNUR_Data t2 on t2.gstin = t1.gstin

	Insert Into #TBL_Summary_Data
	Select 'B2CS',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_B2CS_Data t2 On t2.gstr1id = t1.gstr1id
	
	Insert Into #TBL_Summary_Data
	Select 'CDNR',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_CDNR_Data t2 On t2.gstr1id = t1.gstr1id
	
	Insert Into #TBL_Summary_Data
	Select 'HSN',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_HSN_Data t2 On t2.gstr1id = t1.gstr1id

	Insert Into #TBL_Summary_Data
	Select 'NIL',
			t1.gstin,
			IsNull(t2.txval,0) ,
			IsNull(t2.iamt,0) ,
			IsNull(t2.camt,0) ,
			IsNull(t2.samt,0) ,
			IsNull(t2.csamt,0) 
	From #GstrIds t1 
	LEFT OUTER JOIN #TBL_NIL_Data t2 On t2.gstr1id = t1.gstr1id

	Select * From #TBL_Summary_Data
	Order by actiontype,gstin


	-- Drop Tables

	drop table #GstrIds 

	drop table #TBL_B2B_Data
	drop table #TBL_B2CL_Data
	drop table #TBL_B2CS_Inv_Data
	drop table #TBL_CDNUR_Data
	drop table #TBL_B2CS_Data
	drop table #TBL_CDNR_Data
	drop table #TBL_HSN_Data
	drop table #TBL_NIL_Data


End