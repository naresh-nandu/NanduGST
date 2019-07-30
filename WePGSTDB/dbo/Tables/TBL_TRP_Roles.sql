CREATE TABLE [dbo].[TBL_TRP_Roles] (
    [Role_ID]   INT            IDENTITY (1, 1) NOT NULL,
    [Role_Name] NVARCHAR (100) NOT NULL,
    [TRPId]     INT            NULL,
    CONSTRAINT [TRP_Roles] PRIMARY KEY CLUSTERED ([Role_ID] ASC)
);

