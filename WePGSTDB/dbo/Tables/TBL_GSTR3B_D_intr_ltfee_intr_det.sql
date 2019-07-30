CREATE TABLE [dbo].[TBL_GSTR3B_D_intr_ltfee_intr_det] (
    [intr_detid]   INT             IDENTITY (1, 1) NOT NULL,
    [intr_ltfeeid] INT             NOT NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [gstr3bdid]    INT             NULL,
    [gstinid]      INT             NULL,
    PRIMARY KEY CLUSTERED ([intr_detid] ASC)
);

