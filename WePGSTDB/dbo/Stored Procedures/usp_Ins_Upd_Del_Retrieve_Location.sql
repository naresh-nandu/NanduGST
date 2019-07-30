  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure for Location CURD Operation  
      
Written by  : nareshn@wepindia.com  
  
Date  Who   Decription   
03.05.2018 Naresh  Initial Version  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Ins_Upd_Del_Retrieve_Location 'AADCG9940P','32AADCG9940P1Z2','Hyderabad','raja@wepindia.com',1,1,'',1,'I','2,3,4,5,6'  
 */  
  
CREATE PROCEDURE [dbo].[usp_Ins_Upd_Del_Retrieve_Location]    
 @panNo nvarchar(10) = NULL,  
 @gstinNo nvarchar(15)= NULL,  
 @branchName nvarchar(50)= NULL,  
 @email nvarchar(50)= NULL,  
 @custId int= NULL,  
 @createdBy int= NULL,   
 @branchId int =NULL,  
 @Mode nvarchar(2), /* I-Insert R-Retrieve U-Update D-Delete AU-Add New User DU-Delete Existing User */  
 @userAccessId nvarchar(MAX)=NULL,
 @AccessId int=NULL,  
 @retVal int =NULL out  
-- /*mssx*/ With Encryption   
AS  
  
Set Nocount on  
  set @retVal=0  
  
    
  Declare @PanId int,  
          @GstinId int,  
          @bId int  
      
  Select @PanId=PANId from TBL_Cust_PAN where PANNo=@panNo and rowstatus=1  
  Select @GstinId=GSTINId from TBL_Cust_GSTIN where GSTINNo=@gstinNo and rowstatus=1  
  
  If @Mode='I'  
    Begin   
     If (isnull(@PanId,0)<>0) and (isnull(@GstinId,0)<>0)  
      Begin  
         Insert into TBL_Cust_Location  
         (panId, gstinId, branch, emailId, custId, createdBy, createdDate,rowStatus)   
          Values  
         (@PanId,@GstinId, @branchName, @email, @custId,@createdBy,GETDATE(),1)  
  
         Select @bId= SCOPE_IDENTITY()  
         Select Name into #names from dbo.splitstring(@userAccessId)  
    
         Insert Into TBL_LocationAccess_Users (BranchId,CustId,CreatedBy,CreatedDate,RowStatus,UserId)  
         Select @bId,@custId,@createdBy,GETDate(),1,Name from #names t1   
         where not exists(select 1 from  TBL_LocationAccess_Users t2  
         where t2.Branchid=@bId  
         and t2.UserId = t1.Name)   
    
         Set @retVal=1  
        End  
    End  
  
  Else if @Mode='U'  
   Begin  
    Update TBL_Cust_Location set panId=@PanId,gstinId=@GstinId,branch=@branchName,  
    emailId=@email,modifiedBy=@createdBy,modifiedDate=GETDate() where branchId=@branchId  
   End  
  
  Else if @Mode='R'  
   Begin  
    Select   
    t2.CompanyName as company,  
    t2.PANNo as pan,  
    t3.GSTINNo as gstinNo,  
    branch as branchName,  
    emailId as email,  
    branchId  
      from TBL_Cust_LOCATION t1  
      Inner Join TBL_Cust_PAN t2 On t2.PANId = t1.panId  
      Inner Join TBL_Cust_GSTIN t3 On t3.GSTINId = t1.gstinId  
      where t1.custId=@custId And t1.rowstatus=1  
   End  
   
  Else if @Mode='D'  
   Begin   
    Update TBL_Cust_Location set rowStatus=0 where branchId=@branchId  
    Update TBL_LocationAccess_Users set rowStatus=0 where branchId=@branchId
   End  

  Else if @Mode='AU'
   Begin
    Insert Into TBL_LocationAccess_Users (BranchId,CustId,CreatedBy,CreatedDate,RowStatus,UserId)  
    Select @branchId,@custId,@createdBy,GETDate(),1,@AccessId   
    where not exists(select 1 from  TBL_LocationAccess_Users 
    where UserId=@AccessId)
   End

  Else if @Mode='DU'
   Begin
    Update TBL_LocationAccess_Users set rowStatus=0 where branchId=@branchId And UserId=@AccessId
   End
 
Return @retVal