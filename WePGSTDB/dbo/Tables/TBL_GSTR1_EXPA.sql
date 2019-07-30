CREATE TABLE [dbo].[TBL_GSTR1_EXPA] (
    [expaid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT          NOT NULL,
    [ex_tp]   VARCHAR (25) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_EXPA] PRIMARY KEY CLUSTERED ([expaid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_EXPA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

