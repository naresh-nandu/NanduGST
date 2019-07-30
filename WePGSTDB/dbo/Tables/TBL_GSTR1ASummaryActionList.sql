CREATE TABLE [dbo].[TBL_GSTR1ASummaryActionList] (
    [gstr1AsumActionListid] INT             IDENTITY (1, 1) NOT NULL,
    [gstr1AsumActionid]     INT             NOT NULL,
    [ctin]                  VARCHAR (50)    NOT NULL,
    [chksum]                VARCHAR (64)    NOT NULL,
    [ttl_rec]               INT             NULL,
    [ttl_val]               DECIMAL (18, 2) NULL,
    [ttl_tax]               DECIMAL (18, 2) NULL,
    [ttl_igst]              DECIMAL (18, 2) NULL,
    [ttl_cgst]              DECIMAL (18, 2) NULL,
    [ttl_sgst]              DECIMAL (18, 2) NULL,
    [ttl_cess]              DECIMAL (18, 2) NULL,
    PRIMARY KEY CLUSTERED ([gstr1AsumActionListid] ASC),
    FOREIGN KEY ([gstr1AsumActionid]) REFERENCES [dbo].[TBL_GSTR1ASummaryAction] ([gstr1AsumActionid])
);

