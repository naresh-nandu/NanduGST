CREATE TABLE [dbo].[TBL_GSTR3B_sup_det_osup_nil_exmp] (
    [osup_nil_exmpid] INT             IDENTITY (1, 1) NOT NULL,
    [sup_detid]       INT             NOT NULL,
    [txval]           DECIMAL (18, 2) NULL,
    [gstr3bid]        INT             NULL,
    [gstinid]         INT             NULL,
    PRIMARY KEY CLUSTERED ([osup_nil_exmpid] ASC)
);

