CREATE TABLE [dbo].[TBL_EXT_GSTR1_DOC] (
    [docid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [gt]           DECIMAL (18, 2) NULL,
    [cur_gt]       DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [doc_num]      INT             NULL,
    [doc_typ]      VARCHAR (50)    NULL,
    [num]          INT             NULL,
    [froms]        VARCHAR (50)    NULL,
    [tos]          VARCHAR (50)    NULL,
    [totnum]       INT             NULL,
    [cancel]       INT             NULL,
    [net_issue]    INT             NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createddate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [createdby]    INT             NULL
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_DOC_373829CCD786C6AABF248C3F46C894DC]
    ON [dbo].[TBL_EXT_GSTR1_DOC]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([cancel], [doc_num], [doc_typ], [docid], [errormessage], [fp], [froms], [gstin], [net_issue], [tos], [totnum]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_DOC_748FDD2BA03770324BC840C3DAFD0B81]
    ON [dbo].[TBL_EXT_GSTR1_DOC]([referenceno] ASC, [rowstatus] ASC, [sourcetype] ASC)
    INCLUDE([cancel], [cur_gt], [doc_num], [doc_typ], [errormessage], [fp], [froms], [gstin], [gt], [net_issue], [num], [tos], [totnum]);

