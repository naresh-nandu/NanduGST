CREATE TABLE [dbo].[TBL_GSTR6A_B2B_INV_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [invid]    INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr6aid] INT NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC)
);

