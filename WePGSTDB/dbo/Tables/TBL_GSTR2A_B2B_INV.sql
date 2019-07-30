CREATE TABLE [dbo].[TBL_GSTR2A_B2B_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]    INT             NOT NULL,
    [chksum]   VARCHAR (64)    NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [flag]     VARCHAR (1)     NULL,
    [rchrg]    VARCHAR (1)     NULL,
    [pos]      VARCHAR (2)     NULL,
    [inv_typ]  VARCHAR (5)     NULL,
    [gstinid]  INT             NULL,
    [gstr2aid] INT             NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR2A_B2B_INV_141D31780D7F8088855EE7C2435C61CF]
    ON [dbo].[TBL_GSTR2A_B2B_INV]([idt] ASC, [inum] ASC, [inv_typ] ASC, [pos] ASC, [val] ASC)
    INCLUDE([chksum]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR2A_B2B_INV_AA5E87E5538A31594A5F744786586CA2]
    ON [dbo].[TBL_GSTR2A_B2B_INV]([b2bid] ASC, [flag] ASC)
    INCLUDE([idt], [inum], [inv_typ], [pos], [rchrg], [val]);

