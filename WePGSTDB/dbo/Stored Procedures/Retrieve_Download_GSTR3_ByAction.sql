-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>

-- exec [Retrieve_Download_GSTR3_ByAction] 'REFCLM','04AABFN9870CMZT'
-- =============================================
CREATE PROCEDURE [Retrieve_Download_GSTR3_ByAction] 
	-- Add the parameters for the stored procedure here
	@ActionType nvarchar(20),
	@GSTINNo nvarchar(15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @@GSTINNo varchar(15) = (SELECT GSTINNo FROM TBL_Cust_GSTIN WHERE GSTINId= @GSTINId)
    -- Insert statements for procedure here
	IF @ActionType = 'ISUP_IMP'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.impdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type', iamt as 'IGST Amount', csamt as 'CGST Amount' from TBL_GSTR3_D_ISUP_IMP_DET as t1 inner join  TBL_GSTR3_D_ISUP_IMP as t2 on t1.impid=t2.impid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_INTER'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.interdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value',t1.state_cd as 'State Code', iamt as 'IGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_ISUP_INTER_DET as t1 inner join  TBL_GSTR3_D_ISUP_INTER as t2 on t1.interid=t2.interid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_INTRA'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.intradetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value', camt as 'CGST Amount',samt as 'SGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_ISUP_INTRA_DET as t1 inner join  TBL_GSTR3_D_ISUP_INTRA as t2 on t1.intraid=t2.intraid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_ITCREV'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.itcrevdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',num as 'Number',iamt as 'IGST Amount', camt as 'CGST Amount',samt as 'SGST Amount' from TBL_GSTR3_D_ISUP_ITCREV_DET as t1 inner join  TBL_GSTR3_D_ISUP_ITCREV as t2 on t1.itcrevid=t2.itcrevid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_JBWRK'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.jbwrkdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',iamt as 'IGST Amount', camt as 'CGST Amount',samt as 'SGST Amount' from TBL_GSTR3_D_ISUP_JBWRK_DET as t1 inner join  TBL_GSTR3_D_ISUP_JBWRK as t2 on t1.jbwrkid=t2.jbwrkid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_JBWRK'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.revinvdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',ty as 'Type', docty as 'Doc Type',val as 'Value',iamt as 'IGST Amount', camt as 'CGST Amount',csamt as 'CESS Amount' from TBL_GSTR3_D_ISUP_REVINV_DET as t1 inner join  TBL_GSTR3_D_ISUP_REVINV as t2 on t1.revinvid=t2.revinvid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ISUP_TTXLIAB'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.ttxliabdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',ty as 'Type', txval as 'Tax Value',iamt as 'IGST Amount', camt as 'CGST Amount',samt as 'SGST Amount',csamt as 'CESS Amount' from TBL_GSTR3_D_ISUP_TTXLIAB_DET as t1 inner join  TBL_GSTR3_D_ISUP_TTXLIAB as t2 on t1.ttxliabid=t2.ttxliabid inner join TBL_GSTR3_D_ISUP as t3 on t2.isupid = t3.isupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'ITCCR'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.itccrdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',ty as 'Type', irt as 'IGST Rate', crt as 'CGST Rate',srt as 'SGST Rate',csrt as 'CESS Rate' from TBL_GSTR3_D_ITCCR_DET as t1 inner join  TBL_GSTR3_D_ITCCR as t2 on t1.itccrid=t2.itccrid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_EXP'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.expdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value', iamt as 'IGST Amount', cess as 'CESS Amount' from TBL_GSTR3_D_OSUP_EXP_DET as t1 inner join  TBL_GSTR3_D_OSUP_EXP as t2 on t1.expid=t2.expid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_INTER'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.interdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value',t1.state_cd as 'State Code', iamt as 'IGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_OSUP_INTER_DET as t1 inner join  TBL_GSTR3_D_OSUP_INTER as t2 on t1.interid=t2.interid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_INTERC'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.intercdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value',t1.state_cd as 'State Code', iamt as 'IGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_OSUP_INTERC_DET as t1 inner join  TBL_GSTR3_D_OSUP_INTERC as t2 on t1.intercid=t2.intercid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_INTRA'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.intradetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value', camt as 'CGST Amount',samt as 'SGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_OSUP_INTRA_DET as t1 inner join  TBL_GSTR3_D_OSUP_INTRA as t2 on t1.intraid=t2.intraid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_INTRAC'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.intracdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value', camt as 'CGST Amount',samt as 'SGST Amount', cess as 'Cess Amount' from TBL_GSTR3_D_OSUP_INTRAC_DET as t1 inner join  TBL_GSTR3_D_OSUP_INTRAC as t2 on t1.intracid=t2.intracid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_REVSUP'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.revsupdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',val as 'Value',iamt as 'IGST Amount', camt as 'CGST Amount',samt as 'SGST Amount',csamt as 'CESS Amount' from TBL_GSTR3_D_OSUP_REVSUP_DET as t1 inner join  TBL_GSTR3_D_OSUP_REVSUP as t2 on t1.revsupid=t2.revsupid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'OSUP_TTXLIAB'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.ttxliabdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',txval as 'Tax Value', camt as 'CGST Amount',samt as 'SGST Amount' from TBL_GSTR3_D_OSUP_TTXLIAB_DET as t1 inner join  TBL_GSTR3_D_OSUP_TTXLIAB as t2 on t1.ttxliabid=t2.ttxliabid inner join TBL_GSTR3_D_OSUP as t3 on t2.osupid = t3.osupid where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'REFCLM'
	BEGIN
		CREATE TABLE #t1_All([S.No] varchar(5) NULL,
						[Check Sum] [varchar](64) NULL,
						[GSTIN No]  [varchar](20) NULL,
						[Period] [varchar](20) NULL,
						[Bank Account][varchar](20) NULL,
						Tax [decimal](18, 2) NULL,
						[Type] varchar(15) NULL)
	insert into #t1_All( [S.No],[Check Sum],	[GSTIN No],	[Period],	[Bank Account],	[Tax],	[Type])
		SELECT 1,'NA', @GSTINNo,re_period , bankacc ,tax,'CGRFCLM'  from TBL_GSTR3_D_REFCLM_CGRFCLM as t1 inner join  TBL_GSTR3_D_REFCLM as t2 on t1.refclmid=t2.refclmid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	insert into #t1_All( [S.No],	[Check Sum],	[GSTIN No],	[Period],	[Bank Account],	[Tax],	[Type])
		SELECT 1,'NA' , @GSTINNo,re_period, bankacc,tax,'CSRFCLM' from TBL_GSTR3_D_REFCLM_CSRFCLM as t1 inner join  TBL_GSTR3_D_REFCLM as t2 on t1.refclmid=t2.refclmid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	insert into #t1_All( [S.No],	[Check Sum],	[GSTIN No],	[Period],	[Bank Account],	[Tax],	[Type])
		SELECT 1,'NA', @GSTINNo,re_period, bankacc,tax,'IGRFCLM' from TBL_GSTR3_D_REFCLM_IGRFCLM as t1 inner join  TBL_GSTR3_D_REFCLM as t2 on t1.refclmid=t2.refclmid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	select ROW_NUMBER() OVER(ORDER BY [S.No] ASC) AS 'S.No',[Check Sum],[GSTIN No],	[Period],[Bank Account],	[Tax],	[Type] from #t1_All
	END
	ELSE IF @ActionType = 'REFCLM_SGRFCLM'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.sgrfclmid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',re_period as 'Period', bankacc as 'Bank Account',tax as 'Tax' from TBL_GSTR3_D_REFCLM_SGRFCLM as t1 inner join  TBL_GSTR3_D_REFCLM as t2 on t1.refclmid=t2.refclmid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TCSCR'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.tcscrdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', irt as 'IGST Rate', crt as 'CGST Rate',srt as 'SGST Rate',csrt as 'CESS Rate' from TBL_GSTR3_D_TCSCR_DET as t1 inner join  TBL_GSTR3_D_TCSCR as t2 on t1.tcscrid=t2.tcscrid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TDSCR'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.tdscrdetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', irt as 'IGST Rate', crt as 'CGST Rate',srt as 'SGST Rate',csrt as 'CESS Rate' from TBL_GSTR3_D_TDSCR_DET as t1 inner join  TBL_GSTR3_D_TDSCR as t2 on t1.tdscrid=t2.tdscrid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TOD'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.toditmid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', gr_to as 'GR To', exp_to as 'Exp To',nil_to as 'Nil To',non_to as 'Non To', net_to as 'Net To'  from TBL_GSTR3_D_TOD_ITM as t1 inner join  TBL_GSTR3_D_TOD as t2 on t1.todid=t2.todid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TPM'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.pdcashid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', debitno as 'Debit No', i_pdint as 'IGST pd Int',c_pdint as 'CGST pd int',s_pdint as 'SGST pd int', cs_pdint as 'CESS pd int'  from TBL_GSTR3_D_TPM_PDCASH as t1 inner join  TBL_GSTR3_D_TPM as t2 on t1.tpmid=t2.tpmid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TPM_TXPBA'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.pdcashid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', debitno as 'Debit No', ipd as 'IGST pd',cpd as 'CGST pd',spd as 'SGST pd', cspd as 'CESS pd'  from TBL_GSTR3_D_TPM_TXPBA_PDCASH as t1 inner join TBL_GSTR3_D_TPM_TXPBA as t2 on t1.txpbaid=t2.txpbaid inner join TBL_GSTR3_D_TPM as t3 on t2.tpmid=t3.tpmid  where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TPM_TXPBB'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.pydetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No',  i_int as 'IGST int',c_int as 'CGST int',s_int as 'SGST int', cs_int as 'CESS int'  from TBL_GSTR3_D_TPM_TXPBB_PYDET as t1 inner join TBL_GSTR3_D_TPM_TXPBB as t2 on t1.txpbbid=t2.txpbbid inner join TBL_GSTR3_D_TPM as t3 on t2.tpmid=t3.tpmid  where t3.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	ELSE IF @ActionType = 'TTXL'
	BEGIN
		SELECT ROW_NUMBER() OVER(ORDER BY t1.ttxldetid ASC) AS 'S.No', @GSTINNo as 'GSTIN No', ty as 'Type',txval as 'Tax Value', iamt as 'IGST Amount',camt as 'CGST Amount',samt as 'SGST Amount', csamt as 'CESS Amount'  from TBL_GSTR3_D_TTXL_DET as t1 inner join  TBL_GSTR3_D_TTXL as t2 on t1.ttxlid=t2.ttxlid  where t2.gstr3dId in (select gstr3dId from TBL_GSTR3_D where gstin =@GSTINNo)
	END
	Else
	Begin
		select 'No Record Found' as 'Message'
	ENd
END