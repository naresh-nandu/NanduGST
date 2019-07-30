CREATE TABLE [dbo].[TBL_EXT_GSTR1_B2CS] (
    [invid]           INT             IDENTITY (1, 1) NOT NULL,
    [gstin]           VARCHAR (15)    NULL,
    [fp]              VARCHAR (10)    NULL,
    [gt]              DECIMAL (18, 2) NULL,
    [cur_gt]          DECIMAL (18, 2) NULL,
    [flag]            VARCHAR (1)     NULL,
    [chksum]          VARCHAR (15)    NULL,
    [sply_ty]         VARCHAR (5)     NULL,
    [txval]           DECIMAL (18, 2) NULL,
    [typ]             VARCHAR (2)     NULL,
    [etin]            VARCHAR (15)    NULL,
    [pos]             VARCHAR (2)     NULL,
    [rt]              DECIMAL (18, 2) NULL,
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
    [inum]            VARCHAR (50)    NULL,
    [idt]             VARCHAR (50)    NULL,
    [val]             DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_B2CS_8D23C1CF1C181D9A5BB6A9F5F3C1CDB5]
    ON [dbo].[TBL_EXT_GSTR1_B2CS]([referenceno] ASC, [rowstatus] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [cur_gt], [errormessage], [etin], [fp], [gstin], [gt], [iamt], [pos], [rt], [samt], [sply_ty], [txval], [typ]);

