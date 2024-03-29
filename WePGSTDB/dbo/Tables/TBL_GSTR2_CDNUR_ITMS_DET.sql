﻿CREATE TABLE [dbo].[TBL_GSTR2_CDNUR_ITMS_DET] (
    [itmdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr2id]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_CDNUR_ITMS_DET] PRIMARY KEY CLUSTERED ([itmdetid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_CDNUR_ITMS_DET_TBL_GSTR2_CDNUR_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR2_CDNUR_ITMS] ([itmsid])
);

