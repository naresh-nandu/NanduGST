CREATE TABLE [dbo].[TBL_GSTR2_TXPD] (
    [txpdid]       INT          IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT          NOT NULL,
    [pos]          VARCHAR (2)  NULL,
    [sply_ty]      VARCHAR (5)  NULL,
    [flag]         VARCHAR (1)  NULL,
    [chksum]       VARCHAR (64) NULL,
    [gstinid]      INT          NULL,
    [uploadStatus] VARCHAR (1)  NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_TXPD] PRIMARY KEY CLUSTERED ([txpdid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_TXPD_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

