CREATE TABLE [dbo].[TBL_GSTR2_IMPG] (
    [impgid]       INT             IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [is_sez]       VARCHAR (1)     NULL,
    [stin]         VARCHAR (15)    NULL,
    [boe_num]      VARCHAR (50)    NULL,
    [boe_dt]       VARCHAR (50)    NULL,
    [boe_val]      DECIMAL (18, 2) NULL,
    [chksum]       VARCHAR (64)    NULL,
    [gstinid]      INT             NULL,
    [port_code]    VARCHAR (6)     NULL,
    [uploadStatus] VARCHAR (1)     NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_IMPG] PRIMARY KEY CLUSTERED ([impgid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_IMPG_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

