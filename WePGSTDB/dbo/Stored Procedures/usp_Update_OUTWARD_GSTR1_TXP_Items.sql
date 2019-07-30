
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update TXP Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_TXP_Items 
 */
 
CReate PROCEDURE [usp_Update_OUTWARD_GSTR1_TXP_Items]
		@TXPid    int,
		@Mode  varchar(1), -- 'M' 'D'
		@Pos	varchar(2)=NULL,
		@Rt		decimal(18,2)=NULL,
		@Ad_amt	decimal(18,2)=NULL,
		@Iamt	decimal(18,2)=NULL,
		@Camt	decimal(18,2)=NULL,
		@Samt	decimal(18,2)=NULL,
		@Csamt	 decimal(18,2)=NULL,
		@Createdby int,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Select * from TBL_EXT_GSTR1_TXP
	Set Nocount on
	
	Declare @Gstin varchar(15),@Fp varchar(10),@Flag varchar(1),@Nrt decimal(18,2),@Npos varchar(2)
	Declare @TPpos varchar(2),@Supply_Typ varchar(50)
	select @TPpos= convert(varchar(2),substring(@Gstin,1,2))

	if @TPpos <> @POS
	 Begin
		Set @Supply_Typ = 'INTER'
	 End
	 Else
	 Begin
		Set @Supply_Typ = 'INTRA'
	 End
	
	if Exists(Select 1 from TBL_EXT_GSTR1_TXP 
			  Where txpid = @TXPid
			  And rowstatus in (0,1)
			 )
	Begin
	
				Select  @Gstin = gstin,
						@Fp = fp,
						@Nrt = rt,
						@Npos=pos
				From TBL_EXT_GSTR1_TXP
				Where txpid = @TXPid 

				Select @Flag = flag
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_TXP t2,
					 TBL_GSTR1_TXP_ITMS  t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.txpid = t3.txpid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
	
				If IsNull(@Flag,'') <> ''
				Begin
					Select @Retval = -2 -- Item details cannot be updated.
					Return
				End
			
	If @Mode = 'M'
		Begin
				Update TBL_EXT_GSTR1_TXP 
				Set pos = @Pos,
					sply_ty = @Supply_Typ,
					ad_amt = @Ad_amt,
					iamt= @Iamt,
					camt = @Camt,
					samt = @Samt,
					csamt = @Csamt 
				where txpid = @TXPid
					And rt = @Rt
					--And pos = @Pos 
				-- Update the Values in Staging Area

				Update TBL_GSTR1_TXP_ITMS
				SET TBL_GSTR1_TXP_ITMS.ad_amt = @Ad_amt,
					TBL_GSTR1_TXP_ITMS.iamt = @Iamt,
					TBL_GSTR1_TXP_ITMS.camt = @Camt,
					TBL_GSTR1_TXP_ITMS.samt = @Samt,
					TBL_GSTR1_TXP_ITMS.csamt = @Csamt
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_TXP t2,
					 TBL_GSTR1_TXP_ITMS t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.txpid = t3.txpid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t3.rt = @Rt

				Update TBL_GSTR1_TXP
				SET TBL_GSTR1_TXP.pos = @Pos,
					TBL_GSTR1_TXP.sply_ty = @Supply_Typ
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_TXP t2,
					 TBL_GSTR1_TXP_ITMS t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.txpid = t3.txpid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t3.rt = @Rt

				--And t2.pos = @Pos	
	
				Select @Retval = 1 -- Item details updated and also the Invoice Value updated for the entire Invoice
		End
	Else if @Mode = 'D'
		Begin
			Delete from TBL_EXT_GSTR1_TXP Where txpid = @TXPid 

			Delete TBL_GSTR1_TXP_ITMS
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_TXP t2,
					 TBL_GSTR1_TXP_ITMS t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.txpid = t3.txpid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t3.rt = @Nrt
				And t2.pos = @Npos	

				Select @Retval = 2 -- Successfully Deleted

		End
	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End


End