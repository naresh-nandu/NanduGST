CREATE TABLE [dbo].[TBL_GSTR1_B2CS_INV_ITMS_DET] (
    [itmsdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]    INT             NOT NULL,
    [txval]     DECIMAL (18, 2) NULL,
    [rt]        DECIMAL (18, 2) NULL,
    [iamt]      DECIMAL (18, 2) NULL,
    [csamt]     DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr1id]   INT             NULL,
    [camt]      DECIMAL (18, 2) NULL,
    [samt]      DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CS_INV_ITMS_DET] PRIMARY KEY CLUSTERED ([itmsdetid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CS_INV_ITMS_DET_TBL_GSTR1_B2CS_INV_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR1_B2CS_INV_ITMS] ([itmsid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2CS_INV_ITMS_DET_3F698A1B6FCFFC585362CEB5817E378D]
    ON [dbo].[TBL_GSTR1_B2CS_INV_ITMS_DET]([itmsid] ASC);

