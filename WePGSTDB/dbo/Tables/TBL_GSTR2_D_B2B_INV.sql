CREATE TABLE [dbo].[TBL_GSTR2_D_B2B_INV] (
    [invid]    INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]    INT             NOT NULL,
    [flag]     VARCHAR (1)     NULL,
    [chksum]   NVARCHAR (64)   NULL,
    [inum]     NVARCHAR (50)   NULL,
    [idt]      NVARCHAR (50)   NULL,
    [val]      DECIMAL (18, 2) NULL,
    [pos]      NVARCHAR (50)   NULL,
    [rchrg]    NVARCHAR (50)   NULL,
    [updby]    NVARCHAR (50)   NULL,
    [cflag]    NVARCHAR (50)   NULL,
    [opd]      NVARCHAR (10)   NULL,
    [inv_ty]   NVARCHAR (10)   NULL,
    [gstr2did] INT             NULL,
    [gstinid]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_B2B_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_B2B_INV_TBL_GSTR2_D_B2B] FOREIGN KEY ([b2bid]) REFERENCES [dbo].[TBL_GSTR2_D_B2B] ([b2bid])
);

