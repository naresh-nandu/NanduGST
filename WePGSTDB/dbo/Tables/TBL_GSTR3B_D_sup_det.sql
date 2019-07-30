CREATE TABLE [dbo].[TBL_GSTR3B_D_sup_det] (
    [sup_detid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid] INT NOT NULL,
    [gstinid]   INT NULL,
    PRIMARY KEY CLUSTERED ([sup_detid] ASC)
);

