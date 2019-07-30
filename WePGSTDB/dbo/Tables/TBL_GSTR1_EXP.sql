CREATE TABLE [dbo].[TBL_GSTR1_EXP] (
    [expid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT          NOT NULL,
    [ex_tp]   VARCHAR (25) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_EXP] PRIMARY KEY CLUSTERED ([expid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_EXP_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

