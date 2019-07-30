CREATE TABLE [dbo].[TBL_GSTR1_EXPA_INV] (
    [invid]        INT             IDENTITY (1, 1) NOT NULL,
    [expaid]       INT             NOT NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (75)    NULL,
    [oinum]        VARCHAR (50)    NULL,
    [oidt]         VARCHAR (50)    NULL,
    [inum]         VARCHAR (50)    NULL,
    [idt]          VARCHAR (50)    NULL,
    [val]          DECIMAL (18, 2) NULL,
    [sbpcode]      VARCHAR (6)     NULL,
    [sbnum]        NVARCHAR (15)   NULL,
    [sbdt]         VARCHAR (50)    NULL,
    [diff_percent] DECIMAL (4, 2)  NULL,
    [gstinid]      INT             NULL,
    [gstr1id]      INT             NULL,
    [createddate]  DATETIME        NULL,
    [createdby]    INT             NULL,
    [custid]       INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_EXPA_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_EXPA_INV_TBL_GSTR1_EXPA] FOREIGN KEY ([expaid]) REFERENCES [dbo].[TBL_GSTR1_EXPA] ([expaid])
);

