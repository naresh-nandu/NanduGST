CREATE TABLE [dbo].[TBL_GSTR3B_D_Tax_Pmt] (
    [taxpmtid]  INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid] INT NOT NULL,
    [gstinid]   INT NULL,
    PRIMARY KEY CLUSTERED ([taxpmtid] ASC)
);

