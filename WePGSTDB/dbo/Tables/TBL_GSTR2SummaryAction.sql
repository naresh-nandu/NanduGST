CREATE TABLE [dbo].[TBL_GSTR2SummaryAction] (
    [gstr2sumActionid] INT             IDENTITY (1, 1) NOT NULL,
    [gstr2sumid]       INT             NOT NULL,
    [section_name]     VARCHAR (50)    NOT NULL,
    [chksum]           VARCHAR (64)    NOT NULL,
    [rc]               INT             NULL,
    [ttl_val]          DECIMAL (18, 2) NULL,
    [ttl_txpd_igst]    DECIMAL (18, 2) NULL,
    [ttl_txpd_sgst]    DECIMAL (18, 2) NULL,
    [ttl_txpd_cgst]    DECIMAL (18, 2) NULL,
    [ttl_txpd_cess]    DECIMAL (18, 2) NULL,
    [ttl_itcavld_igst] DECIMAL (18, 2) NULL,
    [ttl_itcavld_sgst] DECIMAL (18, 2) NULL,
    [ttl_itcavld_cgst] DECIMAL (18, 2) NULL,
    [ttl_itcavld_cess] DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([gstr2sumActionid] ASC),
    FOREIGN KEY ([gstr2sumid]) REFERENCES [dbo].[TBL_GSTR2Summary] ([gstr2sumid])
);

