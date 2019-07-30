﻿CREATE TABLE [dbo].[TBL_EXT_GSTR1_CDNR] (
    [cdnrid]          INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [gt]              DECIMAL (18, 2) NULL,
    [cur_gt]          DECIMAL (18, 2) NULL,
    [ctin]            VARCHAR (15)    NULL,
    [cfs]             VARCHAR (1)     NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (75)    NULL,
    [ntty]            VARCHAR (1)     NULL,
    [nt_num]          VARCHAR (50)    NULL,
    [nt_dt]           VARCHAR (50)    NULL,
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    [pos]             VARCHAR (2)     NULL,
    [rt]              DECIMAL (18, 2) NULL,
    [txval]           DECIMAL (18, 2) NULL,
    [iamt]            DECIMAL (18, 2) NULL,
    [camt]            DECIMAL (18, 2) NULL,
    [samt]            DECIMAL (18, 2) NULL,
    [csamt]           DECIMAL (18, 2) NULL,
    [rowstatus]       TINYINT         NULL,
    [sourcetype]      VARCHAR (15)    NULL,
    [referenceno]     VARCHAR (50)    NULL,
    [createddate]     DATETIME        NULL,
    [errormessage]    VARCHAR (255)   NULL,
    [fileid]          INT             NULL,
    [hsncode]         VARCHAR (50)    NULL,
    [hsndesc]         VARCHAR (255)   NULL,
    [qty]             DECIMAL (18, 2) NULL,
    [unitprice]       DECIMAL (18, 2) NULL,
    [discount]        DECIMAL (18, 2) NULL,
    [ispdfgenerated]  BIT             NULL,
    [buyerid]         INT             NULL,
    [createdby]       INT             NULL,
    [uqc]             VARCHAR (50)    NULL,
    [itemdescription] NVARCHAR (500)  NULL,
    [Addinfo]         NVARCHAR (MAX)  NULL,
    [p_gst]           VARCHAR (1)     NULL,
    [rsn]             VARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([cdnrid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_CDNR_3516021ABFC6A16F0E81A78E53DAEEFD]
    ON [dbo].[TBL_EXT_GSTR1_CDNR]([fp] ASC, [gstin] ASC, [nt_dt] ASC, [nt_num] ASC, [rowstatus] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_CDNR_5A03B1AFDF6A65A0336F0960F7E39406]
    ON [dbo].[TBL_EXT_GSTR1_CDNR]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([camt], [cfs], [csamt], [ctin], [cur_gt], [errormessage], [fp], [gstin], [gt], [iamt], [idt], [inum], [nt_dt], [nt_num], [ntty], [p_gst], [pos], [rsn], [rt], [samt], [txval], [val]);

