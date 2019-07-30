CREATE TABLE [dbo].[TBL_GSTR2SummaryActionList] (
    [gstr2sumActionListid] INT             IDENTITY (1, 1) NOT NULL,
    [gstr2sumActionid]     INT             NOT NULL,
    [ctin]                 VARCHAR (50)    NOT NULL,
    [chksum]               VARCHAR (64)    NOT NULL,
    [rc]                   INT             NULL,
    [ttl_val]              DECIMAL (18, 2) NULL,
    [ttl_txpd_igst]        DECIMAL (18, 2) NULL,
    [ttl_txpd_sgst]        DECIMAL (18, 2) NULL,
    [ttl_txpd_cgst]        DECIMAL (18, 2) NULL,
    [ttl_txpd_cess]        DECIMAL (18, 2) NULL,
    [ttl_itcavld_igst]     DECIMAL (18, 2) NULL,
    [ttl_itcavld_sgst]     DECIMAL (18, 2) NULL,
    [ttl_itcavld_cgst]     DECIMAL (18, 2) NULL,
    [ttl_itcavld_cess]     DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([gstr2sumActionListid] ASC),
    FOREIGN KEY ([gstr2sumActionid]) REFERENCES [dbo].[TBL_GSTR2SummaryAction] ([gstr2sumActionid])
);

