

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert Outward B2CS Records in External Table
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/20/2017	Karthik		Initial Version
11/29/2017	Seshadri	Fine tuned the code
12/18/2017	Seshadri	Fixed the issue of invoice details not getting reflected
						in the consolidated B2CS dataset of SA area

*/

/* Sample Procedure Call  

exec usp_Insert_Outward_GSTR1_B2CS_INV_EXT

 */
 

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_B2CS_INV_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Inum varchar(50),
	@Idt varchar(50),
	@Val decimal(18,2),
	@Pos varchar(2),
	@Etin varchar(15),
	@Rt decimal(18,2),
	@Txval decimal(18,2),
	@Iamt decimal(18,2),
	@Camt decimal(18,2),
	@Samt decimal(18,2),
	@Csamt	 decimal(18,2),
	@ReferenceNo varchar(50),
	@HsnCode	varchar(50),
	@HsnDesc	varchar(50),
	@Qty	decimal(18,2),
	@UnitPrice decimal(18,2),
	@Discount decimal(18,2),
	@Uqc varchar(50),
	@BuyerId int,
	@CreatedBy int,
	@Addinfo varchar(MAX),
	@RetInum varchar(50)= null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15),
			@InvCount varchar(10),
			@TmpStr varchar(10),
			@InvNo varchar(50),
			@CustId int,
			@UniqueRefInd char(1)

	Declare @Gstinid int, @Gstr1id int, @B2csid int, @Invid int, @Flag varchar(1)
	Declare @PlaceOfSupply varchar(2), @SupplyType varchar(5), @InvoiceValue dec(18,2)
	
	Declare @TmpPos varchar(2), @Sply_Typ varchar(5), @Typ varchar(2) 
		
	Select @SourceType = 'Manual'
	Select @TmpPos= convert(varchar(2),substring(@Gstin,1,2))

	if @Pos = @TmpPos
	Begin
		Set @Sply_Typ = 'INTRA'
	End
	Else
	Begin
		Set @Sply_Typ = 'INTER'
	End

	If Ltrim(Rtrim(IsNull(@Etin,''))) <> ''
	Begin
		Set @typ = 'E'
	End
	Else
	Begin
		Set @typ = 'OE'
	End

	if @Inum = 'NA'
	Begin

		Select @InvCount= convert(varchar(10),count(distinct(inum))+1) 
		From TBL_EXT_GSTR1_B2CS_INV 
		Where gstin = @Gstin 
		And sourcetype = @SourceType and inum like 'INV%CS%'

		Select @CustId = Custid 
		from userlist
		Where userid = @CreatedBy

		Select gstinno,
		Row_Number()  OVER(order by gstinno ) as rowno,
		'' as refno
		Into #CustGstin
		From Tbl_Cust_Gstin
		Where custid = @Custid

		Update #CustGstin
		set refno =
		Case rowno
			When 1 Then 'A'
			When 2 Then 'B'
			When 3 Then 'C'
			When 4 Then 'D'
			When 5 Then 'E'
			When 6 Then 'F'
			When 7 Then 'G'
			When 8 Then 'H'
			When 9 Then 'I'
			When 10 Then 'J'
			When 11 Then 'K'
			When 12 Then 'L'
			When 13 Then 'M'
			When 14 Then 'N'
			When 15 Then 'O'
			When 16 Then 'P'
			When 17 Then 'Q'
			When 18 Then 'R'
			When 19 Then 'S'
			When 20 Then 'T'
			When 21 Then 'U'
			When 22 Then 'V'
			When 23 Then 'W'
			When 24 Then 'X'
			When 25 Then 'Y'
			When 26 Then 'Z'
			When 27 Then '0'
			When 28 Then '1'
			When 29 Then '2'
			When 30 Then '3'
			When 31 Then '4'
			When 32 Then '5'
			When 33 Then '6'
			When 34 Then '7'
			When 35 Then '8'
			When 36 Then '9'
			Else 'A'
		End

		Select @UniqueRefInd = IsNull(refno,'A')
		From #CustGstin
		Where gstinno = @Gstin

		Select @InvCount = convert(varchar(10),FORMAT(convert(int,@InvCount),'000000'))
		Select @TmpStr = Substring(@Gstin, 13, 15)

		Select @InvNo = 'INV' + @TmpStr + @UniqueRefInd + 'CS' +  @InvCount

	End
	Else
	Begin
		Select @InvNo = @Inum
	End
	
	Begin Try

		Select  @Gstinid = t1.gstinid,
				@Gstr1id = t1.gstr1id,
				@B2CSid = t2.b2csid,
				@PlaceOfSupply = t2.pos,
				@SupplyType = t3.sply_ty,
				@InvoiceValue = t3.val,
				@Invid = invid,
				@Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CS_N t2,
			TBL_GSTR1_B2CS_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2csid = t3.b2csid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @InvNo
		And t3.idt = @Idt

		If IsNull(@Flag,'') <> ''
		Begin
			Select @RetInum = -2 -- Item details cannot be inserted.
			Return
		End

		Insert into TBL_EXT_GSTR1_B2CS_INV
		(	gstin, fp,sply_typ,typ, gt, cur_gt, pos,inum, idt, val, etin,  
			rt, txval, iamt, csamt,
			rowstatus, sourcetype, referenceno,createddate,
			hsncode,hsndesc,qty,unitprice,discount,
			buyerid,uqc,createdby,
			camt,samt,Addinfo)
		Values(	UPPER(@Gstin), @Fp,@sply_typ,@typ, null, null,@Pos,@InvNo, @Idt, @Val,UPPER(@Etin), 
				@Rt, @Txval, @Iamt,@Csamt,
				1 ,@SourceType,@ReferenceNo,GetDate(),
				@HsnCode,@HsnDesc,@Qty,@UnitPrice,@Discount,
				@Buyerid,@Uqc,@CreatedBy,
				@Camt,@Samt,@Addinfo)

		if @@rowcount = 1 
		Begin
			
			Update TBL_EXT_GSTR1_B2CS_INV
			Set val =  @Val,
				rowstatus = 1
			Where inum = @InvNo
			And idt = @Idt
			And sourcetype = @SourceType
			
			
			if IsNull(@Flag,'') = ''
			Begin

				-- B2CS Consolidated DataSet Updation (Begin)

				Select rt,txval,iamt,camt,samt,csamt
				Into #Tbl_Inv_Itms
				From TBL_GSTR1_B2CS_INV_ITMS_DET
				Where itmsid in (Select itmsid From TBL_GSTR1_B2CS_INV_ITMS
								 Where invid = @Invid)

				if Exists(Select 1 From #Tbl_Inv_Itms)
				Begin

					Update TBL_GSTR1_B2CS 
					Set	txval = IsNull(t2.txval,0) - IsNull(t3.txval,0), 
						iamt = IsNull(t2.iamt,0) - IsNull(t3.iamt,0),
						camt = IsNull(t2.camt,0) - IsNull(t3.camt,0),
						samt = IsNull(t2.samt,0) - IsNull(t3.samt,0), 
						csamt = IsNull(t2.csamt,0) - IsNull(t3.csamt,0) 
					From TBL_GSTR1 t1,
						 TBL_GSTR1_B2CS  t2,
						 #Tbl_Inv_Itms t3
					Where t2.gstr1id = t1.gstr1id
					And t3.rt = t2.rt
					And t1.gstin = @Gstin
					And t1.fp = @Fp
					And t2.pos = @PlaceOfSupply
					And t2.sply_ty = @SupplyType	


					Delete TBL_GSTR1_B2CS 
					From TBL_GSTR1 t1,
						 TBL_GSTR1_B2CS  t2,
						 #Tbl_Inv_Itms t3
					Where t2.gstr1id = t1.gstr1id
					And t3.rt = t2.rt
					And t1.gstin = @Gstin
					And t1.fp = @Fp
					And t2.pos = @PlaceOfSupply
					And t2.sply_ty = @SupplyType	
					And IsNull(t2.txval,0) = 0
					And IsNull(t2.iamt,0) = 0
					And IsNull(t2.camt,0) = 0
					And IsNull(t2.samt,0) = 0
					And IsNull(t2.csamt,0) = 0

				End

				-- B2CS Consolidated DataSet Updation (End)


				Delete From TBL_GSTR1_B2CS_INV_ITMS_DET
				Where itmsid In (Select itmsid From TBL_GSTR1_B2CS_INV_ITMS
								 Where invid = @Invid)

				Delete From TBL_GSTR1_B2CS_INV_ITMS
				Where invid = @Invid

				Delete From TBL_GSTR1_B2CS_INV
				Where invid = @Invid

			End
					   
		End

		
	End Try
	Begin Catch
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select 'B2CS -' + error_message()
		End				
	End Catch 

	Select @RetInum = @InvNo

	Return 0

End