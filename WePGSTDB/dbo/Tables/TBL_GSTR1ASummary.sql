CREATE TABLE [dbo].[TBL_GSTR1ASummary] (
    [gstr1Asumid] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]       VARCHAR (50) NOT NULL,
    [gstinid]     INT          NULL,
    [ret_period]  VARCHAR (50) NULL,
    [chksum]      VARCHAR (64) NOT NULL,
    [summ_typ]    VARCHAR (1)  NULL,
    PRIMARY KEY CLUSTERED ([gstr1Asumid] ASC),
    FOREIGN KEY ([gstinid]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId])
);

