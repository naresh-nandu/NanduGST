
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Compute Checksum for GSTR1 B2CS Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Compute_Chksum_GSTR1_B2CS '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Compute_Chksum_GSTR1_B2CS] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@Flag varchar(1) = 'U'

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	Declare @B2csId int,
			@Sply_Ty varchar(5),
			@Pos varchar(2),
			@Typ varchar(2),
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


	Select	t2.b2csid,
			t2.sply_ty,t2.pos,t2.typ,
			space(64) as chksum,
			iamt,rt,txval
	Into #TBL_GSTR1_B2CS
	From TBL_GSTR1 t1,
		TBL_GSTR1_B2CS t2
	Where t1.gstin = @Gstin
	And t1.fp = @Fp
	And t2.gstr1id = t1.gstr1id
	And t2.b2csid In (Select Value From @TBL_Values)
	And t2.flag = @Flag

	if Exists(Select 1 From #TBL_GSTR1_B2CS )
	Begin

		Declare InvCur Insensitive Cursor for
		Select Distinct b2csid,sply_ty,pos,typ,iamt,rt,txval   
		From #TBL_GSTR1_B2CS
		For Read Only

		Open InvCur
		Fetch InvCur Into @B2csId,@Sply_Ty,@Pos,@Typ,@Iamt,@Rt,@Txval  
		While @@Fetch_Status = 0
		Begin

			Set @StrBuf='iamt=' + Convert(Varchar,IsNull(@Iamt,0)) + ',' +
						'pos=' + IsNull(@Pos,'') + ',' +	
						'rt=' + Convert(Varchar,IsNull(@Rt,0)) + ',' +
						'sply_ty=' + IsNull(@Sply_Ty,'') + ',' +	
						'txval=' + Convert(Varchar,IsNull(@Txval,0)) + ',' +
						'typ=' + IsNull(@Typ,'')
			
			Set @ChkSum = Convert(UniqueIdentifier,Hashbytes('SHA2_256',@StrBuf))

			Update #TBL_GSTR1_B2CS
			Set chksum = @ChkSum
			Where b2csid = @B2csId
							
			Fetch InvCur Into @B2csId,@Sply_Ty,@Pos,@Typ,@Iamt,@Rt,@Txval      
		End
		Close InvCur
		Deallocate InvCur

		Update TBL_GSTR1_B2CS 
		SET TBL_GSTR1_B2CS.chksum = t2.chksum 
		FROM	TBL_GSTR1_B2CS t1,	
				#TBL_GSTR1_B2CS t2
		WHERE t1.b2csid = t2.b2csid 
	

	End
		

	Return 0

End