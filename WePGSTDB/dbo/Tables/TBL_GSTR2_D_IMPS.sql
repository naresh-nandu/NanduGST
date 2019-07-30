CREATE TABLE [dbo].[TBL_GSTR2_D_IMPS] (
    [impsid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr2did] INT          NOT NULL,
    [flag]     VARCHAR (50) NULL,
    [chksum]   VARCHAR (50) NULL,
    [inum]     VARCHAR (50) NULL,
    [idt]      VARCHAR (50) NULL,
    [ival]     VARCHAR (50) NULL,
    [pos]      VARCHAR (2)  NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_IMPS] PRIMARY KEY CLUSTERED ([impsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_IMPS_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

