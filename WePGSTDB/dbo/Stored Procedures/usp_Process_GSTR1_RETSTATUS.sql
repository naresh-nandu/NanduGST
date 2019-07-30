
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to process GSTR1 RETSTATUS
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
09/03/2017	Seshadri 	Initial Version
03/06/2018	Seshadri	Implemented the logic

*/

/* Sample Procedure Call

exec usp_Process_GSTR1_RETSTATUS 

 */
 
CREATE PROCEDURE usp_Process_GSTR1_RETSTATUS
	@Gstin varchar(15),
	@Fp varchar(10),
	@ReferenceNo varchar(255),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @RecordContents nvarchar(max)

	Select @RecordContents = errorreport
	From TBL_GSTR1_SAVE_RETSTATUS
	Where gstin = @Gstin
	And fp = @Fp
	And referenceno = @ReferenceNo

	Select @RecordContents

	-- Error Records - B2B
	 
	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			ctin as ctin ,
			error_cd as error_cd,
			error_msg as error_msg,
			inum as inum,
			idt as idt,
			val as val ,
			pos as pos,
			rchrg as rchrg,
			inv_typ as inv_typ,
			etin as etin,
			num as num,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_B2B		
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		b2b nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.b2b) 
	WITH
	(
		ctin varchar(15),
		error_cd varchar(15),
		error_msg varchar(255),
		inv nvarchar(max) as JSON
	) As B2b
	Cross Apply OPENJSON(B2b.inv) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
		val decimal(18,2),
		pos varchar(2),
		rchrg varchar(1),
		inv_typ varchar(5),
		etin varchar(15),
		itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		num int,
		itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Itm_Det

	-- Error Records - B2CL

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			pos as pos,
			error_cd as error_cd,
			error_msg as error_msg,
			inum as inum,
			idt as idt,
			etin as etin,
			val as val ,
			num as num,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_B2CL		
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		b2cl nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.b2cl) 
	WITH
	(
		pos varchar(2),
		error_cd varchar(15),
		error_msg varchar(255),
		inv nvarchar(max) as JSON
	) As B2cl
	Cross Apply OPENJSON(B2cl.inv) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
		etin varchar(15),
		val decimal(18,2),
		itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		num int,
		itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		csamt decimal(18,2)
	) As Itm_Det

	-- Error Records - B2CS

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			pos as pos,
			typ as typ,
			sply_ty as sply_ty,
			etin as etin,
			rt as rt,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt,
			error_cd as error_cd,
			error_msg as error_msg
	Into #TBL_GSTR1_RETSTATUS_B2CS		
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		b2cs nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.b2cs) 
	WITH
	(
		pos varchar(2),
		typ varchar(2),
		sply_ty varchar(5),
		etin varchar(15),
		rt decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
		error_cd varchar(15),
		error_msg varchar(255)
	) As B2cs

	-- Error Records - CDNR
	
	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			ctin as ctin ,
			error_cd as error_cd,
			error_msg as error_msg,
			ntty as ntty,
			nt_num as nt_num,
			nt_dt as nt_dt,
			rsn as rsn,
			p_gst as p_gst,
			inum as inum,
			idt as idt,
			val as val ,
			num as num,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_CDNR
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		cdnr nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.cdnr) 
	WITH
	(
		ctin varchar(15),
		error_cd varchar(15),
		error_msg varchar(255),
		nt nvarchar(max) as JSON
	) As Cdnr
	Cross Apply OPENJSON(Cdnr.nt) 
	WITH
	(
		ntty varchar(1),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(1),
    	inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
    	itms nvarchar(max) as JSON
	) As Nt
	Cross Apply OPENJSON(Nt.itms)
	WITH
	(
		num int,
        itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
        txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
        csamt decimal(18,2)
	) As Itm_Det

	-- Error Records - EXP

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			exp_typ as exp_typ ,
			error_cd as error_cd,
			error_msg as error_msg,
			inum as inum,
			idt as idt,
			val as val ,
			sbpcode as sbpcode,
			sbnum as sbnum,
			sbdt as sbdt,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_EXP
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		[exp] nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.[exp]) 
	WITH
	(
		exp_typ varchar(5),
		error_cd varchar(15),
		error_msg varchar(255),
		inv nvarchar(max) as JSON
	) As [Exp]
	Cross Apply OPENJSON([Exp].inv) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
		sbpcode varchar(6),
		sbnum varchar(15),
		sbdt varchar(50),
    	itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		rt decimal(18,2),
        txval decimal(18,2),
		iamt decimal(18,2),
	    csamt decimal(18,2)
	) As Itms

	-- Error Records - HSN

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			error_cd as error_cd,
			error_msg as error_msg,
			num as num,
			hsn_sc as hsn_sc,
			[desc] as [desc],
			uqc as uqc,
			qty as qty,
			val as val ,
			txval as txval,
			iamt as iamt ,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_HSN
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		hsnsum nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.hsnsum) 
	WITH
	(
		error_cd varchar(15),
		error_msg varchar(255),
		[data] nvarchar(max) as JSON
	) As HsnSum
	Cross Apply OPENJSON(HsnSum.[data]) 
	WITH
	(	
		num int,
		hsn_sc varchar(10),
		[desc] varchar(50),
		uqc varchar(50),
		qty decimal(18,2),
        val decimal(18,2),
	    txval decimal(18,2),
		iamt decimal(18,2),
	    csamt decimal(18,2)
	) As [Data]

	-- Error Records - NIL

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			error_cd as error_cd,
			error_msg as error_msg,
			sply_ty as sply_ty,
			expt_amt as expt_amt,
			nil_amt as nil_amt ,
			ngsup_amt as ngsup_amt
	Into #TBL_GSTR1_RETSTATUS_NIL
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		nil nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.nil) 
	WITH
	(
		error_cd varchar(15),
		error_msg varchar(255),
		inv nvarchar(max) as JSON
	) As Nil
	Cross Apply OPENJSON(Nil.inv) 
	WITH
	(	
		sply_ty varchar(25),
		expt_amt decimal(18,2),
		nil_amt decimal(18,2),
	    ngsup_amt decimal(18,2)
	) As Inv


	-- Error Records - TXP

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			error_cd as error_cd,
			error_msg as error_msg,
			pos as pos,
			sply_ty as sply_ty,
			rt as rt ,
			ad_amt as ad_amt,
			iamt as iamt ,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_TXP
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		txpd nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.txpd) 
	WITH
	(
		error_cd varchar(15),
		error_msg varchar(255),
		pos varchar(2),
		sply_ty varchar(25),
		itms nvarchar(max) as JSON
	) As Txpd
	Cross Apply OPENJSON(Txpd.itms) 
	WITH
	(	
		rt decimal(18,2),
        ad_amt decimal(18,2),
		iamt decimal(18,2),
	    csamt decimal(18,2)
	) As Itms

	-- Error Records - AT

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			@Gstin as gstin,
			@Fp as fp,
			error_cd as error_cd,
			error_msg as error_msg,
			pos as pos,
			sply_ty as sply_ty,
			rt as rt ,
			ad_amt as ad_amt,
			iamt as iamt ,
			csamt as csamt
	Into #TBL_GSTR1_RETSTATUS_AT
	From OPENJSON(@RecordContents) 	
	WITH
	(
		form_typ varchar(15),
		status_cd varchar(15),
		[action] varchar(15),
		error_report nvarchar(max) as JSON 
	) As Error
	Outer Apply OPENJSON(Error.error_report) 
	WITH
	(
		[at] nvarchar(max) as JSON
	) As Error_Report
	Cross Apply OPENJSON(Error_Report.[at]) 
	WITH
	(
		error_cd varchar(15),
		error_msg varchar(255),
		pos varchar(2),
		sply_ty varchar(25),
		itms nvarchar(max) as JSON
	) As [AT]
	Cross Apply OPENJSON([At].itms) 
	WITH
	(	
		rt decimal(18,2),
        ad_amt decimal(18,2),
		iamt decimal(18,2),
	    csamt decimal(18,2)
	) As Itms

	-- Output the Result Set

	Select * From #TBL_GSTR1_RETSTATUS_B2B
	Select * From #TBL_GSTR1_RETSTATUS_B2CL
	Select * From #TBL_GSTR1_RETSTATUS_B2CS
	Select * From #TBL_GSTR1_RETSTATUS_CDNR
	Select * From #TBL_GSTR1_RETSTATUS_EXP
	Select * From #TBL_GSTR1_RETSTATUS_HSN
	Select * From #TBL_GSTR1_RETSTATUS_NIL
	Select * From #TBL_GSTR1_RETSTATUS_TXP
	Select * From #TBL_GSTR1_RETSTATUS_AT

	Return 0


End