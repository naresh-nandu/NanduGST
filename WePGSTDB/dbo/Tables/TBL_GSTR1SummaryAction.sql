CREATE TABLE [dbo].[TBL_GSTR1SummaryAction] (
    [gstr1sumActionid]  INT             IDENTITY (1, 1) NOT NULL,
    [gstr1sumid]        INT             NOT NULL,
    [sec_nm]            VARCHAR (50)    NULL,
    [chksum]            VARCHAR (64)    NULL,
    [ttl_rec]           INT             NULL,
    [ttl_val]           DECIMAL (18, 2) NULL,
    [ttl_tax]           DECIMAL (18, 2) NULL,
    [ttl_igst]          DECIMAL (18, 2) NULL,
    [ttl_cgst]          DECIMAL (18, 2) NULL,
    [ttl_sgst]          DECIMAL (18, 2) NULL,
    [ttl_cess]          DECIMAL (18, 2) NULL,
    [ttl_nilsup_amt]    DECIMAL (18, 2) NULL,
    [ttl_expt_amt]      DECIMAL (18, 2) NULL,
    [ttl_ngsup_amt]     DECIMAL (18, 2) NULL,
    [ttl_doc_issued]    DECIMAL (18, 2) NULL,
    [ttl_doc_cancelled] DECIMAL (18, 2) NULL,
    [net_doc_issued]    DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([gstr1sumActionid] ASC),
    FOREIGN KEY ([gstr1sumid]) REFERENCES [dbo].[TBL_GSTR1Summary] ([gstr1sumid])
);

