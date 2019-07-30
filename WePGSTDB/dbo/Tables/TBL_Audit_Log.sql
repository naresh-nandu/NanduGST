CREATE TABLE [dbo].[TBL_Audit_Log] (
    [Audit_ID]          INT            IDENTITY (1, 1) NOT NULL,
    [FK_Audit_User_ID]  INT            NULL,
    [FK_Audit_Username] NVARCHAR (150) NULL,
    [Audit_DateTime]    DATETIME       NULL,
    [Audit_Message]     NVARCHAR (MAX) NULL,
    [Audit_Exception]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TBL_Audit_Log] PRIMARY KEY CLUSTERED ([Audit_ID] ASC)
);

