CREATE TABLE [dbo].[StatusMaster] (
    [StatusCode] INT            NOT NULL,
    [StatusDesc] NVARCHAR (100) NULL,
    [rowStatus]  BIT            NULL,
    CONSTRAINT [PK_StatusMaster] PRIMARY KEY CLUSTERED ([StatusCode] ASC)
);

