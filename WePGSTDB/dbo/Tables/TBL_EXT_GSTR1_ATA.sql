﻿CREATE TABLE [dbo].[TBL_EXT_GSTR1_ATA] (
    [ataid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [gt]           DECIMAL (18, 2) NULL,
    [cur_gt]       DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [omon]         VARCHAR (6)     NULL,
    [pos]          VARCHAR (2)     NULL,
    [sply_ty]      VARCHAR (5)     NULL,
    [rt]           DECIMAL (18, 2) NULL,
    [ad_amt]       DECIMAL (18, 2) NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createdDate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [createdby]    INT             NULL,
    [diff_percent] DECIMAL (4, 2)  NULL,
    PRIMARY KEY CLUSTERED ([ataid] ASC)
);

