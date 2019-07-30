﻿CREATE TABLE [dbo].[TBL_GSTR1_TXPA] (
    [txpaid]       INT            IDENTITY (1, 1) NOT NULL,
    [gstr1id]      INT            NOT NULL,
    [flag]         VARCHAR (1)    NULL,
    [chksum]       VARCHAR (75)   NULL,
    [omon]         VARCHAR (10)   NULL,
    [pos]          VARCHAR (2)    NULL,
    [sply_ty]      VARCHAR (25)   NULL,
    [diff_percent] DECIMAL (4, 2) NULL,
    [gstinid]      INT            NULL,
    [createddate]  DATETIME       NULL,
    [createdby]    INT            NULL,
    [custid]       INT            NULL,
    PRIMARY KEY CLUSTERED ([txpaid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_TXPA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

