CREATE TABLE [dbo].[TBL_GSTR1_CDNR_NT] (
    [ntid]        INT             IDENTITY (1, 1) NOT NULL,
    [cdnrid]      INT             NOT NULL,
    [flag]        VARCHAR (1)     NULL,
    [chksum]      VARCHAR (75)    NULL,
    [ntty]        VARCHAR (1)     NULL,
    [nt_num]      VARCHAR (50)    NULL,
    [nt_dt]       VARCHAR (50)    NULL,
    [inum]        VARCHAR (50)    NULL,
    [idt]         VARCHAR (50)    NULL,
    [val]         DECIMAL (18, 2) NULL,
    [gstinid]     INT             NULL,
    [gstr1id]     INT             NULL,
    [p_gst]       VARCHAR (1)     NULL,
    [rsn]         VARCHAR (50)    NULL,
    [createddate] DATETIME        NULL,
    [createdby]   INT             NULL,
    [custid]      INT             NULL,
    PRIMARY KEY CLUSTERED ([ntid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNR_NT_TBL_GSTR1_CDNR] FOREIGN KEY ([cdnrid]) REFERENCES [dbo].[TBL_GSTR1_CDNR] ([cdnrid])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_CDNR_NT_453C82CE5AAE8350B8E829231DD7269B]
    ON [dbo].[TBL_GSTR1_CDNR_NT]([cdnrid] ASC, [nt_dt] ASC, [nt_num] ASC)
    INCLUDE([flag], [gstr1id], [idt], [inum], [ntty], [val]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_CDNR_NT_EDC3616BCB1414DD56D1971F50780353]
    ON [dbo].[TBL_GSTR1_CDNR_NT]([flag] ASC)
    INCLUDE([cdnrid], [gstinid], [ntty]);

