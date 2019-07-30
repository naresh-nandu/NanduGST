CREATE TABLE [dbo].[TBL_GSTR3B_D_Tax_Pmt_tx_py] (
    [txpyid]      INT           IDENTITY (1, 1) NOT NULL,
    [taxpmtid]    INT           NOT NULL,
    [liab_ldg_id] BIGINT        NULL,
    [trans_typ]   VARCHAR (50)  NULL,
    [trans_desc]  VARCHAR (MAX) NULL,
    [gstr3bdid]   INT           NULL,
    [gstinid]     INT           NULL,
    PRIMARY KEY CLUSTERED ([txpyid] ASC)
);

