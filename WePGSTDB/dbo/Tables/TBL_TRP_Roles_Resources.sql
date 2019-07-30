CREATE TABLE [dbo].[TBL_TRP_Roles_Resources] (
    [Role_Resource_ID]             INT IDENTITY (1, 1) NOT NULL,
    [FK_Role_Resource_Role_ID]     INT NOT NULL,
    [FK_Role_Resource_Resource_ID] INT NOT NULL,
    [Role_Resource_IsAssigned]     BIT NOT NULL,
    CONSTRAINT [PK_TRP_Roles_Resources] PRIMARY KEY CLUSTERED ([Role_Resource_ID] ASC)
);

