CREATE TABLE [dbo].[TBL_GSTR1SummaryActionList] (
    [gstr1sumActionListid] INT             IDENTITY (1, 1) NOT NULL,
    [gstr1sumActionid]     INT             NOT NULL,
    [ctin]                 VARCHAR (50)    NULL,
    [state_cd]             VARCHAR (50)    NULL,
    [chksum]               VARCHAR (64)    NULL,
    [ttl_rec]              INT             NULL,
    [ttl_val]              DECIMAL (18, 2) NULL,
    [ttl_tax]              DECIMAL (18, 2) NULL,
    [ttl_igst]             DECIMAL (18, 2) NULL,
    [ttl_cgst]             DECIMAL (18, 2) NULL,
    [ttl_sgst]             DECIMAL (18, 2) NULL,
    [ttl_cess]             DECIMAL (18, 2) NULL,
    [sec_nm]               VARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([gstr1sumActionListid] ASC),
    FOREIGN KEY ([gstr1sumActionid]) REFERENCES [dbo].[TBL_GSTR1SummaryAction] ([gstr1sumActionid])
);

