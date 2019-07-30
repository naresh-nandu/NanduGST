CREATE TABLE [dbo].[TBL_GSTR1_B2CS_INV] (
    [invid]       INT             IDENTITY (1, 1) NOT NULL,
    [b2csid]      INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [chksum]      VARCHAR (75)    NULL,
    [inum]        VARCHAR (50)    NULL,
    [idt]         VARCHAR (50)    NULL,
    [val]         DECIMAL (18, 2) NULL,
    [etin]        VARCHAR (15)    NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    [sply_ty]     VARCHAR (5)     NULL,
    [typ]         VARCHAR (2)     NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CS_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CS_INV_TBL_GSTR1_B2CS_N] FOREIGN KEY ([b2csid]) REFERENCES [dbo].[TBL_GSTR1_B2CS_N] ([b2csid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2CS_INV_91C5871CE32301C5E28E082296AA8F57]
    ON [dbo].[TBL_GSTR1_B2CS_INV]([idt] ASC, [inum] ASC, [b2csid] ASC)
    INCLUDE([flag]);

