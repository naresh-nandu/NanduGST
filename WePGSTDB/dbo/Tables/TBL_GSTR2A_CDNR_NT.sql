CREATE TABLE [dbo].[TBL_GSTR2A_CDNR_NT] (
    [ntid]     INT             IDENTITY (1, 1) NOT NULL,
    [cdnrid]   INT             NOT NULL,
    [flag]     VARCHAR (1)     NULL,
    [chksum]   VARCHAR (75)    NULL,
    [ntty]     VARCHAR (1)     NULL,
    [nt_num]   VARCHAR (50)    NULL,
    [nt_dt]    VARCHAR (50)    NULL,
    [rsn]      VARCHAR (50)    NULL,
    [p_gst]    VARCHAR (1)     NULL,
    [inum]     VARCHAR (50)    NULL,
    [idt]      VARCHAR (50)    NULL,
    [val]      DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr2aid] INT             NULL,
    PRIMARY KEY CLUSTERED ([ntid] ASC)
);

