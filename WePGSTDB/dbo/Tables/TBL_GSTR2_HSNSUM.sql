CREATE TABLE [dbo].[TBL_GSTR2_HSNSUM] (
    [hsnsumid]     INT          IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT          NOT NULL,
    [chksum]       VARCHAR (64) NULL,
    [flag]         VARCHAR (1)  NULL,
    [gstinid]      INT          NULL,
    [uploadStatus] VARCHAR (1)  NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_HSNSUM] PRIMARY KEY CLUSTERED ([hsnsumid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_HSNSUM_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

