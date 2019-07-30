CREATE TABLE [dbo].[TBL_GSTR1_D_EXP] (
    [expid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [ex_tp]    VARCHAR (50) NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_EXP] PRIMARY KEY CLUSTERED ([expid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_EXP_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

