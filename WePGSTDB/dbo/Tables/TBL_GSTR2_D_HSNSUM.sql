﻿CREATE TABLE [dbo].[TBL_GSTR2_D_HSNSUM] (
    [hsnsumid] INT          IDENTITY (1, 1) NOT NULL,
    [gstr2did] INT          NOT NULL,
    [chksum]   VARCHAR (50) NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_HSNSUM] PRIMARY KEY CLUSTERED ([hsnsumid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_HSNSUM_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

