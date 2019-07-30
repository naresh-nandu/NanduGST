
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTR2 and GSTR2A CTINs
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
10/31/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2_GSTR2A_Ctins 1,1


 */
Create PROCEDURE [usp_Retrieve_GSTR2_GSTR2A_Ctins]
(
	@CustId int,
	@UserId int
)
as
Begin
		Declare @gstinid int

		DECLARE @TBL_Ctins TABLE 
		(
			Slno int,
			ctin varchar(15)
		)	
				
		select gstinid Into #GstinIds from tbl_cust_gstin where custid=@CustId and rowstatus = 1

			Insert @TBL_Ctins
					(Slno,ctin)
			select Row_Number() OVER(Order by ctin ASC) as slno,ctin from TBL_GSTR2_b2b t1 
			where gstinid in (select gstinid from #GstinIds) 
				And not exists(select 1 from @TBL_Ctins t2 where t2.ctin = t1.ctin)
				Group by ctin

			Insert @TBL_Ctins
					(Slno,ctin)
			select Row_Number() OVER(Order by ctin ASC) as slno, ctin from TBL_GSTR2A_b2b t1 
			where gstinid in (select gstinid from #GstinIds) 
				And not exists(select 1 from @TBL_Ctins t2 where t2.ctin = t1.ctin)
				Group by ctin

					Insert @TBL_Ctins
					(Slno,ctin)
			select Row_Number() OVER(Order by ctin ASC) as slno, ctin from TBL_GSTR2_CDNR t1 
			where gstinid in (select gstinid from #GstinIds) 
				And not exists(select 1 from @TBL_Ctins t2 where t2.ctin = t1.ctin)
				Group by ctin

				Insert @TBL_Ctins
					(Slno,ctin)
			select Row_Number() OVER(Order by ctin ASC) as slno, ctin from TBL_GSTR2A_CDNR t1 
			where gstinid in (select gstinid from #GstinIds) 
				And not exists(select 1 from @TBL_Ctins t2 where t2.ctin = t1.ctin)
				Group by ctin

		select Row_Number() OVER(Order by ctin ASC) as slno, ctin from @TBL_Ctins

End