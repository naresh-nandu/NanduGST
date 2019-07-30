CREATE TABLE [dbo].[TBL_GSTR1_D_EXP_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [expid]    INT             NOT NULL,
    [flag]     VARCHAR (1)     NULL,
    [chksum]   VARCHAR (64)    NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [sbpcode]  VARCHAR (50)    NULL,
    [sbnum]    VARCHAR (50)    NULL,
    [sbdt]     VARCHAR (50)    NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_EXP_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_EXP_INV_TBL_GSTR1_D_EXP] FOREIGN KEY ([expid]) REFERENCES [dbo].[TBL_GSTR1_D_EXP] ([expid])
);

