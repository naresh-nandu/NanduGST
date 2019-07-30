
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Save Manual Entry of GSTR2 B2BUR Inward details
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_INWARD_GSTR2_B2BUR_EXT 
 */
 
CREATE PROCEDURE [usp_Insert_INWARD_GSTR2_B2BUR_EXT]
	
	@gstin   varchar(15),
	@fp   varchar(10),
	@inum   varchar(50),
	@idt   varchar(50),
	@val   decimal(18, 2),
	@cname   varchar(255),
	@rt   decimal(18, 2),
	@txval   decimal(18, 2),
	@iamt	decimal(18,2),
	@camt   decimal(18, 2),
	@samt   decimal(18, 2),
	@csamt   decimal(18, 2),
	@referenceno   varchar(50),
	@hsncode   varchar(50),
	@hsndesc   varchar(255),
	@uqc   varchar(50),
	@qty   decimal(18, 2),
	@unitprice   decimal(18, 2),
	@discount   decimal(18, 2),
	@createdby   int,
	@supplierid   int
	


-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	
	--print @InvoiceNo

	Insert TBL_EXT_GSTR2_B2BUR_INV
	(gstin,	fp,	inum,idt,val,cname,rt,txval,camt,samt,csamt,rowstatus,sourcetype,referenceno,	
	createddate,hsncode,hsndesc,uqc,qty,unitprice,discount,createdby,supplierid,iamt
	)
	values(@gstin,@fp,@inum,@idt,@val,@cname,@rt,@txval,@camt,@samt,@csamt,1,'Manual',@referenceno,
		getdate(),@hsncode,@hsndesc,@uqc,@qty,@unitprice,@discount,@createdby,@supplierid,@iamt)

End