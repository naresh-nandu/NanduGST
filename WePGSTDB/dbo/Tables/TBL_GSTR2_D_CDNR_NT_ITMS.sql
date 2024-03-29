﻿CREATE TABLE [dbo].[TBL_GSTR2_D_CDNR_NT_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [invid]    INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr2did] INT NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_CDNR_NT_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_CDNR_NT_ITMS_TBL_GSTR2_D_CDNR_NT] FOREIGN KEY ([invid]) REFERENCES [dbo].[TBL_GSTR2_D_CDNR_NT] ([invid])
);

