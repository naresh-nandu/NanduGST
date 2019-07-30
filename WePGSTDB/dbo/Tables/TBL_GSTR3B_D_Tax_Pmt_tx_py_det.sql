CREATE TABLE [dbo].[TBL_GSTR3B_D_Tax_Pmt_tx_py_det] (
    [detid]     INT             IDENTITY (1, 1) NOT NULL,
    [txpyid]    INT             NOT NULL,
    [taxname]   VARCHAR (10)    NULL,
    [intr]      DECIMAL (18, 2) NULL,
    [tx]        DECIMAL (18, 2) NULL,
    [fee]       DECIMAL (18, 2) NULL,
    [gstr3bdid] INT             NULL,
    [gstinid]   INT             NULL,
    PRIMARY KEY CLUSTERED ([detid] ASC)
);

