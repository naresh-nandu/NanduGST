CREATE TABLE [dbo].[TBL_GSTR1_EXP_INV] (
    [invid]       INT             IDENTITY (1, 1) NOT NULL,
    [expid]       INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [chksum]      VARCHAR (75)    NULL,
    [inum]        VARCHAR (50)    NULL,
    [idt]         VARCHAR (50)    NULL,
    [val]         DECIMAL (18, 2) NULL,
    [sbnum]       NVARCHAR (15)   NULL,
    [sbdt]        VARCHAR (50)    NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    [sbpcode]     VARCHAR (6)     NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_EXP_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_EXP_INV_TBL_GSTR1_EXP] FOREIGN KEY ([expid]) REFERENCES [dbo].[TBL_GSTR1_EXP] ([expid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_EXP_INV_B59A81138592C8DFCD713485E08C7A05]
    ON [dbo].[TBL_GSTR1_EXP_INV]([expid] ASC, [idt] ASC, [inum] ASC)
    INCLUDE([flag], [sbdt], [sbnum], [val]);

