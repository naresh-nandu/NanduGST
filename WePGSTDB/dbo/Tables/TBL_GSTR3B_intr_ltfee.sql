CREATE TABLE [dbo].[TBL_GSTR3B_intr_ltfee] (
    [intr_ltfeeid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bid]     INT NOT NULL,
    [gstinid]      INT NULL,
    PRIMARY KEY CLUSTERED ([intr_ltfeeid] ASC)
);

