CREATE TABLE [dbo].[TBL_EXT_GSTR1_CDNUR] (
    [cdnurid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]          VARCHAR (15)    NULL,
    [fp]             VARCHAR (10)    NULL,
    [gt]             DECIMAL (18, 2) NULL,
    [cur_gt]         DECIMAL (18, 2) NULL,
    [typ]            VARCHAR (15)    NULL,
    [flag]           VARCHAR (1)     NULL,
    [chksum]         VARCHAR (75)    NULL,
    [ntty]           VARCHAR (1)     NULL,
    [nt_num]         VARCHAR (50)    NULL,
    [nt_dt]          VARCHAR (50)    NULL,
    [inum]           VARCHAR (50)    NULL,
    [idt]            VARCHAR (50)    NULL,
    [val]            DECIMAL (18, 2) NULL,
    [rt]             DECIMAL (18, 2) NULL,
    [txval]          DECIMAL (18, 2) NULL,
    [iamt]           DECIMAL (18, 2) NULL,
    [camt]           DECIMAL (18, 2) NULL,
    [samt]           DECIMAL (18, 2) NULL,
    [csamt]          DECIMAL (18, 2) NULL,
    [rowstatus]      TINYINT         NULL,
    [sourcetype]     VARCHAR (15)    NULL,
    [referenceno]    VARCHAR (50)    NULL,
    [createddate]    DATETIME        NULL,
    [errormessage]   VARCHAR (255)   NULL,
    [fileid]         INT             NULL,
    [hsncode]        VARCHAR (50)    NULL,
    [hsndesc]        VARCHAR (255)   NULL,
    [qty]            DECIMAL (18, 2) NULL,
    [unitprice]      DECIMAL (18, 2) NULL,
    [discount]       DECIMAL (18, 2) NULL,
    [ispdfgenerated] BIT             NULL,
    [p_gst]          VARCHAR (1)     NULL,
    [rsn]            VARCHAR (50)    NULL,
    [pos]            VARCHAR (2)     NULL,
    [sply_ty]        VARCHAR (5)     NULL,
    [uqc]            VARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([cdnurid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_CDNUR_06E6EF82AB0F9C9A8DB4A9A3EC050CF0]
    ON [dbo].[TBL_EXT_GSTR1_CDNUR]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [cur_gt], [errormessage], [fp], [gstin], [gt], [iamt], [idt], [inum], [nt_dt], [nt_num], [ntty], [p_gst], [rsn], [rt], [samt], [txval], [typ], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_CDNUR_2C5723D2CB1C108CE52C11DB59E05275]
    ON [dbo].[TBL_EXT_GSTR1_CDNUR]([fp] ASC, [gstin] ASC, [nt_dt] ASC, [nt_num] ASC, [rowstatus] ASC)
    INCLUDE([camt], [csamt], [iamt], [samt], [txval]);

