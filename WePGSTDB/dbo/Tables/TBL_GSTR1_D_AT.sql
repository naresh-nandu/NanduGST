CREATE TABLE [dbo].[TBL_GSTR1_D_AT] (
    [atid]     INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [chksum]   VARCHAR (64) NULL,
    [pos]      VARCHAR (2)  NULL,
    [sply_ty]  VARCHAR (5)  NULL,
    [gstinid]  INT          NULL,
    [flag]     VARCHAR (1)  NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_AT] PRIMARY KEY CLUSTERED ([atid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_AT_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

