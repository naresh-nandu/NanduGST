CREATE TABLE [dbo].[TBL_EXT_GSTR1_B2CS_INV] (
    [invid]           INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [sply_typ]        VARCHAR (5)     NULL,
    [typ]             VARCHAR (2)     NULL,
    [gt]              DECIMAL (18, 2) NULL,
    [cur_gt]          DECIMAL (18, 2) NULL,
    [pos]             VARCHAR (2)     NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (75)    NULL,
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    [etin]            VARCHAR (15)    NULL,
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
    [uqc]             VARCHAR (50)    NULL,
    [createdby]       INT             NULL,
    [itemdescription] NVARCHAR (500)  NULL,
    [Addinfo]         NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2CS_INV_87555A8A9AE92433DE3C23B5E1EEB912]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([referenceno] ASC, [rowstatus] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [cur_gt], [errormessage], [etin], [fp], [gstin], [gt], [iamt], [idt], [inum], [pos], [rt], [samt], [sply_typ], [txval], [typ], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2CS_INV_9089D9CD9AD34EDA29687D9E20739657]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([fp] ASC, [gstin] ASC, [idt] ASC, [inum] ASC)
    INCLUDE([rowstatus]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2CS_INV_933B54FA200FCABEDA7141680974A1D3]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([rowstatus] ASC)
    INCLUDE([fp], [gstin], [idt], [inum], [rt], [txval]);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2CS_INV_IDX_FP]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([fp] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2CS_INV_IDX_GSTIN]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([gstin] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2CS_INV_IDX_IDT]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([idt] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2CS_INV_IDX_INUM]
    ON [dbo].[TBL_EXT_GSTR1_B2CS_INV]([inum] ASC);

