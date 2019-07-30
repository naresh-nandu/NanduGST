CREATE TABLE [dbo].[TBL_GSTR1_D_TXPD] (
    [txpdid]   INT           IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT           NOT NULL,
    [chksum]   VARCHAR (100) NULL,
    [pos]      VARCHAR (2)   NULL,
    [sply_ty]  VARCHAR (5)   NULL,
    [gstinid]  INT           NULL,
    [flag]     VARCHAR (1)   NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_TXPD] PRIMARY KEY CLUSTERED ([txpdid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_TXPD_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

