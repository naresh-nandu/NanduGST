CREATE TABLE [dbo].[TBL_GSTR2_ITCRVSL_ITMS] (
    [itmsid]    INT             IDENTITY (1, 1) NOT NULL,
    [itcrvslid] INT             NOT NULL,
    [ruleName]  NVARCHAR (50)   NULL,
    [iamt]      DECIMAL (18, 2) NULL,
    [camt]      DECIMAL (18, 2) NULL,
    [samt]      DECIMAL (18, 2) NULL,
    [csamt]     DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr2id]   INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_ITCRVSL_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_ITCRVSL_ITMS_TBL_GSTR2_ITCRVSL] FOREIGN KEY ([itcrvslid]) REFERENCES [dbo].[TBL_GSTR2_ITCRVSL] ([itcrvslid])
);

