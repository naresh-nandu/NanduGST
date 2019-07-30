CREATE TABLE [dbo].[TBL_GSTR1_DOCS] (
    [docsid]     INT          IDENTITY (1, 1) NOT NULL,
    [docissueid] INT          NOT NULL,
    [num]        INT          NULL,
    [from]       VARCHAR (16) NULL,
    [to]         VARCHAR (16) NULL,
    [totnum]     INT          NULL,
    [cancel]     INT          NULL,
    [net_issue]  INT          NULL,
    [gstinid]    INT          NULL,
    [gstr1id]    INT          NULL,
    PRIMARY KEY CLUSTERED ([docsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_DOCS_TBL_GSTR1_DOC_ISSUE] FOREIGN KEY ([docissueid]) REFERENCES [dbo].[TBL_GSTR1_DOC_ISSUE] ([docissueid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_DOCS_6B6551E452FB1908B87FE92B739C9DB6]
    ON [dbo].[TBL_GSTR1_DOCS]([docissueid] ASC, [from] ASC, [num] ASC, [to] ASC)
    INCLUDE([cancel], [net_issue], [totnum]);

