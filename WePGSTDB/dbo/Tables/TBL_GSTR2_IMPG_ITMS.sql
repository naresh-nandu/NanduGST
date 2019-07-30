﻿CREATE TABLE [dbo].[TBL_GSTR2_IMPG_ITMS] (
    [itmsid]  INT             IDENTITY (1, 1) NOT NULL,
    [impgid]  INT             NOT NULL,
    [num]     INT             NULL,
    [rt]      DECIMAL (18, 2) NULL,
    [txval]   DECIMAL (18, 2) NULL,
    [iamt]    DECIMAL (18, 2) NULL,
    [csamt]   DECIMAL (18, 2) NULL,
    [elg]     VARCHAR (2)     NULL,
    [tx_i]    DECIMAL (18, 2) NULL,
    [tx_cs]   DECIMAL (18, 2) NULL,
    [gstinid] INT             NULL,
    [gstr2id] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_IMPG_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_IMPG_ITMS_TBL_GSTR2_IMPG] FOREIGN KEY ([impgid]) REFERENCES [dbo].[TBL_GSTR2_IMPG] ([impgid])
);

