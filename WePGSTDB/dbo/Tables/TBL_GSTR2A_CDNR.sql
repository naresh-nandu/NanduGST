CREATE TABLE [dbo].[TBL_GSTR2A_CDNR] (
    [cdnrid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr2aid] INT          NOT NULL,
    [ctin]     VARCHAR (15) NULL,
    [cfs]      VARCHAR (1)  NULL,
    [gstinid]  INT          NULL,
    PRIMARY KEY CLUSTERED ([cdnrid] ASC)
);

