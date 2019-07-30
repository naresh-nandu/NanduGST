CREATE TABLE [dbo].[TBL_GSTR6_CDNR_NT] (
    [invid]        INT             IDENTITY (1, 1) NOT NULL,
    [cdnrid]       INT             NOT NULL,
    [chksum]       VARCHAR (64)    NULL,
    [flag]         VARCHAR (1)     NULL,
    [ntty]         VARCHAR (1)     NULL,
    [nt_num]       VARCHAR (50)    NULL,
    [nt_dt]        VARCHAR (50)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [gstinid]      INT             NULL,
    [gstr6id]      INT             NULL,
    [val]          DECIMAL (18, 2) NULL,
    [uploadStatus] VARCHAR (1)     NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR6_CDNR_NT] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_CDNR_NT_TBL_GSTR6_CDNR] FOREIGN KEY ([cdnrid]) REFERENCES [dbo].[TBL_GSTR6_CDNR] ([cdnrid])
);

