CREATE TABLE [dbo].[TBL_GSTR3B_inward_sup] (
    [inward_supid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bid]     INT NOT NULL,
    [gstinid]      INT NULL,
    PRIMARY KEY CLUSTERED ([inward_supid] ASC)
);

