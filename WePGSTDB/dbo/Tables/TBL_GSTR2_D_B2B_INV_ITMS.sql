CREATE TABLE [dbo].[TBL_GSTR2_D_B2B_INV_ITMS] (
    [itmsid]   INT           IDENTITY (1, 1) NOT NULL,
    [invid]    INT           NOT NULL,
    [num]      NVARCHAR (50) NULL,
    [gstr2did] INT           NULL,
    [gstinid]  INT           NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_B2B_INV_ITMS] PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_B2B_INV_ITMS_TBL_GSTR2_D_B2B_INV] FOREIGN KEY ([invid]) REFERENCES [dbo].[TBL_GSTR2_D_B2B_INV] ([invid])
);

