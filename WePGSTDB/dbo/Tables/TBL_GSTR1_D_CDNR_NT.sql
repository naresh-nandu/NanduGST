CREATE TABLE [dbo].[TBL_GSTR1_D_CDNR_NT] (
    [ntid]     INT             IDENTITY (1, 1) NOT NULL,
    [cdnrid]   INT             NOT NULL,
    [flag]     VARCHAR (1)     NULL,
    [cflag]    VARCHAR (1)     NULL,
    [opd]      VARCHAR (6)     NULL,
    [updby]    VARCHAR (1)     NULL,
    [chksum]   VARCHAR (64)    NULL,
    [ntty]     VARCHAR (1)     NULL,
    [nt_num]   VARCHAR (16)    NULL,
    [nt_dt]    VARCHAR (10)    NULL,
    [rsn]      VARCHAR (50)    NULL,
    [p_gst]    VARCHAR (1)     NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_CDNR_NT] PRIMARY KEY CLUSTERED ([ntid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_CDNR_NT_TBL_GSTR1_D_CDNR] FOREIGN KEY ([cdnrid]) REFERENCES [dbo].[TBL_GSTR1_D_CDNR] ([cdnrid])
);

