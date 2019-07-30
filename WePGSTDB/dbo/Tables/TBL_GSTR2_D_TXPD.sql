CREATE TABLE [dbo].[TBL_GSTR2_D_TXPD] (
    [txpdid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr2did] INT          NOT NULL,
    [chksum]   VARCHAR (50) NULL,
    [pos]      VARCHAR (2)  NULL,
    [sply_ty]  VARCHAR (5)  NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_TXPD] PRIMARY KEY CLUSTERED ([txpdid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_TXPD_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

