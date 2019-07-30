CREATE TABLE [dbo].[TBL_GSTR1ASummaryAction] (
    [gstr1AsumActionid] INT             IDENTITY (1, 1) NOT NULL,
    [gstr1Asumid]       INT             NOT NULL,
    [sec_nm]            VARCHAR (50)    NULL,
    [chksum]            VARCHAR (64)    NULL,
    [ttl_rec]           INT             NULL,
    [ttl_val]           DECIMAL (18, 2) NULL,
    [ttl_tax]           DECIMAL (18, 2) NULL,
    [ttl_igst]          DECIMAL (18, 2) NULL,
    [ttl_cgst]          DECIMAL (18, 2) NULL,
    [ttl_sgst]          DECIMAL (18, 2) NULL,
    [ttl_cess]          DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([gstr1AsumActionid] ASC),
    FOREIGN KEY ([gstr1Asumid]) REFERENCES [dbo].[TBL_GSTR1ASummary] ([gstr1Asumid])
);

