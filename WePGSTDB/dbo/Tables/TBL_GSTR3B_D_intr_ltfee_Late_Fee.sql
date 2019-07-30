CREATE TABLE [dbo].[TBL_GSTR3B_D_intr_ltfee_Late_Fee] (
    [late_fee_id]  INT             IDENTITY (1, 1) NOT NULL,
    [intr_ltfeeid] INT             NOT NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [gstr3bdid]    INT             NULL,
    [gstinid]      INT             NULL,
    PRIMARY KEY CLUSTERED ([late_fee_id] ASC)
);

