﻿CREATE TABLE [dbo].[TBL_GSTR3B_D_sup_det_osup_det] (
    [osup_detid] INT             IDENTITY (1, 1) NOT NULL,
    [sup_detid]  INT             NOT NULL,
    [txval]      DECIMAL (18, 2) NULL,
    [iamt]       DECIMAL (18, 2) NULL,
    [camt]       DECIMAL (18, 2) NULL,
    [samt]       DECIMAL (18, 2) NULL,
    [csamt]      DECIMAL (18, 2) NULL,
    [gstr3bdid]  INT             NULL,
    [gstinid]    INT             NULL,
    PRIMARY KEY CLUSTERED ([osup_detid] ASC)
);

