CREATE TABLE [dbo].[TBL_GSTR3B_D_Tax_Pmt_txpaidAcredit] (
    [txpaidAcreditid] INT             IDENTITY (1, 1) NOT NULL,
    [taxpmtid]        INT             NOT NULL,
    [i_pdi]           DECIMAL (18, 2) NULL,
    [i_pdc]           DECIMAL (18, 2) NULL,
    [i_pds]           DECIMAL (18, 2) NULL,
    [c_pdi]           DECIMAL (18, 2) NULL,
    [c_pdc]           DECIMAL (18, 2) NULL,
    [s_pdi]           DECIMAL (18, 2) NULL,
    [s_pds]           DECIMAL (18, 2) NULL,
    [cs_pdcs]         DECIMAL (18, 2) NULL,
    [gstr3bdid]       INT             NULL,
    [gstinid]         INT             NULL,
    PRIMARY KEY CLUSTERED ([txpaidAcreditid] ASC)
);

