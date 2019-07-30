CREATE TABLE [dbo].[TBL_GSTR2_B2B_INV_ITMS_ITC] (
    [itmitcid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [tx_i]     DECIMAL (18, 2) NULL,
    [tx_c]     DECIMAL (18, 2) NULL,
    [tx_s]     DECIMAL (18, 2) NULL,
    [tx_cs]    DECIMAL (18, 2) NULL,
    [elg]      VARCHAR (2)     NULL,
    [gstinid]  INT             NULL,
    [gstr2id]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_B2B_INV_ITMS_ITC] PRIMARY KEY CLUSTERED ([itmitcid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_B2B_INV_ITMS_ITC_TBL_GSTR2_B2B_INV_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR2_B2B_INV_ITMS] ([itmsid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR2_B2B_INV_ITMS_ITC_4FC93A15298B501DFBC37F535ABB06E2]
    ON [dbo].[TBL_GSTR2_B2B_INV_ITMS_ITC]([itmsid] ASC)
    INCLUDE([elg], [gstinid], [gstr2id], [tx_c], [tx_cs], [tx_i], [tx_s]);

