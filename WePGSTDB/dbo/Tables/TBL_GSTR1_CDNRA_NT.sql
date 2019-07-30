CREATE TABLE [dbo].[TBL_GSTR1_CDNRA_NT] (
    [ntid]         INT             IDENTITY (1, 1) NOT NULL,
    [cdnraid]      INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (75)    NULL,
    [ont_num]      VARCHAR (50)    NULL,
    [ont_dt]       VARCHAR (50)    NULL,
    [ntty]         VARCHAR (1)     NULL,
    [nt_num]       VARCHAR (50)    NULL,
    [nt_dt]        VARCHAR (50)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [p_gst]        VARCHAR (1)     NULL,
    [val]          DECIMAL (18, 2) NULL,
    [diff_percent] DECIMAL (4, 2)  NULL,
    [gstinid]      INT             NULL,
    [gstr1id]      INT             NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    PRIMARY KEY CLUSTERED ([ntid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNRA_NT_TBL_GSTR1_CDNRA] FOREIGN KEY ([cdnraid]) REFERENCES [dbo].[TBL_GSTR1_CDNRA] ([cdnraid])
);

