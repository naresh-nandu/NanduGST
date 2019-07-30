CREATE TABLE [dbo].[TBL_EXT_GSTR1_EXP_INV] (
    [invid]           INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [gt]              DECIMAL (18, 2) NULL,
    [cur_gt]          DECIMAL (18, 2) NULL,
    [exp_typ]         VARCHAR (5)     NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (75)    NULL,
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    [sbnum]           VARCHAR (50)    NULL,
    [sbdt]            VARCHAR (50)    NULL,
    [txval]           DECIMAL (18, 2) NULL,
    [rt]              DECIMAL (18, 2) NULL,
    [iamt]            DECIMAL (18, 2) NULL,
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
    [uqc]             VARCHAR (50)    NULL,
    [sbpcode]         VARCHAR (6)     NULL,
    [Addinfo]         VARCHAR (MAX)   NULL,
    [ShippingAddress] VARCHAR (MAX)   NULL,
    [createdby]       INT             NULL,
    [buyerid]         INT             NULL,
    [ReceiverName]    VARCHAR (250)   NULL,
    [csamt]           DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_EXP_INV_61865BC8D7F94C532148DD04E6BDDC09]
    ON [dbo].[TBL_EXT_GSTR1_EXP_INV]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([cur_gt], [errormessage], [exp_typ], [fp], [gstin], [gt], [iamt], [idt], [inum], [rt], [sbdt], [sbnum], [sbpcode], [txval], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_EXP_INV_FD1829497E374EE1A7770AE978A6E31E]
    ON [dbo].[TBL_EXT_GSTR1_EXP_INV]([fp] ASC, [gstin] ASC, [idt] ASC, [inum] ASC, [rowstatus] ASC)
    INCLUDE([Addinfo], [buyerid], [chksum], [createdby], [createddate], [cur_gt], [discount], [errormessage], [exp_typ], [fileid], [flag], [gt], [hsncode], [hsndesc], [iamt], [ispdfgenerated], [qty], [ReceiverName], [referenceno], [rt], [sbdt], [sbnum], [sbpcode], [ShippingAddress], [sourcetype], [txval], [unitprice], [uqc], [val]);

