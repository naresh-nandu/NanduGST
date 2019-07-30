CREATE TABLE [dbo].[MAS_Roles_Resources] (
    [Role_Resource_ID]             INT IDENTITY (1, 1) NOT NULL,
    [FK_Role_Resource_Role_ID]     INT NOT NULL,
    [FK_Role_Resource_Resource_ID] INT NOT NULL,
    [Role_Resource_IsAssigned]     BIT NOT NULL,
    CONSTRAINT [PK_MAS_Roles_Resources] PRIMARY KEY CLUSTERED ([Role_Resource_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_MAS_Roles_Resources_44FD3714A253419EFDF19FEA9DFCF64A]
    ON [dbo].[MAS_Roles_Resources]([FK_Role_Resource_Role_ID] ASC, [Role_Resource_IsAssigned] ASC, [FK_Role_Resource_Resource_ID] ASC);

