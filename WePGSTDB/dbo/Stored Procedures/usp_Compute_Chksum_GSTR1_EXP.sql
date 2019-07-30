
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Compute Checksum for GSTR1 EXP Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Compute_Chksum_GSTR1_EXP '12BNAPG9891G2Z9','062017'
 */

CREATE PROCEDURE [usp_Compute_Chksum_GSTR1_EXP] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@Flag varchar(1) = 'U'

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	Declare @InvId int,
			@Inum varchar(50),
			@Idt varchar(50),
			@Val dec(18,2),
			@Sbpcode varchar(6),
			@Sbnum int,
			@Sbdt varchar(50),
			@ItmsId int,
			@Iamt dec(18,2),
			@Rt dec(18,2),
			@Txval dec(18,2)
		
	DECLARE @TBL_Values TABLE 
	(
		Value int
	)

	Declare @StrBuf nvarchar(MAX),
			@ChkSum nvarchar(MAX)

	Select @Delimiter = ','
	
	Insert Into @TBL_Values
	Select	Value 
	From string_split( @RefIds,@Delimiter) 


	Select	t3.invid,inum,idt,val,
			sbpcode,sbnum,sbdt,
			space(64) as chksum,
			t4.itmsid,
			iamt,rt,txval
	Into #TBL_GSTR1_EXP
	From TBL_GSTR1 t1,
		TBL_GSTR1_EXP t2,
		TBL_GSTR1_EXP_INV t3,
		TBL_GSTR1_EXP_INV_ITMS t4
	Where t1.gstin = @Gstin
	And t1.fp = @Fp
	And t2.gstr1id = t1.gstr1id
	And t3.expid = t2.expid
	And t3.invid In (Select Value From @TBL_Values)
	And t3.flag = @Flag
	And t4.invid = t3.invid

	if Exists(Select 1 From #TBL_GSTR1_EXP )
	Begin

		Declare InvCur Insensitive Cursor for
		Select Distinct invid,inum,idt,val,sbpcode,sbnum,sbdt   
		From #TBL_GSTR1_EXP 
		For Read Only

		Open InvCur
		Fetch InvCur Into @InvId,@Inum,@Idt,@Val,@Sbpcode,@Sbnum,@Sbdt
		  
		While @@Fetch_Status = 0
		Begin

			Set @StrBuf='idt=' + Convert(Varchar,IsNull(@Idt,'')) + ',' +
						'inum=' + Convert(Varchar,IsNull(@Inum,'')) + ',' +
						'itms=' 

			Declare InvItmCur Insensitive Cursor for
			Select itmsid   
			From #TBL_GSTR1_EXP 
			Where invid = @InvId 
			For Read Only

			Open InvItmCur
			Fetch InvItmCur Into @ItmsId  
			While @@Fetch_Status = 0
			Begin
		
				Select	@Iamt = iamt,
						@Rt = rt,
						@Txval = txval
				From #TBL_GSTR1_EXP 
				Where invid = @InvId
			
				If @@RowCount = 1
				Begin
					Set @StrBuf = @StrBuf + 
								'iamt=' + Convert(Varchar,IsNull(@Iamt,0)) + ',' +
								'rt=' + Convert(Varchar,IsNull(@Rt,0)) + ',' +
								'txval=' + Convert(Varchar,IsNull(@Txval,0)) 
				End

				Set @StrBuf = @StrBuf + ','

				Fetch InvItmCur Into @ItmsId
			End
			Close InvItmCur
			Deallocate InvItmCur

			Set @StrBuf = @StrBuf +
						'sbdt=' + Convert(Varchar,IsNull(@Sbdt,'')) + ',' + 
						'sbnum=' + Convert(Varchar,IsNull(@Sbnum,0)) + ',' +
						'sbpcode=' + Convert(Varchar,IsNull(@Sbpcode,'')) + ',' + 
						'val=' + Convert(Varchar,IsNull(@Val,0)) 

			Set @ChkSum = Convert(UniqueIdentifier,Hashbytes('SHA2_256',@StrBuf))

			Update #TBL_GSTR1_EXP
			Set chksum = @ChkSum
			Where invid = @InvId
							
			Fetch InvCur Into @InvId,@Inum,@Idt,@Val,@Sbpcode,@Sbnum,@Sbdt
   
		End
		Close InvCur
		Deallocate InvCur

		Update TBL_GSTR1_EXP_INV 
		SET TBL_GSTR1_EXP_INV.chksum = t2.chksum 
		FROM	TBL_GSTR1_EXP_INV t1,	
				#TBL_GSTR1_EXP t2
		WHERE t1.invid = t2.invid 
	

	End
		

	Return 0

End