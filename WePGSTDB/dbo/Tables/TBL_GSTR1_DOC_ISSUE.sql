CREATE TABLE [dbo].[TBL_GSTR1_DOC_ISSUE] (
    [docissueid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id]     INT          NOT NULL,
    [flag]        VARCHAR (1)  NULL,
    [doc_num]     INT          NULL,
    [doc_typ]     VARCHAR (50) NULL,
    [gstinid]     INT          NULL,
    [chksum]      VARCHAR (75) NULL,
    [createddate] DATETIME     NULL,
    [createdby]   INT          NULL,
    [custid]      INT          NULL,
    PRIMARY KEY CLUSTERED ([docissueid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_DOC_ISSUE_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

