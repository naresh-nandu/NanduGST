CREATE TABLE [dbo].[TBL_EXT_GSTR1_NIL] (
    [nilid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [gt]           DECIMAL (18, 2) NULL,
    [cur_gt]       DECIMAL (18, 2) NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [nil_amt]      DECIMAL (18, 2) NULL,
    [expt_amt]     DECIMAL (18, 2) NULL,
    [ngsup_amt]    DECIMAL (18, 2) NULL,
    [sply_ty]      VARCHAR (50)    NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createddate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [createdby]    INT             NULL,
    PRIMARY KEY CLUSTERED ([nilid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_EXT_GSTR1_NIL_74038786187FF3432680F4DEACA7B2FF]
    ON [dbo].[TBL_EXT_GSTR1_NIL]([referenceno] ASC, [rowstatus] ASC, [sourcetype] ASC)
    INCLUDE([cur_gt], [errormessage], [expt_amt], [fp], [gstin], [gt], [ngsup_amt], [nil_amt], [sply_ty]);

