﻿CREATE TABLE [dbo].[TBL_GSTR3B_sup_det_osup_nongst] (
    [osup_nongstid] INT             IDENTITY (1, 1) NOT NULL,
    [sup_detid]     INT             NOT NULL,
    [txval]         DECIMAL (18, 2) NULL,
    [gstr3bid]      INT             NULL,
    [gstinid]       INT             NULL,
    PRIMARY KEY CLUSTERED ([osup_nongstid] ASC)
);

