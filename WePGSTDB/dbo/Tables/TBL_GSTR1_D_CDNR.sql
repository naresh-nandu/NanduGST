CREATE TABLE [dbo].[TBL_GSTR1_D_CDNR] (
    [cdnrid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [ctin]     VARCHAR (15) NULL,
    [cfs]      VARCHAR (1)  NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_CDNR] PRIMARY KEY CLUSTERED ([cdnrid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNR_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

