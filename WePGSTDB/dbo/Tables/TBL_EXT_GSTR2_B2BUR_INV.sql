﻿CREATE TABLE [dbo].[TBL_EXT_GSTR2_B2BUR_INV] (
    [b2burid]      INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (75)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [val]          DECIMAL (18, 2) NULL,
    [cname]        VARCHAR (255)   NULL,
    [rt]           DECIMAL (18, 2) NULL,
    [txval]        DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [tx_c]         DECIMAL (18, 2) NULL,
    [tx_s]         DECIMAL (18, 2) NULL,
    [tx_cs]        DECIMAL (18, 2) NULL,
    [elg]          VARCHAR (2)     NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createddate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [hsncode]      VARCHAR (50)    NULL,
    [hsndesc]      VARCHAR (255)   NULL,
    [uqc]          VARCHAR (50)    NULL,
    [qty]          DECIMAL (18, 2) NULL,
    [unitprice]    DECIMAL (18, 2) NULL,
    [discount]     DECIMAL (18, 2) NULL,
    [createdby]    INT             NULL,
    [supplierid]   INT             NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [sply_ty]      VARCHAR (5)     NULL,
    [pos]          VARCHAR (2)     NULL,
    [tx_i]         DECIMAL (18, 2) NULL,
    [CompCode]     VARCHAR (50)    NULL,
    [UnitCode]     VARCHAR (50)    NULL,
    [ReceivedBy]   VARCHAR (50)    NULL,
    [ReceivedDate] VARCHAR (10)    NULL,
    PRIMARY KEY CLUSTERED ([b2burid] ASC)
);




GO


