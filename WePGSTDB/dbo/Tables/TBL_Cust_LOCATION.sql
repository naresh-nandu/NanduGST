CREATE TABLE [dbo].[TBL_Cust_LOCATION] (
    [branchId]     INT            IDENTITY (1, 1) NOT NULL,
    [panId]        INT            NULL,
    [gstinId]      INT            NULL,
    [branch]       NVARCHAR (MAX) NULL,
    [emailId]      NVARCHAR (100) NULL,
    [custId]       INT            NULL,
    [createdBy]    INT            NULL,
    [createdDate]  DATETIME       NULL,
    [modifiedBy]   INT            NULL,
    [modifiedDate] DATETIME       NULL,
    [rowStatus]    BIT            NULL,
    CONSTRAINT [PK_TBL_Cust_LOCATION] PRIMARY KEY CLUSTERED ([branchId] ASC)
);

