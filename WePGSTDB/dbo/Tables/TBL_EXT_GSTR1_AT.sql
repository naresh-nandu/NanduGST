CREATE TABLE [dbo].[TBL_EXT_GSTR1_AT] (
    [atid]         INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [gt]           DECIMAL (18, 2) NULL,
    [cur_gt]       DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [pos]          VARCHAR (2)     NULL,
    [sply_ty]      VARCHAR (5)     NULL,
    [rt]           DECIMAL (18, 2) NULL,
    [ad_amt]       DECIMAL (18, 2) NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createdDate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [createdby]    INT             NULL,
    PRIMARY KEY CLUSTERED ([atid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_AT_42C17A00A66D1AA592362E86CE926BE1]
    ON [dbo].[TBL_EXT_GSTR1_AT]([referenceno] ASC, [rowstatus] ASC, [sourcetype] ASC)
    INCLUDE([ad_amt], [camt], [csamt], [cur_gt], [errormessage], [fp], [gstin], [gt], [iamt], [pos], [rt], [samt], [sply_ty]);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_AT_IDX_FP]
    ON [dbo].[TBL_EXT_GSTR1_AT]([fp] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_AT_IDX_GSTIN]
    ON [dbo].[TBL_EXT_GSTR1_AT]([gstin] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_AT_IDX_ReferenceNo]
    ON [dbo].[TBL_EXT_GSTR1_AT]([referenceno] ASC);


GO
CREATE NONCLUSTERED INDEX [TBL_EXT_GSTR1_AT_IDX_RowStatus]
    ON [dbo].[TBL_EXT_GSTR1_AT]([rowstatus] ASC);

