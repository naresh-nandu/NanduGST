CREATE TABLE [dbo].[MAS_Roles] (
    [Role_ID]    INT            IDENTITY (1, 1) NOT NULL,
    [Role_Name]  NVARCHAR (100) NOT NULL,
    [CustomerID] INT            NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Role_ID] ASC)
);

