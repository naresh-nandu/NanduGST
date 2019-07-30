CREATE TABLE [dbo].[TBL_GSTR1_HSN_DATA] (
    [dataid]      INT             IDENTITY (1, 1) NOT NULL,
    [hsnid]       INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [chksum]      VARCHAR (75)    NULL,
    [num]         INT             NULL,
    [hsn_sc]      VARCHAR (10)    NULL,
    [desc]        VARCHAR (50)    NULL,
    [uqc]         VARCHAR (50)    NULL,
    [qty]         DECIMAL (18, 2) NULL,
    [val]         DECIMAL (18, 2) NULL,
    [txval]       DECIMAL (18, 2) NULL,
    [iamt]        DECIMAL (18, 2) NULL,
    [camt]        DECIMAL (18, 2) NULL,
    [samt]        DECIMAL (18, 2) NULL,
    [csamt]       DECIMAL (18, 2) NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_HSN_DATA] PRIMARY KEY CLUSTERED ([dataid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_HSN_DATA_TBL_GSTR1_HSN] FOREIGN KEY ([hsnid]) REFERENCES [dbo].[TBL_GSTR1_HSN] ([hsnid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_HSN_DATA_211AEEB2542D87640832F72071BCC1B2]
    ON [dbo].[TBL_GSTR1_HSN_DATA]([hsnid] ASC, [hsn_sc] ASC, [qty] ASC, [val] ASC)
    INCLUDE([camt], [csamt], [desc], [flag], [iamt], [samt], [txval], [uqc]);

