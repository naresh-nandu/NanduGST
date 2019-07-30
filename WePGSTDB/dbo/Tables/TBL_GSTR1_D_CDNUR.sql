CREATE TABLE [dbo].[TBL_GSTR1_D_CDNUR] (
    [cdnurid]  INT             IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT             NOT NULL,
    [flag]     VARCHAR (1)     NULL,
    [chksum]   VARCHAR (64)    NULL,
    [typ]      VARCHAR (6)     NULL,
    [ntty]     VARCHAR (1)     NULL,
    [nt_num]   VARCHAR (50)    NULL,
    [nt_dt]    VARCHAR (50)    NULL,
    [rsn]      VARCHAR (50)    NULL,
    [p_gst]    VARCHAR (1)     NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    PRIMARY KEY CLUSTERED ([cdnurid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNUR_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

