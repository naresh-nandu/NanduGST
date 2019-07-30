
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR3B Records in the corresponding staging area tables
				
Written by  : vishal.singh@wepdigital.com

Date		Who			Decription 
05/04/2018	vishal 	Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_GSTR3B_Data


 */
 CREATE PROCEDURE [usp_Delete_GSTR3B_Data]
       @gstin nvarchar(15),
	    @fp nvarchar(10),
		@Mode tinyint

AS 
SET NOCOUNT ON 
if @Mode = 1
BEGIN 
   

   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp
Delete from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp

Select a.* Into #GstrIds
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) a

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIds

Select * from TBL_GSTR3B_sup_det_osup_zero where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_sup_det_osup_nongst  where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_sup_det_osup_nil_exmp  where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_sup_det_osup_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_sup_det_isup_rev where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_sup_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_itc_elg_itc_rev where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_itc_elg_itc_net where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_itc_elg_itc_inelg where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_itc_elg_itc_avl where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_itc_elg where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inward_sup_isup_details where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inward_sup where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_intr_ltfee_intr_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_intr_ltfee where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inter_sup_unreg_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inter_sup_uin_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inter_sup_comp_det where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B_inter_sup where gstr3bid in(Select gstr3bid From #GstrIds)
Select * from TBL_GSTR3B  where gstr3bid in(Select gstr3bid From #GstrIds)

select * from TBL_EXT_GSTR3B_DET where gstin = @gstin




Delete  from TBL_GSTR3B_sup_det_osup_zero where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_sup_det_osup_nongst  where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_sup_det_osup_nil_exmp  where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_sup_det_osup_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_sup_det_isup_rev where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_sup_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_itc_elg_itc_rev where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_itc_elg_itc_net where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_itc_elg_itc_inelg where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_itc_elg_itc_avl where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_itc_elg where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inward_sup_isup_details where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inward_sup where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_intr_ltfee_intr_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_intr_ltfee where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inter_sup_unreg_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inter_sup_uin_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inter_sup_comp_det where gstr3bid in(Select gstr3bid From #GstrIds)
Delete  from TBL_GSTR3B_inter_sup where gstr3bid in(Select gstr3bid From #GstrIds)
END
else if  @Mode = 2

BEGIN 
   

   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp
  

Select b.* Into #GstrIdsb
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) b

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIdsb

Select * from TBL_GSTR3B_sup_det_osup_zero where gstr3bid in(Select gstr3bid From #GstrIdsb)
Select * from TBL_GSTR3B_sup_det_osup_nongst  where gstr3bid in(Select gstr3bid From #GstrIdsb)
Select * from TBL_GSTR3B_sup_det_osup_nil_exmp  where gstr3bid in(Select gstr3bid From #GstrIdsb)
Select * from TBL_GSTR3B_sup_det_osup_det where gstr3bid in(Select gstr3bid From #GstrIdsb)
Select * from TBL_GSTR3B_sup_det_isup_rev where gstr3bid in(Select gstr3bid From #GstrIdsb)
Select * from TBL_GSTR3B  where gstr3bid in(Select gstr3bid From #GstrIdsb)

select * from TBL_EXT_GSTR3B_DET where gstin = @gstin

Delete  from TBL_GSTR3B_sup_det_osup_zero where gstr3bid in(Select gstr3bid From #GstrIdsb)
Delete  from TBL_GSTR3B_sup_det_osup_nongst  where gstr3bid in(Select gstr3bid From #GstrIdsb)
Delete  from TBL_GSTR3B_sup_det_osup_nil_exmp  where gstr3bid in(Select gstr3bid From #GstrIdsb)
Delete  from TBL_GSTR3B_sup_det_osup_det where gstr3bid in(Select gstr3bid From #GstrIdsb)
Delete  from TBL_GSTR3B_sup_det_isup_rev where gstr3bid in(Select gstr3bid From #GstrIdsb)
END
else if @mode = 4

BEGIN 
   

   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp


Select c.* Into #GstrIdsc
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) c

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIdsc



Select * from TBL_GSTR3B_itc_elg_itc_rev where gstr3bid in(Select gstr3bid From #GstrIdsc)
Select * from TBL_GSTR3B_itc_elg_itc_net where gstr3bid in(Select gstr3bid From #GstrIdsc)
Select * from TBL_GSTR3B_itc_elg_itc_inelg where gstr3bid in(Select gstr3bid From #GstrIdsc)
Select * from TBL_GSTR3B_itc_elg_itc_avl where gstr3bid in(Select gstr3bid From #GstrIdsc)
Select * from TBL_GSTR3B_itc_elg where gstr3bid in(Select gstr3bid From #GstrIdsc)


select * from TBL_EXT_GSTR3B_DET where gstin = @gstin



Delete  from TBL_GSTR3B_itc_elg_itc_rev where gstr3bid in(Select gstr3bid From #GstrIdsc)
Delete  from TBL_GSTR3B_itc_elg_itc_net where gstr3bid in(Select gstr3bid From #GstrIdsc)
Delete  from TBL_GSTR3B_itc_elg_itc_inelg where gstr3bid in(Select gstr3bid From #GstrIdsc)
Delete  from TBL_GSTR3B_itc_elg_itc_avl where gstr3bid in(Select gstr3bid From #GstrIdsc)
Delete  from TBL_GSTR3B_itc_elg where gstr3bid in(Select gstr3bid From #GstrIdsc)
END
else if @mode = 5

BEGIN 
  

   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp


Select d.* Into #GstrIdsd
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) d

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIdsd


Select * from TBL_GSTR3B_inward_sup_isup_details where gstr3bid in(Select gstr3bid From #GstrIdsd)
Select * from TBL_GSTR3B_inward_sup where gstr3bid in(Select gstr3bid From #GstrIdsd)

Select * from TBL_GSTR3B  where gstr3bid in(Select gstr3bid From #GstrIdsd)

select * from TBL_EXT_GSTR3B_DET where gstin = @gstin

Delete  from TBL_GSTR3B_inward_sup_isup_details where gstr3bid in(Select gstr3bid From #GstrIdsd)
Delete  from TBL_GSTR3B_inward_sup where gstr3bid in(Select gstr3bid From #GstrIdsd)

END
else if @mode = 6

BEGIN 
   

   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp


Select e.* Into #GstrIdse
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) e

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIdse


Select * from TBL_GSTR3B_intr_ltfee_intr_det where gstr3bid in(Select gstr3bid From #GstrIdse)
Select * from TBL_GSTR3B_intr_ltfee where gstr3bid in(Select gstr3bid From #GstrIdse)

Select * from TBL_GSTR3B  where gstr3bid in(Select gstr3bid From #GstrIdse)

select * from TBL_EXT_GSTR3B_DET where gstin = @gstin

Delete  from TBL_GSTR3B_intr_ltfee_intr_det where gstr3bid in(Select gstr3bid From #GstrIdse)
Delete  from TBL_GSTR3B_intr_ltfee where gstr3bid in(Select gstr3bid From #GstrIdse)
END
else if @mode=3


BEGIN 


   select * from TBL_EXT_GSTR3B_DET where gstin = @gstin and fp =@fp


Select f.* Into #GstrIdsf
From
(Select gstr3bid as gstr3bid from TBL_GSTR3B where gstin =@gstin and ret_period =@fp
) f

-- drop table #GstrIds

--drop table #GstrIds

select * from #GstrIdsf


Select * from TBL_GSTR3B_inter_sup_unreg_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Select * from TBL_GSTR3B_inter_sup_uin_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Select * from TBL_GSTR3B_inter_sup_comp_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Select * from TBL_GSTR3B_inter_sup where gstr3bid in(Select gstr3bid From #GstrIdsf)
Select * from TBL_GSTR3B  where gstr3bid in(Select gstr3bid From #GstrIdsf)

select * from TBL_EXT_GSTR3B_DET where gstin = @gstin


Delete  from TBL_GSTR3B_inter_sup_unreg_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Delete  from TBL_GSTR3B_inter_sup_uin_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Delete  from TBL_GSTR3B_inter_sup_comp_det where gstr3bid in(Select gstr3bid From #GstrIdsf)
Delete  from TBL_GSTR3B_inter_sup where gstr3bid in(Select gstr3bid From #GstrIdsf)



END