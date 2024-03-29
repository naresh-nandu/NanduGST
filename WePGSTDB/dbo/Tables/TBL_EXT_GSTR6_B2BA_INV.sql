﻿CREATE TABLE [dbo].[TBL_EXT_GSTR6_B2BA_INV] (
    [b2baid]         INT             IDENTITY (1, 1) NOT NULL,
    [gstin]          VARCHAR (15)    NULL,
    [fp]             VARCHAR (10)    NULL,
    [ctin]           VARCHAR (15)    NULL,
    [flag]           VARCHAR (1)     NULL,
    [chksum]         VARCHAR (75)    NULL,
    [inum]           VARCHAR (50)    NULL,
    [idt]            VARCHAR (50)    NULL,
    [val]            DECIMAL (18, 2) NULL,
    [pos]            VARCHAR (2)     NULL,
    [oinum]          VARCHAR (50)    NULL,
    [oidt]           VARCHAR (50)    NULL,
    [rt]             DECIMAL (11, 2) NULL,
    [txval]          DECIMAL (11, 2) NULL,
    [iamt]           DECIMAL (11, 2) NULL,
    [camt]           DECIMAL (11, 2) NULL,
    [samt]           DECIMAL (11, 2) NULL,
    [csamt]          DECIMAL (11, 2) NULL,
    [rowstatus]      TINYINT         NULL,
    [sourcetype]     VARCHAR (15)    NULL,
    [referenceno]    VARCHAR (50)    NULL,
    [createddate]    DATETIME        NULL,
    [errormessage]   VARCHAR (255)   NULL,
    [fileid]         INT             NULL,
    [discount]       DECIMAL (18, 2) NULL,
    [ispdfgenerated] BIT             NULL,
    [supplierid]     INT             NULL,
    [createdby]      INT             NULL,
    PRIMARY KEY CLUSTERED ([b2baid] ASC)
);

