CREATE TABLE [dbo].[TBL_GSTR1_D_DOC_ISSUE] (
    [docissueid] INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did]   INT          NOT NULL,
    [flag]       VARCHAR (1)  NULL,
    [chksum]     VARCHAR (64) NULL,
    [doc_num]    INT          NULL,
    [doc_typ]    VARCHAR (50) NULL,
    [gstinid]    INT          NULL,
    PRIMARY KEY CLUSTERED ([docissueid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_DOC_ISSUE_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

