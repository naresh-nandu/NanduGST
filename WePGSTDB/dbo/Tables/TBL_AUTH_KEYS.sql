CREATE TABLE [dbo].[TBL_AUTH_KEYS] (
    [KeyId]              INT            IDENTITY (1, 1) NOT NULL,
    [Username]           NVARCHAR (MAX) NULL,
    [AppKey]             NVARCHAR (MAX) NULL,
    [EncryptedAppKey]    NVARCHAR (MAX) NULL,
    [EncryptedOTP]       NVARCHAR (MAX) NULL,
    [EncryptedSEK]       NVARCHAR (MAX) NULL,
    [AuthToken]          NVARCHAR (MAX) NULL,
    [Expiry]             INT            NULL,
    [CreatedBy]          INT            NULL,
    [CreatedDate]        DATETIME       NULL,
    [AuthorizationToken] NVARCHAR (MAX) NULL,
    [userid]             INT            NULL,
    [custid]             INT            NULL,
    CONSTRAINT [PK_TBL_AUTH_KEYS] PRIMARY KEY CLUSTERED ([KeyId] ASC)
);

