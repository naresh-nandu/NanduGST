CREATE TABLE [dbo].[TBL_GSTR3B_D_Tax_Pmt_txpaidAcash] (
    [txpaidAcashid] INT             IDENTITY (1, 1) NOT NULL,
    [taxpmtid]      INT             NOT NULL,
    [liab_ldg_id]   BIGINT          NULL,
    [trans_typ]     VARCHAR (50)    NULL,
    [ipd]           DECIMAL (18, 2) NULL,
    [cpd]           DECIMAL (18, 2) NULL,
    [spd]           DECIMAL (18, 2) NULL,
    [cspd]          DECIMAL (18, 2) NULL,
    [i_intrpd]      DECIMAL (18, 2) NULL,
    [c_intrpd]      DECIMAL (18, 2) NULL,
    [s_intrpd]      DECIMAL (18, 2) NULL,
    [cs_intrpd]     DECIMAL (18, 2) NULL,
    [c_lfeepd]      DECIMAL (18, 2) NULL,
    [s_lfeepd]      DECIMAL (18, 2) NULL,
    [gstr3bdid]     INT             NULL,
    [gstinid]       INT             NULL,
    PRIMARY KEY CLUSTERED ([txpaidAcashid] ASC)
);

