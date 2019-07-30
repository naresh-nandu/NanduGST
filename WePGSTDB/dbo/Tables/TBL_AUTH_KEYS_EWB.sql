CREATE TABLE [dbo].[TBL_AUTH_KEYS_EWB] (
    [KeyId]             INT            IDENTITY (1, 1) NOT NULL,
    [EWBUserId]         NVARCHAR (50)  NULL,
    [GSTIN]             NVARCHAR (15)  NULL,
    [Username]          NVARCHAR (MAX) NULL,
    [Password]          NVARCHAR (MAX) NULL,
    [AppKey]            NVARCHAR (MAX) NULL,
    [EncryptedAppKey]   NVARCHAR (MAX) NULL,
    [EncryptedPassword] NVARCHAR (MAX) NULL,
    [EncryptedSEK]      NVARCHAR (MAX) NULL,
    [AuthToken]         NVARCHAR (MAX) NULL,
    [GSTINId]           INT            NULL,
    [Userid]            INT            NULL,
    [CustId]            INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [ModifiedDate]      DATETIME       NULL,
    CONSTRAINT [PK_TBL_AUTH_KEYS_EWB] PRIMARY KEY CLUSTERED ([KeyId] ASC)
);

