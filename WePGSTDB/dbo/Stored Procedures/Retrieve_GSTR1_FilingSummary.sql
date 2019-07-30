/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve The GSTR1 Filling Summary Action wise
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec Retrieve_GSTR1_FilingSummary
 */
 
CREATE PROCEDURE [Retrieve_GSTR1_FilingSummary] 
	@ActionType nvarchar(7),
	@GSTINno nvarchar(15),
	@fp	varchar(10)
AS
BEGIN
	
	SET NOCOUNT ON;
	
	IF @ActionType = 'B2B'
	BEGIN	
			SELECT       B.ctin as ctin,'NA' as chksum,count( Binv.inum) as ttl_rec,sum(Binv.val) as ttl_val,
                        sum( binvitmdet.txval) as ttl_tax, sum( binvitmdet.iamt) as ttl_igst, 
                        sum( binvitmdet.camt) as ttl_cgst ,sum( binvitmdet.samt) as ttl_sgst, 
                         sum( binvitmdet.csamt) as ttl_cess, 'B2B' as sec_nm into #t1_b2b
				FROM     dbo.TBL_GSTR1 as gs1 INNER JOIN
                         dbo.TBL_GSTR1_B2B as B ON gs1.gstr1id = B.gstr1id INNER JOIN
                         dbo.TBL_GSTR1_B2B_INV as Binv ON B.b2bid = Binv.b2bid INNER JOIN
                         dbo.TBL_GSTR1_B2B_INV_ITMS as binvitm ON Binv.invid = binvitm.invid INNER JOIN
                         dbo.TBL_GSTR1_B2B_INV_ITMS_DET as binvitmdet ON binvitm.itmsid = binvitmdet.itmsid where gs1.gstin = @GSTINno group by  B.ctin

						select * into #t2_b2b from #t1_b2b
						if not exists (select ttl_rec from #t1_b2b  where ctin like '.Total')
							Begin
							insert into #t1_b2b(ctin,chksum,ttl_rec,ttl_val,ttl_igst,ttl_cgst,ttl_sgst,ttl_cess,ttl_tax,sec_nm ) 
							select '.Total', 'NA', sum(ttl_rec) , sum(ttl_val), sum(ttl_igst),sum(ttl_cgst),sum(ttl_sgst), sum(ttl_cess), sum(ttl_tax) ,'B2B'
							from #t2_b2b group by sec_nm
							End
						select ctin as 'CTIN', chksum  as 'CHK SUM',ttl_rec as 'Total Records', ttl_val as 'Total Invoice Value', ttl_tax as 'Total Taxable value',ttl_igst as 'Total IGST Amount' from #t1_b2b order by ctin desc
		END

	Else 
		Begin
			select 'Other Actions are in progress'
		End
End