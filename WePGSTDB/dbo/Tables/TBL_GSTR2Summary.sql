CREATE TABLE [dbo].[TBL_GSTR2Summary] (
    [gstr2sumid] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]      VARCHAR (50) NOT NULL,
    [gstinid]    INT          NULL,
    [ret_period] VARCHAR (50) NULL,
    [chksum]     VARCHAR (64) NOT NULL,
    PRIMARY KEY CLUSTERED ([gstr2sumid] ASC),
    FOREIGN KEY ([gstinid]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId])
);

