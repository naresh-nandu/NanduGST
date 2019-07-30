CREATE TABLE [dbo].[TBL_GSTR1Summary] (
    [gstr1sumid] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]      VARCHAR (50) NOT NULL,
    [gstinid]    INT          NULL,
    [ret_period] VARCHAR (6)  NULL,
    [chksum]     VARCHAR (64) NULL,
    [summ_typ]   VARCHAR (1)  NULL,
    PRIMARY KEY CLUSTERED ([gstr1sumid] ASC),
    FOREIGN KEY ([gstinid]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId])
);

