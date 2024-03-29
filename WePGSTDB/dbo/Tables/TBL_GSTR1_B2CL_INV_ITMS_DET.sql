﻿CREATE TABLE [dbo].[TBL_GSTR1_B2CL_INV_ITMS_DET] (
    [itmsdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]    INT             NOT NULL,
    [txval]     DECIMAL (18, 2) NULL,
    [rt]        DECIMAL (18, 2) NULL,
    [iamt]      DECIMAL (18, 2) NULL,
    [csamt]     DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr1id]   INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CL_INV_ITMS_DET] PRIMARY KEY CLUSTERED ([itmsdetid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CL_INV_ITMS_DET_TBL_GSTR1_B2CL_INV_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR1_B2CL_INV_ITMS] ([itmsid])
);

