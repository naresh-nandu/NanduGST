CREATE TABLE [dbo].[TBL_GSTR6A_CDNR_NT] (
    [ntid]     INT          IDENTITY (1, 1) NOT NULL,
    [cdnrid]   INT          NOT NULL,
    [flag]     VARCHAR (1)  NULL,
    [chksum]   VARCHAR (75) NULL,
    [ntty]     VARCHAR (1)  NULL,
    [nt_num]   VARCHAR (50) NULL,
    [nt_dt]    VARCHAR (50) NULL,
    [inum]     VARCHAR (50) NULL,
    [idt]      VARCHAR (50) NULL,
    [pos]      VARCHAR (2)  NULL,
    [gstinid]  INT          NULL,
    [gstr6aid] INT          NULL,
    PRIMARY KEY CLUSTERED ([ntid] ASC)
);

