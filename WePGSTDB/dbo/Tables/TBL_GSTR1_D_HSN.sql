CREATE TABLE [dbo].[TBL_GSTR1_D_HSN] (
    [hsnid]    INT           IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT           NOT NULL,
    [chksum]   VARCHAR (100) NULL,
    [gstinid]  INT           NULL,
    [flag]     VARCHAR (1)   NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_HSN] PRIMARY KEY CLUSTERED ([hsnid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_HSN_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

