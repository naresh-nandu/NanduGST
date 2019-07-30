CREATE TABLE [dbo].[TBL_GSTR2_TXI] (
    [txiid]        INT          IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT          NOT NULL,
    [pos]          VARCHAR (2)  NULL,
    [sply_ty]      VARCHAR (5)  NULL,
    [chksum]       VARCHAR (64) NULL,
    [flag]         VARCHAR (1)  NULL,
    [gstinid]      INT          NULL,
    [uploadStatus] VARCHAR (1)  NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_TXI] PRIMARY KEY CLUSTERED ([txiid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_TXI_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

