CREATE TABLE [dbo].[TBL_GSTR1_SAVE_RETSTATUS] (
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
    CONSTRAINT [PK_TBL_GSTR1_SAVE_RETSTATUS] PRIMARY KEY CLUSTERED ([retstatusid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_SAVE_RETSTATUS_0655D6C3CF8432C0639059DFDAFE6957]
    ON [dbo].[TBL_GSTR1_SAVE_RETSTATUS]([fp] ASC, [gstin] ASC)
    INCLUDE([actiontype], [createdby], [createddate], [customerid], [errorreport], [referenceno], [refids], [status]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_SAVE_RETSTATUS_65FF3B6222F0D978D642AE33C352B2C1]
    ON [dbo].[TBL_GSTR1_SAVE_RETSTATUS]([referenceno] ASC, [status] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_SAVE_RETSTATUS_C48C0AFE635A0C3218EB7174BC4FBC42]
    ON [dbo].[TBL_GSTR1_SAVE_RETSTATUS]([createdby] ASC, [customerid] ASC, [status] ASC)
    INCLUDE([actiontype], [createddate], [errorreport], [fp], [gstin], [referenceno], [refids]);

