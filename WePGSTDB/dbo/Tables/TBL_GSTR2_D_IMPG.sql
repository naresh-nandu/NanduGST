CREATE TABLE [dbo].[TBL_GSTR2_D_IMPG] (
    [impgid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr2did]  INT          NOT NULL,
    [boe_num]   VARCHAR (50) NULL,
    [boe_dt]    VARCHAR (50) NULL,
    [boe_val]   VARCHAR (50) NULL,
    [is_sez]    VARCHAR (1)  NULL,
    [stin]      VARCHAR (15) NULL,
    [chksum]    VARCHAR (64) NULL,
    [flag]      VARCHAR (50) NULL,
    [port_code] VARCHAR (6)  NULL,
    [gstinid]   INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_IMPG] PRIMARY KEY CLUSTERED ([impgid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_IMPG_TBL_GSTR2_D] FOREIGN KEY ([gstr2did]) REFERENCES [dbo].[TBL_GSTR2_D] ([gstr2did])
);

