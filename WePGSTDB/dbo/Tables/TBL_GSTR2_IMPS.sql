CREATE TABLE [dbo].[TBL_GSTR2_IMPS] (
    [impsid]       INT             IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [pos]          VARCHAR (2)     NULL,
    [ival]         DECIMAL (18, 2) NULL,
    [chksum]       VARCHAR (64)    NULL,
    [gstinid]      INT             NULL,
    [uploadStatus] VARCHAR (1)     NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_IMPS] PRIMARY KEY CLUSTERED ([impsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_IMPS_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

