CREATE TABLE [dbo].[TBL_GSTR2_CDNUR] (
    [cdnurid]      INT             IDENTITY (1, 1) NOT NULL,
    [gstr2id]      INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [rtin]         VARCHAR (15)    NULL,
    [ntty]         VARCHAR (1)     NULL,
    [nt_num]       VARCHAR (50)    NULL,
    [nt_dt]        VARCHAR (50)    NULL,
    [rsn]          VARCHAR (50)    NULL,
    [p_gst]        VARCHAR (1)     NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [gstinid]      INT             NULL,
    [val]          DECIMAL (18, 2) NULL,
    [inv_typ]      VARCHAR (5)     NULL,
    [uploadStatus] VARCHAR (1)     NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_CDNUR] PRIMARY KEY CLUSTERED ([cdnurid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_CDNUR_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

