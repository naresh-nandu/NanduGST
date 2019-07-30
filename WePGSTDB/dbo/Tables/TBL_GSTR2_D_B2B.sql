CREATE TABLE [dbo].[TBL_GSTR2_D_B2B] (
    [b2bid]    INT           IDENTITY (1, 1) NOT NULL,
    [gstr2did] INT           NOT NULL,
    [ctin]     NVARCHAR (15) NULL,
    [cfs]      VARCHAR (10)  NULL,
    [gstinid]  INT           NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_B2B] PRIMARY KEY CLUSTERED ([b2bid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_B2B_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

