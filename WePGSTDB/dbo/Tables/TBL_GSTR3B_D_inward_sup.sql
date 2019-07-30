CREATE TABLE [dbo].[TBL_GSTR3B_D_inward_sup] (
    [inward_supid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid]    INT NOT NULL,
    [gstinid]      INT NULL,
    PRIMARY KEY CLUSTERED ([inward_supid] ASC)
);

