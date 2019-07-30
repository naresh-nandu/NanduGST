CREATE TABLE [dbo].[TBL_GSTR1_B2CSA_ITMS] (
    [b2csaitmsid] INT             IDENTITY (1, 1) NOT NULL,
    [b2csaid]     INT             NOT NULL,
    [rt]          DECIMAL (18, 2) NULL,
    [txval]       DECIMAL (18, 2) NULL,
    [iamt]        DECIMAL (18, 2) NULL,
    [camt]        DECIMAL (18, 2) NULL,
    [samt]        DECIMAL (18, 2) NULL,
    [csamt]       DECIMAL (18, 2) NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CSA_ITMS] PRIMARY KEY CLUSTERED ([b2csaitmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CSA_ITMS_TBL_GSTR1_B2CSA] FOREIGN KEY ([b2csaid]) REFERENCES [dbo].[TBL_GSTR1_B2CSA] ([b2csaid])
);

