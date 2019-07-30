CREATE TABLE [dbo].[TBL_GSTR1_CDNUR] (
    [cdnurid]     INT             IDENTITY (1, 1) NOT NULL,
    [gstr1id]     INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [typ]         VARCHAR (6)     NULL,
    [ntty]        VARCHAR (1)     NULL,
    [nt_num]      VARCHAR (50)    NULL,
    [nt_dt]       VARCHAR (50)    NULL,
    [inum]        VARCHAR (50)    NULL,
    [idt]         VARCHAR (50)    NULL,
    [val]         DECIMAL (18, 2) NULL,
    [gstinid]     INT             NULL,
    [p_gst]       VARCHAR (1)     NULL,
    [rsn]         VARCHAR (50)    NULL,
    [chksum]      VARCHAR (75)    NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    PRIMARY KEY CLUSTERED ([cdnurid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNUR_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

