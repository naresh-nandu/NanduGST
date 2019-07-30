
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert CDNR Invoice Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Insert_Outward_GSTR1_CDNR_EXT 
 */

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_CDNR_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Ctin varchar(15),
	@Cfs varchar(1),
	@Ntty varchar(1),
	@Nt_Num varchar(50),
	@Nt_Dt varchar(50),
	@Inum varchar(50),
	@Idt varchar(50),
	@Val decimal(18,2),
	@Pos varchar(2),
	@Rt decimal(18,2),
	@Txval decimal(18,2),
	@Iamt decimal(18,2),
	@Camt decimal(18,2),
	@Samt decimal(18,2),
	@Csamt	 decimal(18,2),
	@ReferenceNo varchar(50),
	@HsnCode	varchar(50),
	@HsnDesc	varchar(250),
	@Qty	decimal(18,2),
	@UnitPrice decimal(18,2),
	@Discount decimal(18,2),
	@Uqc varchar(50),
	@BuyerId int,
	@CreatedBy int,
	@Addinfo varchar(MAX),
	@p_gst varchar(1),
	@rsn varchar(50),
	@RetInum varchar(50)= null Out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15),
			@InvCount varchar(10),
			@TmpStr varchar(10),
			@InvNo varchar(50)

	Declare @CustId int,
			@UniqueRefInd char(1)

	Declare @Gstinid int, @Gstr1id int, @Cdnrid int, @Ntid int, @Flag varchar(1)



	Select @SourceType = 'Manual'

	if @Nt_Num = 'NA'
	Begin
		Select @InvCount= convert(varchar(10),count(distinct(nt_num))+1) 
		From TBL_EXT_GSTR1_CDNR 
		Where gstin = @Gstin 
		And sourcetype = @SourceType and nt_num like 'CDNT%CD%'

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
		Select @InvNo = 'CDNT' + @TmpStr  + @UniqueRefInd + 'CD' +  @InvCount
	End
	Else
	Begin
		Select @InvNo = @Nt_Num
	End

	Begin Try

		Select @Gstinid = t1.gstinid,
			   @Gstr1id = t1.gstr1id,
			   @Cdnrid = t2.cdnrid,
			   @Ntid = ntid,	
			   @Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_CDNR t2,
			 TBL_GSTR1_CDNR_NT  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.cdnrid = t3.cdnrid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.nt_num = @InvNo
		And t3.nt_dt = @Nt_dt

		If IsNull(@Flag,'') <> ''
		Begin
			Select @RetInum = -2 -- Item details cannot be inserted.
			Return
		End
	
		Insert into TBL_EXT_GSTR1_CDNR
		(	gstin, fp, gt, cur_gt, ctin,cfs,
			ntty,nt_num,nt_dt,inum, idt, val,pos, 
			rt, txval, iamt, camt, samt, csamt,
			rowstatus, sourcetype, referenceno, createddate,
			hsncode,hsndesc,qty,unitprice,discount,
			buyerid,uqc,createdby,Addinfo,p_gst,rsn)
		Values(	UPPER(@Gstin), @Fp, null, null,UPPER(@Ctin),@Cfs,
				@Ntty,@InvNo,@Nt_Dt,@Inum, @Idt,@Val,@Pos,
				@Rt,@Txval,@Iamt,@Camt,@Samt,@Csamt,
				1 ,@SourceType,@ReferenceNo,GetDate(),
				@HsnCode,@HsnDesc,@Qty,@UnitPrice,@Discount,
				@Buyerid,@Uqc,@CreatedBy,@Addinfo,@p_gst,@rsn)

		if @@rowcount = 1 
		Begin

			Update TBL_EXT_GSTR1_CDNR
			Set rowstatus = 1
			Where nt_num = @InvNo
			And nt_dt = @Nt_Dt
			And sourcetype = @SourceType

			if IsNull(@Flag,'') = ''
			Begin

				Delete From TBL_GSTR1_CDNR_NT_ITMS_DET
				Where itmsid In (Select itmsid From TBL_GSTR1_CDNR_NT_ITMS
								 Where ntid = @Ntid)

				Delete From TBL_GSTR1_CDNR_NT_ITMS
				Where ntid = @Ntid

				Delete From TBL_GSTR1_CDNR_NT
				Where ntid = @Ntid

			End


		End

		
	End Try
	Begin Catch
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select 'CDNR -' + error_message()
		End				
	End Catch 

	Select @RetInum = @InvNo

	Return 0

End