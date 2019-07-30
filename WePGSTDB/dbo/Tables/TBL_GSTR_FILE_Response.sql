CREATE TABLE [dbo].[TBL_GSTR_FILE_Response] (
    [GSTRFileId]  INT          IDENTITY (1, 1) NOT NULL,
    [GSTINNo]     VARCHAR (15) NULL,
    [GSTRName]    VARCHAR (50) NULL,
    [Status]      VARCHAR (50) NULL,
    [AckNo]       VARCHAR (50) NULL,
    [CustomerId]  INT          NULL,
    [UserId]      INT          NULL,
    [CreatedDate] DATETIME     NULL,
    [fp]          VARCHAR (6)  NULL,
    CONSTRAINT [PK_TBL_GSTR_FILE_Response] PRIMARY KEY CLUSTERED ([GSTRFileId] ASC)
);

