CREATE TABLE [dbo].[TBL_GSTR1_B2BA_INV] (
    [invid]        INT             IDENTITY (1, 1) NOT NULL,
    [b2baid]       INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (75)    NULL,
    [oinum]        VARCHAR (50)    NULL,
    [oidt]         VARCHAR (50)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [val]          DECIMAL (18, 2) NULL,
    [pos]          VARCHAR (2)     NULL,
    [rchrg]        VARCHAR (1)     NULL,
    [etin]         VARCHAR (15)    NULL,
    [inv_typ]      VARCHAR (5)     NULL,
    [diff_percent] DECIMAL (4, 2)  NULL,
    [gstinid]      INT             NULL,
    [gstr1id]      INT             NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2BA_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2BA_INV_TBL_GSTR1_B2BA] FOREIGN KEY ([b2baid]) REFERENCES [dbo].[TBL_GSTR1_B2BA] ([b2baid])
);

