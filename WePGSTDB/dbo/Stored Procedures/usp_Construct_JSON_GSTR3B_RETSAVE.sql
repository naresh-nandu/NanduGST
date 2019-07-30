
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR3B Save
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/10/2017	Seshadri			Initial Version
12/11/2017	Karthik		Fixed Integration Testing defects
12/15/2017	Seshadri	Fixed Null Value Issues

*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR3B_RETSAVE '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Construct_JSON_GSTR3B_RETSAVE] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@Flag varchar(1),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	gstin,ret_period,

	( Select JSON_QUERY
			((	Select
					(	Select	JSON_QUERY	
						((	
							(	Select txval,iamt,camt,samt,csamt
								From TBL_GSTR3B_sup_det_osup_det t1,
								TBL_GSTR3B_sup_det t2,
								TBL_GSTR3B t3
								Where t1.sup_detid = t2.sup_detid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	 
								/*
								Where t1.sup_detid = TBL_GSTR3B_sup_det.sup_detid
								And TBL_GSTR3B_sup_det.gstr3bid in 
									(	Select TBL_GSTR3B.gstr3bid 
										From TBL_GSTR3B 
										Where TBL_GSTR3B.gstin = @Gstin 
										And TBL_GSTR3B.ret_period = @Fp
									) */
								FOR JSON PATH , WITHOUT_ARRAY_WRAPPER	
							) 
						)) 
					) As osup_det,

					(	Select	JSON_QUERY	
						((	
							(	Select txval,iamt,csamt
								From TBL_GSTR3B_sup_det_osup_zero t1,
								TBL_GSTR3B_sup_det t2,
								TBL_GSTR3B t3
								Where t1.sup_detid = t2.sup_detid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	   	
								FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
							) 
						)) 
					) As osup_zero,
					
					(	Select	JSON_QUERY	
						((	
							(	Select txval
								From TBL_GSTR3B_sup_det_osup_nil_exmp t1,
								TBL_GSTR3B_sup_det t2,
								TBL_GSTR3B t3
								Where t1.sup_detid = t2.sup_detid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp		
								FOR JSON PATH , WITHOUT_ARRAY_WRAPPER
							) 
						)) 
					) As osup_nil_exmp,	
	
					(	Select	JSON_QUERY	
						((	
							(	Select txval,iamt,camt,samt,csamt
								From TBL_GSTR3B_sup_det_isup_rev t1,
								TBL_GSTR3B_sup_det t2,
								TBL_GSTR3B t3
								Where t1.sup_detid = t2.sup_detid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp		
								FOR JSON PATH , WITHOUT_ARRAY_WRAPPER
							) 
						)) 
					) As isup_rev,	

					(	Select	JSON_QUERY	
						((	
							(	Select txval
								From TBL_GSTR3B_sup_det_osup_nongst t1,
								TBL_GSTR3B_sup_det t2,
								TBL_GSTR3B t3
								Where t1.sup_detid = t2.sup_detid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp		
								FOR JSON PATH , WITHOUT_ARRAY_WRAPPER
							) 
						)) 
					) As osup_nongst	


				From TBL_GSTR3B_sup_det 
				Where TBL_GSTR3B_sup_det.gstr3bid in 
				(	Select TBL_GSTR3B.gstr3bid 
					From TBL_GSTR3B 
					Where TBL_GSTR3B.gstin = @Gstin 
					And TBL_GSTR3B.ret_period = @Fp
				) 
				Group by TBL_GSTR3B_sup_det.gstr3bid,TBL_GSTR3B_sup_det.sup_detid  
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
	) As sup_details,

	( Select JSON_QUERY
			((	Select
					(	Select	JSON_QUERY	
						((	
							(	Select pos,txval,iamt
								From TBL_GSTR3B_inter_sup_unreg_det t1,
								TBL_GSTR3B_inter_sup t2,
								TBL_GSTR3B t3
								Where t1.inter_supid = t2.inter_supid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	 
								FOR JSON PATH	
							) 
						)) 
					) As unreg_details,

					(	Select	JSON_QUERY	
						((	
							(	Select pos,txval,iamt
								From TBL_GSTR3B_inter_sup_comp_det t1,
								TBL_GSTR3B_inter_sup t2,
								TBL_GSTR3B t3
								Where t1.inter_supid = t2.inter_supid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	   	
								FOR JSON PATH 		
							) 
						)) 
					) As comp_details,
					
					(	Select	JSON_QUERY	
						((	
							(	Select pos,txval,iamt
								From TBL_GSTR3B_inter_sup_uin_det t1,
								TBL_GSTR3B_inter_sup t2,
								TBL_GSTR3B t3
								Where t1.inter_supid = t2.inter_supid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp		
								FOR JSON PATH 
							) 
						)) 
					) As uin_details	
	
				From TBL_GSTR3B_inter_sup 
				Where TBL_GSTR3B_inter_sup.gstr3bid in 
				(	Select TBL_GSTR3B.gstr3bid 
					From TBL_GSTR3B 
					Where TBL_GSTR3B.gstin = @Gstin 
					And TBL_GSTR3B.ret_period = @Fp
				) 
				Group by TBL_GSTR3B_inter_sup.gstr3bid,TBL_GSTR3B_inter_sup.inter_supid  
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
	) As inter_sup,

	( Select JSON_QUERY
			((	Select
					(	Select	JSON_QUERY	
						((	
							(	Select ty,
								IsNull(iamt,0) as iamt,
								IsNull(camt,0) as camt,
								IsNull(samt,0) as samt,
								IsNull(csamt,0) as csamt
								From TBL_GSTR3B_itc_elg_itc_avl t1,
								TBL_GSTR3B_itc_elg t2,
								TBL_GSTR3B t3
								Where t1.itc_elgid = t2.itc_elgid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	 
								FOR JSON PATH	
							) 
						)) 
					) As itc_avl,

					(	Select	JSON_QUERY	
						((	
							(	Select ty,iamt,camt,samt,csamt
								From TBL_GSTR3B_itc_elg_itc_rev t1,
								TBL_GSTR3B_itc_elg t2,
								TBL_GSTR3B t3
								Where t1.itc_elgid = t2.itc_elgid 
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	   	
								FOR JSON PATH 		
							) 
						)) 
					) As itc_rev,

					(	Select	JSON_QUERY	
						((	
							(	Select iamt,camt,samt,csamt
								From TBL_GSTR3B_itc_elg_itc_net t1,
								TBL_GSTR3B_itc_elg t2,
								TBL_GSTR3B t3
								Where t1.itc_elgid = t2.itc_elgid 
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	   	
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER 		
							) 
						)) 
					) As itc_net,
			
					(	Select	JSON_QUERY	
						((	
							(	Select ty,iamt,camt,samt,csamt
								From TBL_GSTR3B_itc_elg_itc_inelg t1,
								TBL_GSTR3B_itc_elg t2,
								TBL_GSTR3B t3
								Where t1.itc_elgid = t2.itc_elgid 
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	   	
								FOR JSON PATH 		
							) 
						)) 
					) As itc_inelg
			
				From TBL_GSTR3B_itc_elg 
				Where TBL_GSTR3B_itc_elg.gstr3bid in 
				(	Select TBL_GSTR3B.gstr3bid 
					From TBL_GSTR3B 
					Where TBL_GSTR3B.gstin = @Gstin 
					And TBL_GSTR3B.ret_period = @Fp
				) 
				Group by TBL_GSTR3B_itc_elg.gstr3bid,TBL_GSTR3B_itc_elg.itc_elgid  
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
	) As itc_elg ,

	( Select JSON_QUERY
			((	Select
					(	Select	JSON_QUERY	
						((	
							(	Select ty,inter,intra
								From TBL_GSTR3B_inward_sup_isup_details t1,
								TBL_GSTR3B_inward_sup t2,
								TBL_GSTR3B t3
								Where t1.inward_supid = t2.inward_supid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	 
								FOR JSON PATH	
							) 
						)) 
					) As isup_details

				From TBL_GSTR3B_inward_sup 
				Where TBL_GSTR3B_inward_sup.gstr3bid in 
				(	Select TBL_GSTR3B.gstr3bid 
					From TBL_GSTR3B 
					Where TBL_GSTR3B.gstin = @Gstin 
					And TBL_GSTR3B.ret_period = @Fp
				) 
				Group by TBL_GSTR3B_inward_sup.gstr3bid,TBL_GSTR3B_inward_sup.inward_supid  
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
	) As inward_sup,

	( Select JSON_QUERY
			((	Select
					(	Select	JSON_QUERY	
						((	
							(	Select iamt,camt,samt,csamt
								From TBL_GSTR3B_intr_ltfee_intr_det t1,
								TBL_GSTR3B_intr_ltfee t2,
								TBL_GSTR3B t3
								Where t1.intr_ltfeeid = t2.intr_ltfeeid
								And t2.gstr3bid = t3.gstr3bid
								And t3.gstin = @Gstin
								And t3.ret_period = @Fp	 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER	
							) 
						)) 
					) As intr_details

				From TBL_GSTR3B_intr_ltfee 
				Where TBL_GSTR3B_intr_ltfee.gstr3bid in 
				(	Select TBL_GSTR3B.gstr3bid 
					From TBL_GSTR3B 
					Where TBL_GSTR3B.gstin = @Gstin 
					And TBL_GSTR3B.ret_period = @Fp
				) 
				Group by TBL_GSTR3B_intr_ltfee.gstr3bid,TBL_GSTR3B_intr_ltfee.intr_ltfeeid  
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
	) As intr_ltfee





	From TBL_GSTR3B 
	Where gstin = @Gstin 
	And TBL_GSTR3B.ret_period = @fp 
	Group by TBL_GSTR3B.gstin, TBL_GSTR3B.ret_period 
	FOR JSON AUTO   

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End