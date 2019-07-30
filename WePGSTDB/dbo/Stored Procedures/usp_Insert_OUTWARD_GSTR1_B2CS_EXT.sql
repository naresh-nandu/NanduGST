
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_OUTWARD_GSTR1_B2CS_EXT 
 */
 
CREATE PROCEDURE [usp_Insert_OUTWARD_GSTR1_B2CS_EXT]
		@gstin    varchar(15),
		@fp    varchar(10),
		@sply_typ varchar(5),
		@txval	decimal(18,2),
		@typ  varchar(2),
		@etin	varchar(15),
		@POS	varchar(2),
		@rt		decimal(18,2),
		@iamt	decimal(18,2),
		@camt	 decimal(18,2),		
		@samt	decimal(18,2),
		@csamt	 decimal(18,2),
		@ReferenceNo	varchar(50),
		@hsncode	varchar(50),
		@hsndesc	varchar(250),
		@qty	decimal(18,2),
		@unitprice decimal(18,2),
		@discount decimal(18,2),
		@uqc varchar(50),
		@Createdby int,
		@Addinfo varchar(MAX)
		
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Insert TBL_EXT_GSTR1_B2CS
				(	gstin, fp, gt, cur_gt, sply_ty,txval,typ,etin, pos,  
					rt, iamt,camt,samt,csamt,
					rowstatus, sourcetype, referenceno, createddate,
					hsncode,hsndesc,qty,unitprice,discount,uqc,createdby,Addinfo)
				values
					(UPPER(@gstin), @fp, null, null,@sply_typ,@txval,@typ,UPPER(@etin), @pos,
					@rt,@iamt,@camt,@samt,@csamt,
					1 ,'Manual' ,@ReferenceNo,GetDate(),
					@hsncode,@hsndesc,@qty,@unitprice,@discount,@uqc,@Createdby,@Addinfo)

End