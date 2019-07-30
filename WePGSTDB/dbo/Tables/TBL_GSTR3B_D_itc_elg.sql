CREATE TABLE [dbo].[TBL_GSTR3B_D_itc_elg] (
    [itc_elgid] INT IDENTITY (1, 1) NOT NULL,
    [gstr3bdid] INT NOT NULL,
    [gstinid]   INT NULL,
    PRIMARY KEY CLUSTERED ([itc_elgid] ASC)
);

