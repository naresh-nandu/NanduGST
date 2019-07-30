CREATE TABLE [dbo].[TBL_GSTR3B_sup_det] (
    [sup_detid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bid]  INT NOT NULL,
    [gstinid]   INT NULL,
    PRIMARY KEY CLUSTERED ([sup_detid] ASC)
);

