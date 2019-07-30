﻿CREATE TABLE [dbo].[TBL_GSTR1_TXPA_ITMS] (
    [itmsid]  INT             IDENTITY (1, 1) NOT NULL,
    [txpaid]  INT             NOT NULL,
    [rt]      DECIMAL (18, 2) NULL,
    [ad_amt]  DECIMAL (18, 2) NULL,
    [iamt]    DECIMAL (18, 2) NULL,
    [camt]    DECIMAL (18, 2) NULL,
    [samt]    DECIMAL (18, 2) NULL,
    [csamt]   DECIMAL (18, 2) NULL,
    [gstinid] INT             NULL,
    [gstr1id] INT             NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_TXPA_ITMS_TBL_GSTR1_TXPA] FOREIGN KEY ([txpaid]) REFERENCES [dbo].[TBL_GSTR1_TXPA] ([txpaid])
);

