CREATE TABLE [dbo].[TBL_GSTR2A_B2B_INV_ITMS_DET] (
    [itmdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr2aid] INT             NULL,
    PRIMARY KEY CLUSTERED ([itmdetid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR2A_B2B_INV_ITMS_DET_222D0DB4B2E407EBE910EA5EB61B17E0]
    ON [dbo].[TBL_GSTR2A_B2B_INV_ITMS_DET]([itmsid] ASC, [rt] ASC)
    INCLUDE([camt], [csamt], [iamt], [samt], [txval]);

