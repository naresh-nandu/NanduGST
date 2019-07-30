﻿CREATE TABLE [dbo].[TBL_GSTR1_B2CLA_INV_ITMS] (
    [itmsid]  INT IDENTITY (1, 1) NOT NULL,
    [invid]   INT NOT NULL,
    [num]     INT NULL,
    [gstinid] INT NULL,
    [gstr1id] INT NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CLA_INV_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CLA_INV_ITMS_TBL_GSTR1_B2CLA_INV] FOREIGN KEY ([invid]) REFERENCES [dbo].[TBL_GSTR1_B2CLA_INV] ([invid])
);

