CREATE TABLE [dbo].[TBL_GSTR3B_inter_sup] (
    [inter_supid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bid]    INT NOT NULL,
    [gstinid]     INT NULL,
    PRIMARY KEY CLUSTERED ([inter_supid] ASC)
);

