CREATE TABLE [dbo].[TBL_EXT_GSTR1_B2B_INV] (
    [invid]           INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [gt]              DECIMAL (18, 2) NULL,
    [cur_gt]          DECIMAL (18, 2) NULL,
    [ctin]            VARCHAR (15)    NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (75)    NULL,
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    [pos]             VARCHAR (2)     NULL,
    [rchrg]           VARCHAR (1)     NULL,
    [etin]            VARCHAR (15)    NULL,
    [inv_typ]         VARCHAR (5)     NULL,
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
    [ShippingAddress] VARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_65C9627CBE51B7B299770F40A3F604BB]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([gstin] ASC, [idt] ASC, [inum] ASC)
    INCLUDE([inv_typ]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_6CEFBFBE8C90E76F4794C600F887C473]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([inum] ASC, [idt] ASC, [referenceno] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_81E360E01F76D5A5BF0639AAB05BB496]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [ctin], [cur_gt], [errormessage], [etin], [fp], [gstin], [gt], [iamt], [idt], [inum], [inv_typ], [pos], [rchrg], [rt], [samt], [txval], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_9089D9CD9AD34EDA29687D9E20739657]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([fp] ASC, [gstin] ASC, [idt] ASC, [inum] ASC)
    INCLUDE([rowstatus]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_A62B98DDB2174196276FB7B63AB84FD0]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([fp] ASC, [gstin] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2B_INV_D942C84B126856C5F14F6C3C3139D42A]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([gstin] ASC, [idt] ASC, [inum] ASC, [rowstatus] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_Flag]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([flag] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_FP]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([fp] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_GSTIN]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([gstin] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_IDT]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([idt] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_INUM]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([inum] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_ReferenceNo]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([referenceno] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_B2B_INV_IDX_RowStatus]
    ON [dbo].[TBL_EXT_GSTR1_B2B_INV]([rowstatus] ASC);

