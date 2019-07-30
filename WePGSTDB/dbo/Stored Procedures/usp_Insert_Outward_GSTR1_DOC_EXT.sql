
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert Doc Invoice Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
12/12/2017	Karthik		Initial Version


*/

/* Sample Procedure Call

exec usp_Insert_Outward_GSTR1_DOC_EXT 
 */

CREATE PROCEDURE [usp_Insert_Outward_GSTR1_DOC_EXT]
	@Gstin varchar(15),
	@Fp varchar(10),
	@doc_num int,
	@doc_typ varchar(50)=NULL,
	@num int,
	@froms varchar(50),
	@tos varchar(50),
	@totnum int,
	@cancel int,
	@net_issue int,
	@ReferenceNo varchar(50),
	@CreatedBy int,
	@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @Gstinid int, @Gstr1id int, @docissueid int, @docsid int, @Flag varchar(1)	
	Begin Try

		Select  @Gstinid = t1.gstinid,
				@Gstr1id = t1.gstr1id,
				@docissueid = t2.docissueid,
				@Flag = flag
		FROM TBL_GSTR1 t1,
				TBL_GSTR1_DOC_ISSUE t2 
		Where t2.gstr1id = t1.gstr1id
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t2.doc_num = @doc_num

		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be inserted.
			Return
		End

		Insert into TBL_EXT_GSTR1_DOC
		(	gstin, fp, gt, cur_gt, doc_num,doc_typ, num, froms,tos,totnum,cancel,net_issue,
			rowstatus, sourcetype, referenceno, createddate,createdby)
		Values(	UPPER(@Gstin), @Fp, null, null,@doc_num,@doc_typ,@num, @froms,@tos,@totnum,@cancel,@net_issue,
				1 ,'Manual',@ReferenceNo,GetDate(),@CreatedBy)	

		if @@rowcount = 1 
		Begin
		if IsNull(@Flag,'') = ''
			Begin

				Delete From TBL_GSTR1_DOCs
				Where docissueid = @docissueid

				Delete From TBL_GSTR1_DOC_ISSUE
				Where docissueid = @docissueid

			End
					   
		End
		Select @Retval = 1 -- Doc issue data save successfully
		Return
	End Try
	Begin Catch
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select 'Doc -' + error_message()
		End				
	End Catch 
	Return 0

End