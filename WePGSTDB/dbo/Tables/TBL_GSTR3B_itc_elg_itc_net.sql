﻿CREATE TABLE [dbo].[TBL_GSTR3B_itc_elg_itc_net] (
    [itc_netid] INT             IDENTITY (1, 1) NOT NULL,
    [itc_elgid] INT             NOT NULL,
    [ty]        VARCHAR (10)    NULL,
    [iamt]      DECIMAL (18, 2) NULL,
    [camt]      DECIMAL (18, 2) NULL,
    [samt]      DECIMAL (18, 2) NULL,
    [csamt]     DECIMAL (18, 2) NULL,
    [gstr3bid]  INT             NULL,
    [gstinid]   INT             NULL,
    PRIMARY KEY CLUSTERED ([itc_netid] ASC)
);

