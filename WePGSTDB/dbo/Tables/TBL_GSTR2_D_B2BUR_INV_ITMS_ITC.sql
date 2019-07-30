CREATE TABLE [dbo].[TBL_GSTR2_D_B2BUR_INV_ITMS_ITC] (
    [itcid]    INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [tx_i]     DECIMAL (18, 2) NULL,
    [tx_c]     DECIMAL (18, 2) NULL,
    [tx_s]     DECIMAL (18, 2) NULL,
    [tx_cs]    DECIMAL (18, 2) NULL,
    [elg]      VARCHAR (2)     NULL,
    [gstinid]  INT             NULL,
    [gstr2did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_B2BUR_INV_ITMS_ITC] PRIMARY KEY CLUSTERED ([itcid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_B2BUR_INV_ITMS_ITC_TBL_GSTR2_D_B2BUR_INV_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR2_D_B2BUR_INV_ITMS] ([itmsid])
);

