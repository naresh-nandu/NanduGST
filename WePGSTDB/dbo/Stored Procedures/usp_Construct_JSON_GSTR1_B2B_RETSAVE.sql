
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/12/2017	Seshadri			Initial Version
*/
/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_B2B_RETSAVE '33GSPTN0802G1ZL','012017',991,1240,'U'
 */

CREATE PROCEDURE [usp_Construct_JSON_GSTR1_B2B_RETSAVE] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@MinInvId int,
	@MaxInvId int,
	@Flag varchar(1),
	@JsonOut nvarchar(Max) = NULL OUT,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select (
	Select	gstin,fp,gt,cur_gt,
	(	Select	ctin,
		(	Select inum, idt,val, pos,rchrg,etin,inv_typ,
			(	Select num, JSON_QUERY
						((
							Select rt ,txval,iamt,camt,samt,csamt 
							From TBL_GSTR1_B2B_INV_ITMS_DET 
							Where itmsid = TBL_GSTR1_B2B_INV_ITMS.itmsid
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det
				From TBL_GSTR1_B2B_INV_ITMS
				Where invid = TBL_GSTR1_B2B_INV.invid
				And  TBL_GSTR1_B2B_INV.b2bid = TBL_GSTR1_B2B.b2bid
				And IsNull(TBL_GSTR1_B2B_INV.flag,'') = @Flag
				And  invid between @MinInvId and @MaxInvId 			
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_B2B_INV
			Where b2bid = TBL_GSTR1_B2B.b2bid
			And IsNull(flag,'') = @Flag
			And  invid between @MinInvId and @MaxInvId 			
			FOR JSON PATH	
		) As inv
		From TBL_GSTR1_B2B 
		Where TBL_GSTR1_B2B.gstr1id in 
			(	Select TBL_GSTR1.gstr1id 
				From TBL_GSTR1 
				Where TBL_GSTR1.gstin = @Gstin 
				And TBL_GSTR1.fp = @Fp
			) 
		And TBL_GSTR1_B2B.b2bid in
			(	Select b2bid
				From TBL_GSTR1_B2B_INV
				Where IsNull(flag,'') = @Flag
				And   invid between @MinInvId and @MaxInvId 		
				And
				gstr1id In
				(
					Select TBL_GSTR1.gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)
			)
		Group by TBL_GSTR1_B2B.b2bid,TBL_GSTR1_B2B.ctin 
		FOR JSON PATH
	) As b2b

	From TBL_GSTR1 
	Where gstin = @Gstin 
	And TBL_GSTR1.fp = @fp 
	Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
	FOR JSON AUTO  ) as JsonOut into #JsonOut 
	
	select @JsonOut = JsonOut from #JsonOut
	select @JsonOut -- = '1 pass'

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End