
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert EXP Invoice Details
				
Written by  : nareshn@wepindia.com

Date		Who			Decription 
05/12/2017	Naresh		Initial Version


*/

/* Sample Procedure Call

exec usp_Insert_Outward_GSTR1_EXP_EXT 
 */

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_EXP_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Exp_Typ varchar(15),
	@Inum varchar(50),
	@Idt varchar(50),
	@Val decimal(18,2),
	@sbnum varchar(2),
	@sbdt varchar(10),
	@sbpcode varchar(15),
	@Rt decimal(18,2),
	@Txval decimal(18,2),
	@Iamt decimal(18,2),
	@ReferenceNo varchar(50),
	@HsnCode	varchar(50),
	@HsnDesc	varchar(250),
	@Qty	decimal(18,2),
	@UnitPrice decimal(18,2),
	@Discount decimal(18,2),
	@Uqc varchar(50),
	@CreatedBy int,
	@Addinfo varchar(max),
	@ShippingAddress varchar(Max),
	@BuyerId Int,
	@ReceiverName	varchar(250),
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

	Declare @Gstinid int, @Gstr1id int, @expid int, @Invid int, @Flag varchar(1)

	Select @SourceType = 'Manual'

	if @Inum = 'NA'
	Begin

		Select @InvCount= convert(varchar(10),count(distinct(inum))+1) 
		From TBL_EXT_GSTR1_EXP_INV 
		Where gstin = @Gstin 
		And sourcetype = @SourceType and inum like 'INV%EXP%'

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

		Select @InvNo = 'INV' + @TmpStr + @UniqueRefInd + 'EXP' +  @InvCount

	End
	Else
	Begin
		Select @InvNo = @Inum
	End

	Begin Try

		Select  @Gstinid = t1.gstinid,
				@Gstr1id = t1.gstr1id,
				@expid = t2.expid,
				@Invid = invid,
				@Flag = flag
		FROM TBL_GSTR1 t1,
				TBL_GSTR1_EXP t2,
			TBL_GSTR1_EXP_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.expid = t3.expid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @InvNo
		And t3.idt = @Idt

		If IsNull(@Flag,'') <> ''
		Begin
			Select @RetInum = -2 -- Item details cannot be inserted.
			Return
		End

		Insert into TBL_EXT_GSTR1_EXP_INV
		(	gstin, fp, gt, cur_gt, exp_typ,inum, idt, val, 
			sbnum, sbdt, sbpcode,rt, txval, iamt,
			rowstatus, sourcetype, referenceno, createddate,
			hsncode,hsndesc,qty,unitprice,discount,
			uqc,createdby,Addinfo,ShippingAddress,buyerid,ReceiverName)
		Values(	UPPER(@Gstin), @Fp, null, null, UPPER(@Exp_Typ),@InvNo, @Idt, @Val, 
				@sbnum, @sbdt, @sbpcode,@Rt, @Txval, @Iamt,
				1 ,@SourceType,@ReferenceNo,GetDate(),
				@HsnCode,@HsnDesc,@Qty,@UnitPrice,@Discount,
				@Uqc,@CreatedBy,@Addinfo,@ShippingAddress,@BuyerId,@ReceiverName)	

		if @@rowcount = 1 
		Begin

			Update TBL_EXT_GSTR1_EXP_INV
			Set rowstatus = 1,
				val =  @Val
			Where inum = @InvNo
			And idt = @Idt
			And sourcetype = @SourceType

			if IsNull(@Flag,'') = ''
			Begin

				Delete From TBL_GSTR1_EXP_INV_ITMS
				Where invid = @Invid

				Delete From TBL_GSTR1_EXP_INV
				Where invid = @Invid

			End
					   
		End
		
	End Try
	Begin Catch
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select 'EXP -' + error_message()
		End				
	End Catch 

	Select @RetInum = @InvNo

	Return 0

End