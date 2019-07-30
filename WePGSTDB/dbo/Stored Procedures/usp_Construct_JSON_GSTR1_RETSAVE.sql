

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 RET Save with batch wise
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/12/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_RETSAVE '33GSPTN0802G1ZL','012017','U'
 */

CREATE PROCEDURE [usp_Construct_JSON_GSTR1_RETSAVE]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Flag varchar(1),
	@rangeDefault int,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

Declare @range int = @rangeDefault
	Set Nocount on
		declare @ActionType varchar(15)
		set @ActionType='B2B'
		Select a.* Into #GstrIds
		From
		(Select gstr1id from TBL_GSTR1 where gstin = @Gstin and fp = @Fp
		) a

		CREATE TABLE #AllinvDetails(
			slNo int  IDENTITY(1,1) NOT NULL,
			ActionType varchar(15) NULL,
			ActionRowNo int NULL,
			Gstr1Id int NULL,
			InvId int NULL,
			NoOfItems int NULL,
			SumOfExrow int NULL)

			Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			(select 'B2B', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,invid,count(*) , 0  
				from TBL_GSTR1_B2B_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,invid)

			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'B2CL', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,invid,count(*) , 0  
			--	from TBL_GSTR1_B2CL_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,invid)

			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'B2CS', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,b2csid,count(*) , 0  
			--	from TBL_GSTR1_B2CS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,b2csid)

			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'CDNR', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,ntid,count(*) , 0  
			--	from TBL_GSTR1_CDNR_NT_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,ntid)
			
			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'CDNUR', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,cdnurid,count(*) , 0  
			--	from TBL_GSTR1_CDNUR_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,cdnurid)
			
			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'DOCISSUE', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,docissueid,count(*) , 0  
			--	from TBL_GSTR1_DOCS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,docissueid)
			
			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'EXP', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,invid,count(*) , 0  
			--	from TBL_GSTR1_EXP_INV_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,invid)
			
			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'HSN', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,hsnid,count(*) , 0  
			--	from TBL_GSTR1_HSN_DATA where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,hsnid)
			
			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'NIL', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,nilid,count(*) , 0  
			--	from TBL_GSTR1_NIL_INV where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,nilid)

			--Insert into #AllinvDetails(ActionType,ActionRowNo,Gstr1Id,InvId,NoOfItems,SumOfExrow)
			--(select 'TXP', ROW_NUMBER() OVER(ORDER BY gstr1id Desc) , gstr1id,txpid,count(*) , 0  
			--	from TBL_GSTR1_TXP_ITMS where gstr1id in(Select gstr1id From #GstrIds) group by gstr1id,txpid)


				declare @rowNumber int=1;
				declare @maxRow int;
				declare @lastSum int;
				
				set @maxRow=(
				select Max(ActionRowNo) from #AllinvDetails where ActionType = @ActionType)
						---------- Loop to update the sum of No Of items using cumulative sum and update in SumOfExrow column
						while(@rowNumber <= @maxRow)
						begin
								if @rownumber=1
									begin
									update #AllinvDetails set SumofExRow = NoOFItems  where ActionRowNo=1 and ActionType = @ActionType
									end
								else
									begin
									set @lastSum=(select SumofExRow from #AllinvDetails where ActionRowNo=@rowNumber-1 and ActionType = @ActionType)
									update #AllinvDetails set SumofExRow=@lastSum + NoOfItems where ActionRowNo = @rowNumber and  ActionType = @ActionType
									end
								set @rowNumber = @rowNumber + 1
						end

						-------------- To Summarize the Min and Max Inv Ids

			
			declare @maxSelected int,@lastMaxSelected int;
			declare @maxOfSum int;
			declare @i int=1,@maxLoop int; 
					
					--IF object_id('dbo.#InvDetails') is not null
					--   Begin
					   CREATE TABLE #InvDetails(
						ActionType varchar(15) NULL,
						slno int NULL,
						MinInvid int NULL,
						MaxInvid int NULL,
						ReccountSelected int NULL,
						Rangecnt int NULL)
					--   End

					set @maxOfSum=(select max(sumofexrow) from #AllinvDetails where ActionType = @ActionType)
					set @maxLoop=  cast(@maxOfSum as int ) / cast(@rangeDefault as int )  + 1
					 while (@maxLoop >= @i)
						 begin
							 if @i=1
							  set @lastMaxSelected =(select top 1 sumofexrow from #AllinvDetails where sumofexrow <= @range and ActionType = @ActionType )
							 else
							 set @lastMaxSelected=@maxSelected

							 set @maxSelected= (select max(sumofexrow)from #AllinvDetails where sumofexrow <= @range and ActionType = @ActionType)
							 set @range=@maxSelected

							 if @i=1
							 begin
								 insert into #InvDetails(ActionType,slno,MinInvid,MaxInvid,ReccountSelected,Rangecnt)
								 select @ActionType,@i,min(invid) MinInvoice,max(invid) MaxInvoice,@maxSelected MaxSelected,@rangeDefault*@i [Range]
									 from #AllinvDetails where sumofexrow <= @range  and ActionType = @ActionType 
							 end
							 else
							 begin
								  insert into #InvDetails(ActionType,slno,MinInvid,MaxInvid,ReccountSelected,Rangecnt)
								  select @ActionType,@i,min(invid) MinInvoice,max(invid) MaxInvoice,@maxSelected MaxSelected,@rangeDefault*@i [Range]
									 from #AllinvDetails where sumofexrow <= @maxSelected and sumofexrow > @lastMaxSelected  and ActionType = @ActionType
							 end
							 set @i=@i+1;
							 set @range=@rangeDefault+@maxSelected
						 end

					 --select * from #InvDetails where ActionType = @ActionType
					-- select * from #AllinvDetails  where ActionType = @ActionType
				Alter table #InvDetails add OPJsonStrings nvarchar(MAX)
				declare @a int=1,@maxslno int;
				Declare @MinInvId int,@MaxInvId int,@oppar nvarchar(Max)
				
				set @maxslno=(select max(slno) from #InvDetails where ActionType = @ActionType)
				
				 while (@maxslno >= @a)
						 begin
							select @MinInvId= MinInvId,@MaxInvId=MaxInvId from #InvDetails where slno = @a and  ActionType = @ActionType
							Declare @OPJsonString nvarchar(MAX) = NULL
							--set @OPJsonString=
							exec usp_Construct_JSON_GSTR1_B2B_RETSAVE @Gstin,@Fp,@MinInvId,@MaxInvId,@Flag ,@OPJsonString Output
							
							update #InvDetails set OPJsonStrings= @OPJsonString where slno =  @a and  ActionType = @ActionType
							set @a = @a + 1
						 End					
					
			 select * from #InvDetails where ActionType = @ActionType
			 select * from #AllinvDetails
			 drop table #InvDetails
			 drop table #AllinvDetails

END