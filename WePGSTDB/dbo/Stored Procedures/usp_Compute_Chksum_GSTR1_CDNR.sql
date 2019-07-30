
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Compute Checksum for GSTR1 CDNR Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Compute_Chksum_GSTR1_CDNR '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Compute_Chksum_GSTR1_CDNR] 
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
			@Nt_Num varchar(50),
			@Nt_Dt varchar(50),
			@Ntty varchar(1),
			@P_Gst varchar(1),
			@Rsn varchar(50),
			@ItmsId int,
			@Num int,
			@Rt dec(18,2),
			@Txval dec(18,2),
			@Iamt dec(18,2),
			@Camt dec(18,2),
			@Samt dec(18,2),
			@Csamt dec(18,2)
		

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

	Select	t3.ntid as invid,inum,idt,val,
			nt_num,nt_dt,ntty,p_gst,rsn,
			space(64) as chksum,
			t4.itmsid,num,
			rt,txval,iamt,camt,samt,csamt
	Into #TBL_GSTR1_CDNR
	From TBL_GSTR1 t1,
		TBL_GSTR1_CDNR t2,
		TBL_GSTR1_CDNR_NT t3,
		TBL_GSTR1_CDNR_NT_ITMS t4,
		TBL_GSTR1_CDNR_NT_ITMS_DET t5
	Where t1.gstin = @Gstin
	And t1.fp = @Fp
	And t2.gstr1id = t1.gstr1id
	And t3.cdnrid = t2.cdnrid
	And t3.ntid In (Select Value From @TBL_Values)
	And t3.flag = @Flag
	And t4.ntid = t3.ntid
	And t5.itmsid = t4.itmsid

	if Exists(Select 1 From #TBL_GSTR1_CDNR )
	Begin

		Declare InvCur Insensitive Cursor for
		Select Distinct invid,inum,idt,val,nt_num,nt_dt,ntty,p_gst,rsn   
		From #TBL_GSTR1_CDNR 
		For Read Only

		Open InvCur
		Fetch InvCur Into @InvId,@Inum,@Idt,@Val,@Nt_Num,@Nt_Dt,@Ntty,@P_Gst,@Rsn  
		While @@Fetch_Status = 0
		Begin

			Set @StrBuf='idt=' + Convert(Varchar,IsNull(@Idt,'')) + ',' +
						'inum=' + Convert(Varchar,IsNull(@Inum,'')) + ',' +
						'itms=' 

			Declare InvItmCur Insensitive Cursor for
			Select itmsid,num   
			From #TBL_GSTR1_CDNR 
			Where invid = @InvId 
			For Read Only

			Open InvItmCur
			Fetch InvItmCur Into @ItmsId,@Num  
			While @@Fetch_Status = 0
			Begin
		
				Select	@Rt = rt,
						@Txval = txval,
						@Iamt = iamt,
						@Camt = camt,
						@Samt = samt,
						@Csamt = csamt
				From #TBL_GSTR1_CDNR 
				Where invid = @InvId
				And num = @Num

				If @@RowCount = 1
				Begin
					Set @StrBuf = @StrBuf + 'num=' +  Convert(Varchar,IsNull(@Num,'')) + ',' + 
								'itm_det='  +
								'iamt=' + Convert(Varchar,IsNull(@Iamt,0)) + ',' +
								'camt=' + Convert(Varchar,IsNull(@Camt,0)) + ','  +
								'samt=' + Convert(Varchar,IsNull(@Samt,0)) + ',' +
								'csamt=' + Convert(Varchar,IsNull(@Csamt,0)) + ',' +
								'rt=' + Convert(Varchar,IsNull(@Rt,0)) + ',' +
								'txval=' + Convert(Varchar,IsNull(@Txval,0)) 
				End

				Set @StrBuf = @StrBuf + ','

				Fetch InvItmCur Into @ItmsId,@Num  
			End
			Close InvItmCur
			Deallocate InvItmCur

			Set @StrBuf = @StrBuf +
						'nt_num=' + Convert(Varchar,IsNull(@Nt_Num,'')) + ',' +
						'nt_dt=' + Convert(Varchar,IsNull(@Nt_Dt,'')) + ',' +
					    'ntty=' + IsNull(@Ntty,'') + ',' +
						'p_gst = ' + IsNull(@P_Gst,'') + ',' +
						'rsn=' + Convert(Varchar,IsNull(@Rsn,'')) + ',' + 
						'val=' + Convert(Varchar,IsNull(@Val,0)) 

			Set @ChkSum = Convert(UniqueIdentifier,Hashbytes('SHA2_256',@StrBuf))

			Update #TBL_GSTR1_CDNR
			Set chksum = @ChkSum
			Where invid = @InvId
							
			Fetch InvCur Into @InvId,@Inum,@Idt,@Val,@Nt_Num,@Nt_Dt,@Ntty,@P_Gst,@Rsn  
  
		End
		Close InvCur
		Deallocate InvCur

		Update TBL_GSTR1_CDNR_NT 
		SET TBL_GSTR1_CDNR_NT.chksum = t2.chksum 
		FROM	TBL_GSTR1_CDNR_NT t1,	
				#TBL_GSTR1_CDNR t2
		WHERE t1.ntid = t2.invid 
	

	End
		
	Return 0

End