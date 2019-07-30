CREATE TABLE [dbo].[TBL_GSTR6A_B2B_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]    INT             NOT NULL,
    [chksum]   VARCHAR (64)    NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [flag]     VARCHAR (1)     NULL,
    [gstinid]  INT             NULL,
    [gstr6aid] INT             NULL,
    PRIMARY KEY CLUSTERED ([invid] ASC)
);

