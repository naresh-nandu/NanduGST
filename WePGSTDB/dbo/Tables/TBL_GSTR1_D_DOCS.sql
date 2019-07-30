CREATE TABLE [dbo].[TBL_GSTR1_D_DOCS] (
    [docsid]     INT          IDENTITY (1, 1) NOT NULL,
    [docissueid] INT          NOT NULL,
    [num]        INT          NULL,
    [from]       VARCHAR (10) NULL,
    [to]         VARCHAR (10) NULL,
    [totnum]     INT          NULL,
    [cancel]     INT          NULL,
    [net_issue]  INT          NULL,
    [gstinid]    INT          NULL,
    [gstr1did]   INT          NULL,
    PRIMARY KEY CLUSTERED ([docsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_DOCS_TBL_GSTR1_D_DOC_ISSUE] FOREIGN KEY ([docissueid]) REFERENCES [dbo].[TBL_GSTR1_D_DOC_ISSUE] ([docissueid])
);

