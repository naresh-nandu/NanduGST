CREATE TABLE [dbo].[TBL_EXT_GSTR1_HSN] (
    [hsnid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [gt]           DECIMAL (18, 2) NULL,
    [cur_gt]       DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [hsn_sc]       VARCHAR (50)    NULL,
    [descs]        VARCHAR (50)    NULL,
    [uqc]          VARCHAR (50)    NULL,
    [qty]          DECIMAL (18, 2) NULL,
    [val]          DECIMAL (18, 2) NULL,
    [txval]        DECIMAL (18, 2) NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createddate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    PRIMARY KEY CLUSTERED ([hsnid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_HSN_0BEB983681FCFB7E2AAA59B2CD3BDA97]
    ON [dbo].[TBL_EXT_GSTR1_HSN]([fp] ASC, [gstin] ASC, [hsn_sc] ASC, [descs] ASC, [rowstatus] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_HSN_A0B3D8A838C430F84DF423178056A516]
    ON [dbo].[TBL_EXT_GSTR1_HSN]([rowstatus] ASC, [referenceno] ASC, [sourcetype] ASC)
    INCLUDE([camt], [csamt], [cur_gt], [descs], [errormessage], [fp], [gstin], [gt], [hsn_sc], [iamt], [qty], [samt], [txval], [uqc], [val]);

