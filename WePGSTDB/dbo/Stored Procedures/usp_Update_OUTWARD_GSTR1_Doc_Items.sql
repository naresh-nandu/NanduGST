
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update Doc Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_Doc_Items 
 */
 
CREATE PROCEDURE [usp_Update_OUTWARD_GSTR1_Doc_Items]
		@docid    int,
		@Mode  varchar(1), -- 'M' 'D'
		@doc_num	int = NULL,
		@froms	varchar(50)=NULL,
		@tos	varchar(50)=NULL,
		@totnum	int =NULL,
		@cancel	int=NULL,
		@net_issue	 int=NULL,
		@Createdby int,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin


	Set Nocount on
	
	Declare @Gstin varchar(15),@Fp varchar(10),@Flag varchar(1)
	
	if Exists(Select 1 from TBL_EXT_GSTR1_DOC 
			  Where docid = @docid
			  And rowstatus in (0,1)
			 )
	Begin
	
				Select  @Gstin = gstin,
						@Fp = fp
				From TBL_EXT_GSTR1_DOC
				Where docid = @docid 

				Select @Flag = flag
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_DOC_ISSUE t2,
					 TBL_GSTR1_DOCS  t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.docissueid = t3.docissueid
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t2.doc_num = @doc_num
	
				If IsNull(@Flag,'') <> ''
				Begin
					Select @Retval = -2 -- Item details cannot be updated.
					Return
				End
			
	If @Mode = 'M'
		Begin
				Update TBL_EXT_GSTR1_DOC 
				Set froms = @froms,
					tos= @tos,
					totnum = @totnum,
					cancel = @cancel,
					net_issue = @net_issue 
				where docid = @docid					
					And doc_num = @doc_num
					--And pos = @Pos 
				-- Update the Values in Staging Area

				Update TBL_GSTR1_DOCS
				SET TBL_GSTR1_DOCS.[from] = @froms,
					TBL_GSTR1_DOCS.[to] = @tos,
					TBL_GSTR1_DOCS.totnum = @totnum,
					TBL_GSTR1_DOCS.cancel = @cancel,
					TBL_GSTR1_DOCS.net_issue = @net_issue
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_DOC_ISSUE t2,
					 TBL_GSTR1_DOCS t3 
				Where t1.gstr1id = t2.gstr1id
				And t2.docissueid = t3.docissueid
				And t1.gstin = @Gstin
				And t1.fp = @Fp				
				And t2.doc_num = @doc_num

				

				--And t2.pos = @Pos	
	
				Select @Retval = 1 -- Item details updated and also the Invoice Value updated for the entire Invoice
		End
	Else if @Mode = 'D'
		Begin
			Delete from TBL_EXT_GSTR1_DOC Where docid = @docid 

			Delete TBL_GSTR1_DOCS
				FROM TBL_GSTR1 t1,
					 TBL_GSTR1_DOC_ISSUE t2 
				Where t1.gstr1id = t2.gstr1id
				And t1.gstin = @Gstin
				And t1.fp = @Fp				
				And t2.doc_num = @doc_num

				Select @Retval = 2 -- Successfully Deleted

		End
	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End


End