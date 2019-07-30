CREATE TABLE [dbo].[TBL_GSTR1_D_CDNR_NT_ITMS_DET] (
    [itmsdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]    INT             NOT NULL,
    [rt]        DECIMAL (18, 2) NULL,
    [txval]     DECIMAL (18, 2) NULL,
    [iamt]      DECIMAL (18, 2) NULL,
    [camt]      DECIMAL (18, 2) NULL,
    [samt]      DECIMAL (18, 2) NULL,
    [csamt]     DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr1did]  INT             NULL,
    PRIMARY KEY CLUSTERED ([itmsdetid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNR_NT_ITMS_DET_TBL_GSTR1_D_CDNR_NT_ITMS] FOREIGN KEY ([itmsid]) REFERENCES [dbo].[TBL_GSTR1_D_CDNR_NT_ITMS] ([itmsid])
);

