CREATE TABLE [dbo].[TBL_GSTR3B_D_inter_sup] (
    [inter_supid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid]   INT NOT NULL,
    [gstinid]     INT NULL,
    PRIMARY KEY CLUSTERED ([inter_supid] ASC)
);

