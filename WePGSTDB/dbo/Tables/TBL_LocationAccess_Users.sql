CREATE TABLE [dbo].[TBL_LocationAccess_Users] (
    [UserAccessId] INT      IDENTITY (1, 1) NOT NULL,
    [BranchId]     INT      NULL,
    [UserId]       INT      NULL,
    [CustId]       INT      NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    [RowStatus]    BIT      NULL,
    CONSTRAINT [PK_LocationAccess_Users] PRIMARY KEY CLUSTERED ([UserAccessId] ASC)
);

