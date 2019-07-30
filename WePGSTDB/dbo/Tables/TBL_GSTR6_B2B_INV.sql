CREATE TABLE [dbo].[TBL_GSTR6_B2B_INV] (
    [invid]        INT             IDENTITY (1, 1) NOT NULL,
    [b2bid]        INT             NOT NULL,
    [chksum]       VARCHAR (64)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [val]          DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [pos]          VARCHAR (2)     NULL,
    [gstinid]      INT             NULL,
    [gstr6id]      INT             NULL,
    [uploadStatus] VARCHAR (1)     NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR6_B2B_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_B2B_INV_TBL_GSTR6_B2B] FOREIGN KEY ([b2bid]) REFERENCES [dbo].[TBL_GSTR6_B2B] ([b2bid])
);

