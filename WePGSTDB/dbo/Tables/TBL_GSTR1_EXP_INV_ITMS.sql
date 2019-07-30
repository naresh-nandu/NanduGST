CREATE TABLE [dbo].[TBL_GSTR1_EXP_INV_ITMS] (
    [itmsid]  INT             IDENTITY (1, 1) NOT NULL,
    [invid]   INT             NOT NULL,
    [txval]   DECIMAL (18, 2) NULL,
    [rt]      DECIMAL (18, 2) NULL,
    [iamt]    DECIMAL (18, 2) NULL,
    [gstinid] INT             NULL,
    [gstr1id] INT             NULL,
    [csamt]   DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_TBL_GSTR1_EXP_INV_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_EXP_INV_ITMS_TBL_GSTR1_EXP_INV] FOREIGN KEY ([invid]) REFERENCES [dbo].[TBL_GSTR1_EXP_INV] ([invid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_EXP_INV_ITMS_ADEC9189FB7283612A90ECB1C47CD5C4]
    ON [dbo].[TBL_GSTR1_EXP_INV_ITMS]([invid] ASC)
    INCLUDE([iamt], [rt], [txval]);

