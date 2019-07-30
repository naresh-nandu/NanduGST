CREATE TABLE [dbo].[TBL_GSTR1_D_NIL] (
    [nilid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [chksum]   VARCHAR (64) NULL,
    [gstinid]  INT          NULL,
    [flag]     VARCHAR (1)  NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_NIL] PRIMARY KEY CLUSTERED ([nilid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_NIL_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

