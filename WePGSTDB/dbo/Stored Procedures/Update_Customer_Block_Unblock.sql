-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Update the User details into Userlist Table
-- exec [Update_Customer_Block_Unblock] 21,0,1
-- =============================================
CREATE PROCEDURE [Update_Customer_Block_Unblock] 
	@CustomerId    int,
	@Status	int,	
	@CreatedBy	int

AS
BEGIN
	
	update TBL_Customer set RowStatus=@Status,ModifiedBy=@CreatedBy,ModifiedDate=GETDATE() where CustId=@CustomerId
		
End