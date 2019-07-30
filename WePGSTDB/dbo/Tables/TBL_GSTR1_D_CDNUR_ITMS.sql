﻿CREATE TABLE [dbo].[TBL_GSTR1_D_CDNUR_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [cdnurid]  INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr1did] INT NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNUR_ITMS_TBL_GSTR1_D_CDNUR] FOREIGN KEY ([cdnurid]) REFERENCES [dbo].[TBL_GSTR1_D_CDNUR] ([cdnurid])
);

