CREATE TABLE [dbo].[TBL_GSTR_ESIGN_DATA] (
    [EsignId]     INT            IDENTITY (1, 1) NOT NULL,
    [GSTRName]    NVARCHAR (50)  NULL,
    [ESignedData] NVARCHAR (MAX) NULL,
    [GSTINNo]     VARCHAR (15)   NULL,
    [CreatedBy]   INT            NULL,
    [CreatedDate] DATETIME       NULL,
    [aadhaarNo]   NVARCHAR (15)  NULL,
    [TxId]        NVARCHAR (50)  NULL,
    [authMode]    NVARCHAR (50)  NULL,
    [status]      INT            NULL,
    [Consent]     NCHAR (10)     NULL,
    CONSTRAINT [PK_TBL_GSTR_ESIGN_DATA] PRIMARY KEY CLUSTERED ([EsignId] ASC)
);

