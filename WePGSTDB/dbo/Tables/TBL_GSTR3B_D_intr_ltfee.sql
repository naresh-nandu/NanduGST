CREATE TABLE [dbo].[TBL_GSTR3B_D_intr_ltfee] (
    [intr_ltfeeid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid]    INT NOT NULL,
    [gstinid]      INT NULL,
    PRIMARY KEY CLUSTERED ([intr_ltfeeid] ASC)
);

