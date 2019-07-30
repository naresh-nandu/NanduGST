CREATE TABLE [dbo].[TBL_GSTR1_D_B2B] (
    [b2bid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [ctin]     VARCHAR (50) NULL,
    [cfs]      VARCHAR (20) NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_B2B] PRIMARY KEY CLUSTERED ([b2bid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2B_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

