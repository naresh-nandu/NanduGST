
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete OUTWARD_GSTR1_CDNR
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Delete_OUTWARD_GSTR1_B2CL_Items

 */
 
Create PROCEDURE [usp_Delete_OUTWARD_GSTR1_EXT]  
	@ActionType	varchar(50),
	@invitemid int, 
	@RetVal int = NULL Out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	if @ActionType = 'B2B'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_B2B_INV where invid = @invitemid )
			begin
				delete from TBL_EXT_GSTR1_B2B_INV where  invid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	Else If @ActionType = 'B2CL'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_B2CL_INV where invid = @invitemid )
			begin
				delete from TBL_EXT_GSTR1_B2CL_INV where  invid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	Else If @ActionType = 'B2CS'  -- Here we referring B2CL as a B2CS where the amount < 2.5 Lac
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_B2CL_INV where invid = @invitemid )
			begin
				delete from TBL_EXT_GSTR1_B2CL_INV where  invid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	Else If @ActionType = 'CDNR'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_CDNR where cdnrid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_CDNR where  cdnrid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	Else If @ActionType = 'CDNUR'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_CDNUR where cdnurid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_CDNUR where  cdnurid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	Else If @ActionType = 'DOCISSUE'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_DOC where docid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_DOC where  docid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	Else If @ActionType = 'EXP'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_EXP_INV where invid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_EXP_INV where  invid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	Else If @ActionType = 'HSN'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_HSN where hsnid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_HSN where  hsnid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	Else If @ActionType = 'NIL'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_NIL where nilid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_NIL where  nilid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	Else If @ActionType = 'TXP'
	Begin
		if exists (select 1 from TBL_EXT_GSTR1_TXP where txpid = @invitemid)
			begin
				delete from TBL_EXT_GSTR1_TXP where  txpid = @invitemid
				set @RetVal = 1 -- Deleted Successfully
			End
		Else
			Begin
				 set @RetVal = -1 -- Not deleted because record not exists
			End
	End
	
	select  @RetVal
  End