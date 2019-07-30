﻿CREATE TABLE [dbo].[TBL_GSTR1_D_B2CL_INV_ITMS] (
    [itmsid]   INT IDENTITY (1, 1) NOT NULL,
    [invid]    INT NOT NULL,
    [num]      INT NULL,
    [gstinid]  INT NULL,
    [gstr1did] INT NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_B2CL_INV_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2CL_INV_ITMS_TBL_GSTR1_D_B2CL_INV] FOREIGN KEY ([invid]) REFERENCES [dbo].[TBL_GSTR1_D_B2CL_INV] ([invid])
);

