
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Compute Checksum for GSTR1 B2B Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Compute_Chksum_GSTR1_B2B '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Compute_Chksum_GSTR1_B2B] 
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
			@Idt varchar(50),
			@Inum varchar(50),
			@InvTyp varchar(5),
			@ItmsId int,
			@Num int,
			@Camt dec(18,2),
			@Csamt dec(18,2),
			@Iamt dec(18,2),
			@Rt dec(18,2),
			@Samt dec(18,2),
			@Txval dec(18,2),
			@Pos varchar(2),
			@Rchrg varchar(1),
			@Val dec(18,2)

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


	Select	t3.invid,idt,inum,inv_typ,
			space(64) as chksum,
			t4.itmsid,num,
			camt,csamt,iamt,rt,samt,txval,
			pos,rchrg,val
	Into #TBL_GSTR1_B2B
	From TBL_GSTR1 t1,
		TBL_GSTR1_B2B t2,
		TBL_GSTR1_B2B_INV t3,
		TBL_GSTR1_B2B_INV_ITMS t4,
		TBL_GSTR1_B2B_INV_ITMS_DET t5
	Where t1.gstin = @Gstin
	And t1.fp = @Fp
	And t2.gstr1id = t1.gstr1id
	And t3.b2bid = t2.b2bid
	And t3.invid In (Select Value From @TBL_Values)
	And t3.flag = @Flag
	And t4.invid = t3.invid
	And t5.itmsid = t4.itmsid

	if Exists(Select 1 From #TBL_GSTR1_B2B )
	Begin

		Declare InvCur Insensitive Cursor for
		Select Distinct invid,idt,inum,inv_typ,pos,rchrg,val   
		From #TBL_GSTR1_B2B 
		For Read Only

		Open InvCur
		Fetch InvCur Into @InvId,@Idt,@Inum,@InvTyp,@Pos,@Rchrg,@Val  
		While @@Fetch_Status = 0
		Begin

			Set @StrBuf='idt=' + Convert(Varchar,IsNull(@Idt,'')) + ',' +
						'inum=' + Convert(Varchar,IsNull(@Inum,'')) + ',' +
						'inv_typ = ' + @InvTyp + ',' +
						'itms=' 

			Declare InvItmCur Insensitive Cursor for
			Select itmsid,num   
			From #TBL_GSTR1_B2B 
			Where invid = @InvId 
			For Read Only

			Open InvItmCur
			Fetch InvItmCur Into @ItmsId,@Num  
			While @@Fetch_Status = 0
			Begin
		
				Select	@Camt = camt,
						@Csamt = csamt,
						@Iamt = iamt,
						@Samt = samt,
						@Rt = rt,
						@Txval = txval
				From #TBL_GSTR1_B2B 
				Where invid = @InvId
				And num = @Num

				If @@RowCount = 1
				Begin
					Set @StrBuf = @StrBuf + 'num=' +  Convert(Varchar,IsNull(@Num,'')) + ',' + 
								'itm_det='  +
								'camt=' + Convert(Varchar,IsNull(@Camt,0)) + ','  +
								'csamt=' + Convert(Varchar,IsNull(@Csamt,0)) + ',' +
								'iamt=' + Convert(Varchar,IsNull(@Iamt,0)) + ',' +
								'samt=' + Convert(Varchar,IsNull(@Samt,0)) + ',' +
								'rt=' + Convert(Varchar,IsNull(@Rt,0)) + ',' +
								'txval=' + Convert(Varchar,IsNull(@Txval,0)) 
				End

				Set @StrBuf = @StrBuf + ','

				Fetch InvItmCur Into @ItmsId,@Num  
			End
			Close InvItmCur
			Deallocate InvItmCur

			Set @StrBuf = @StrBuf + 'pos=' + @Pos + ',' +
						'rchrg = ' + @Rchrg + ',' +
						'val=' + Convert(Varchar,IsNull(@Val,0)) 

			Set @ChkSum = Convert(UniqueIdentifier,Hashbytes('SHA2_256',@StrBuf))

			Update #TBL_GSTR1_B2B
			Set chksum = @ChkSum
			Where invid = @InvId
							
			Fetch InvCur Into @InvId,@Idt,@Inum,@InvTyp,@Pos,@Rchrg,@Val    
		End
		Close InvCur
		Deallocate InvCur

		Update TBL_GSTR1_B2B_INV 
		SET TBL_GSTR1_B2B_INV.chksum = t2.chksum 
		FROM	TBL_GSTR1_B2B_INV t1,	
				#TBL_GSTR1_B2B t2
		WHERE t1.invid = t2.invid 
	

	End
		

	Return 0

End