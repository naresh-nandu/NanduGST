﻿CREATE TABLE [dbo].[TBL_GSTR2_D_B2BUR_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2burid]  INT             NOT NULL,
    [chksum]   VARCHAR (64)    NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [sply_ty]  VARCHAR (10)    NULL,
    [pos]      VARCHAR (2)     NULL,
    [flag]     VARCHAR (1)     NULL,
    [gstinid]  INT             NULL,
    [gstr2did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_B2BUR_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_B2BUR_INV_TBL_GSTR2_D_B2BUR] FOREIGN KEY ([b2burid]) REFERENCES [dbo].[TBL_GSTR2_D_B2BUR] ([b2burid])
);

