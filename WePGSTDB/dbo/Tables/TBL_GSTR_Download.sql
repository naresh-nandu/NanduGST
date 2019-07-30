CREATE TABLE [dbo].[TBL_GSTR_Download] (
    [GSTRDownloadId] INT            IDENTITY (1, 1) NOT NULL,
    [GSTRName]       VARCHAR (50)   NULL,
    [GSTINNo]        VARCHAR (15)   NULL,
    [JsonData]       NVARCHAR (MAX) NULL,
    [CustomerId]     INT            NULL,
    [UserId]         INT            NULL,
    [CreatedDate]    DATETIME       NULL,
    CONSTRAINT [PK_TBL_GSTR_Download] PRIMARY KEY CLUSTERED ([GSTRDownloadId] ASC)
);

