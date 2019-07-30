CREATE TABLE [dbo].[TBL_GSTR1_D_B2CL_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2clid]   INT             NOT NULL,
    [flag]     VARCHAR (20)    NULL,
    [chksum]   VARCHAR (100)   NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [etin]     VARCHAR (15)    NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_B2CL_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2CL_INV_TBL_GSTR1_D_B2CL] FOREIGN KEY ([b2clid]) REFERENCES [dbo].[TBL_GSTR1_D_B2CL] ([b2clid])
);

