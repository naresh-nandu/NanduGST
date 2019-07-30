﻿CREATE TABLE [dbo].[TBL_GSTR6_ISDA_DOCLST] (
    [invid]        INT             IDENTITY (1, 1) NOT NULL,
    [isdaid]       INT             NOT NULL,
    [chksum]       VARCHAR (64)    NULL,
    [flag]         VARCHAR (1)     NULL,
    [cpty]         VARCHAR (15)    NULL,
    [statecd]      VARCHAR (2)     NULL,
    [isd_docty]    VARCHAR (60)    NULL,
    [rdocnum]      VARCHAR (10)    NULL,
    [rdocdt]       VARCHAR (50)    NULL,
    [odocnum]      VARCHAR (10)    NULL,
    [odocdt]       VARCHAR (50)    NULL,
    [rcrdnum]      VARCHAR (16)    NULL,
    [rcrddt]       VARCHAR (50)    NULL,
    [ocrdnum]      VARCHAR (16)    NULL,
    [ocrddt]       VARCHAR (50)    NULL,
    [iamti]        DECIMAL (11, 2) NULL,
    [iamts]        DECIMAL (11, 2) NULL,
    [iamtc]        DECIMAL (11, 2) NULL,
    [samts]        DECIMAL (11, 2) NULL,
    [samti]        DECIMAL (11, 2) NULL,
    [camti]        DECIMAL (11, 2) NULL,
    [camtc]        DECIMAL (11, 2) NULL,
    [csamt]        DECIMAL (11, 2) NULL,
    [gstinid]      INT             NULL,
    [gstr6id]      INT             NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    [CompCode]     VARCHAR (50)    NULL,
    [UnitCode]     VARCHAR (50)    NULL,
    [ReceivedBy]   VARCHAR (50)    NULL,
    [ReceivedDate] VARCHAR (10)    NULL,
    CONSTRAINT [PK_TBL_GSTR6_ISDA_DOCLST] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_ISDA_DOCLST_TBL_GSTR6_ISDA] FOREIGN KEY ([isdaid]) REFERENCES [dbo].[TBL_GSTR6_ISDA] ([isdaid])
);

