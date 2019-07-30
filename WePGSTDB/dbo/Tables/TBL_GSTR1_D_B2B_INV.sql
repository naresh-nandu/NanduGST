CREATE TABLE [dbo].[TBL_GSTR1_D_B2B_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]    INT             NOT NULL,
    [flag]     VARCHAR (20)    NULL,
    [updby]    VARCHAR (20)    NULL,
    [chksum]   VARCHAR (100)   NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [pos]      VARCHAR (2)     NULL,
    [rchrg]    VARCHAR (3)     NULL,
    [etin]     VARCHAR (15)    NULL,
    [inv_typ]  VARCHAR (2)     NULL,
    [cflag]    VARCHAR (1)     NULL,
    [opd]      VARCHAR (6)     NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B_B2B_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2B_INV_TBL_GSTR1_D_B2B] FOREIGN KEY ([b2bid]) REFERENCES [dbo].[TBL_GSTR1_D_B2B] ([b2bid])
);

