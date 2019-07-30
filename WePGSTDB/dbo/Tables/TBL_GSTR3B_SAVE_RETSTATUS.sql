CREATE TABLE [dbo].[TBL_GSTR3B_SAVE_RETSTATUS] (
    [retstatusid] INT            IDENTITY (1, 1) NOT NULL,
    [gstin]       VARCHAR (15)   NULL,
    [fp]          VARCHAR (10)   NULL,
    [referenceno] VARCHAR (255)  NULL,
    [actiontype]  VARCHAR (15)   NULL,
    [refids]      NVARCHAR (MAX) NULL,
    [status]      VARCHAR (10)   NULL,
    [errorreport] NVARCHAR (MAX) NULL,
    [customerid]  INT            NULL,
    [createdby]   INT            NULL,
    [createddate] DATETIME       NULL,
    CONSTRAINT [PK_TBL_GSTR3B_SAVE_RETSTATUS] PRIMARY KEY CLUSTERED ([retstatusid] ASC)
);

