CREATE TABLE [dbo].[TBL_GSTR6A_CDNR] (
    [cdnrid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr6aid] INT          NOT NULL,
    [ctin]     VARCHAR (15) NULL,
    [gstinid]  INT          NULL,
    PRIMARY KEY CLUSTERED ([cdnrid] ASC)
);

