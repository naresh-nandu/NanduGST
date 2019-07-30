CREATE TABLE [dbo].[TBL_EXT_GSTR2_B2B_INV] (
    [b2bid]           INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [ctin]            VARCHAR (15)    NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (75)    NULL,
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    [pos]             VARCHAR (2)     NULL,
    [rchrg]           VARCHAR (1)     NULL,
    [inv_typ]         VARCHAR (5)     NULL,
    [rt]              DECIMAL (18, 2) NULL,
    [txval]           DECIMAL (18, 2) NULL,
    [iamt]            DECIMAL (18, 2) NULL,
    [camt]            DECIMAL (18, 2) NULL,
    [samt]            DECIMAL (18, 2) NULL,
    [csamt]           DECIMAL (18, 2) NULL,
    [tx_i]            DECIMAL (18, 2) NULL,
    [tx_c]            DECIMAL (18, 2) NULL,
    [tx_s]            DECIMAL (18, 2) NULL,
    [tx_cs]           DECIMAL (18, 2) NULL,
    [elg]             VARCHAR (2)     NULL,
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
    [supplierid]      INT             NULL,
    [uqc]             VARCHAR (50)    NULL,
    [createdby]       INT             NULL,
    [itemdescription] NVARCHAR (500)  NULL,
    [CompCode]        VARCHAR (50)    NULL,
    [UnitCode]        VARCHAR (50)    NULL,
    [ReceivedBy]      VARCHAR (50)    NULL,
    [ReceivedDate]    VARCHAR (10)    NULL,
    PRIMARY KEY CLUSTERED ([b2bid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR2_B2B_INV_6988AA84020345938B11E1690C6FC228]
    ON [dbo].[TBL_EXT_GSTR2_B2B_INV]([fp] ASC, [gstin] ASC, [idt] ASC, [inum] ASC, [ctin] ASC)
    INCLUDE([camt], [chksum], [createdby], [createddate], [csamt], [discount], [elg], [errormessage], [fileid], [flag], [hsncode], [hsndesc], [iamt], [inv_typ], [ispdfgenerated], [itemdescription], [pos], [qty], [rchrg], [referenceno], [rowstatus], [rt], [samt], [sourcetype], [supplierid], [tx_c], [tx_cs], [tx_i], [tx_s], [txval], [unitprice], [uqc], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR2_B2B_INV_B526247625530D1B62189572A11093FD]
    ON [dbo].[TBL_EXT_GSTR2_B2B_INV]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [ctin], [elg], [errormessage], [fp], [gstin], [iamt], [idt], [inum], [inv_typ], [pos], [rchrg], [rt], [samt], [tx_c], [tx_cs], [tx_i], [tx_s], [txval], [val]);

