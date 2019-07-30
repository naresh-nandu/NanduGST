CREATE TABLE [dbo].[TBL_GSTR1_B2B_INV] (
    [invid]       INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]       INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [chksum]      VARCHAR (75)    NULL,
    [inum]        VARCHAR (50)    NULL,
    [idt]         VARCHAR (50)    NULL,
    [val]         DECIMAL (18, 2) NULL,
    [pos]         VARCHAR (2)     NULL,
    [rchrg]       VARCHAR (1)     NULL,
    [etin]        VARCHAR (15)    NULL,
    [inv_typ]     VARCHAR (5)     NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2B_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2B_INV_TBL_GSTR1_B2B] FOREIGN KEY ([b2bid]) REFERENCES [dbo].[TBL_GSTR1_B2B] ([b2bid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_0632E6133CCA54DFDC5730C9998095C8]
    ON [dbo].[TBL_GSTR1_B2B_INV]([idt] ASC, [inum] ASC, [gstinid] ASC)
    INCLUDE([inv_typ]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_0D17E32EB2E8F9C8170ECEC8A61FCAF9]
    ON [dbo].[TBL_GSTR1_B2B_INV]([gstinid] ASC)
    INCLUDE([flag], [inum]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_204ED6FE6126739EE264B45F424DF133]
    ON [dbo].[TBL_GSTR1_B2B_INV]([gstr1id] ASC)
    INCLUDE([inum]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_419915F734708611BD48CDDE0A435981]
    ON [dbo].[TBL_GSTR1_B2B_INV]([b2bid] ASC, [flag] ASC)
    INCLUDE([chksum], [etin], [gstinid], [gstr1id], [idt], [inum], [inv_typ], [pos], [rchrg], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_447D83F5B944D54768186D520335B262]
    ON [dbo].[TBL_GSTR1_B2B_INV]([inum] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_5A187687BB366632F1FBFF58D1A86DC7]
    ON [dbo].[TBL_GSTR1_B2B_INV]([flag] ASC, [gstinid] ASC)
    INCLUDE([b2bid]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2B_INV_DAB1731C3EADB010D3F1C4D886AF1C8B]
    ON [dbo].[TBL_GSTR1_B2B_INV]([flag] ASC, [gstr1id] ASC)
    INCLUDE([b2bid], [chksum], [etin], [gstinid], [idt], [inum], [inv_typ], [pos], [rchrg], [val]);

