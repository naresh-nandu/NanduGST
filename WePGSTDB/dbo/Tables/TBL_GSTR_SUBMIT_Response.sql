CREATE TABLE [dbo].[TBL_GSTR_SUBMIT_Response] (
    [GSTRSubmitId] INT          IDENTITY (1, 1) NOT NULL,
    [GSTINNo]      VARCHAR (15) NULL,
    [GSTRName]     VARCHAR (50) NULL,
    [TransId]      VARCHAR (50) NULL,
    [RefId]        VARCHAR (50) NULL,
    [CustomerId]   INT          NULL,
    [UserId]       INT          NULL,
    [CreatedDate]  DATETIME     NULL,
    [fp]           VARCHAR (6)  NULL,
    [ActionType]   VARCHAR (10) NULL,
    [batchid]      INT          NULL,
    CONSTRAINT [PK_TBL_GSTR_SUBMIT_Response] PRIMARY KEY CLUSTERED ([GSTRSubmitId] ASC)
);

