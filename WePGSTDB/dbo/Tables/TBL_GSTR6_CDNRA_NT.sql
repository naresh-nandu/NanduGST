CREATE TABLE [dbo].[TBL_GSTR6_CDNRA_NT] (
    [invid]        INT          IDENTITY (1, 1) NOT NULL,
    [cdnraid]      INT          NOT NULL,
    [chksum]       VARCHAR (64) NULL,
    [flag]         VARCHAR (1)  NULL,
    [ntty]         VARCHAR (1)  NULL,
    [nt_num]       VARCHAR (50) NULL,
    [nt_dt]        VARCHAR (50) NULL,
    [ont_num]      VARCHAR (50) NULL,
    [ont_dt]       VARCHAR (50) NULL,
    [inum]         VARCHAR (50) NULL,
    [idt]          VARCHAR (50) NULL,
    [gstinid]      INT          NULL,
    [gstr6id]      INT          NULL,
    [uploadStatus] VARCHAR (1)  NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    CONSTRAINT [PK_TBL_GSTR6_CDNRA_NT] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_CDNRA_NT_TBL_GSTR6_CDNRA] FOREIGN KEY ([cdnraid]) REFERENCES [dbo].[TBL_GSTR6_CDNRA] ([cdnraid])
);

