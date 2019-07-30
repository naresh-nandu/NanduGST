CREATE TABLE [dbo].[TBL_GSTR2A_CDNR_NT_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [ntid]     INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr2aid] INT NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC)
);

