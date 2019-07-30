CREATE TABLE [dbo].[TBL_GSTR1_CDNURA_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [cdnuraid] INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr1id]  INT NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNURA_ITMS_TBL_GSTR1_CDNURA] FOREIGN KEY ([cdnuraid]) REFERENCES [dbo].[TBL_GSTR1_CDNURA] ([cdnuraid])
);

