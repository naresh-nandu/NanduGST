CREATE TABLE [dbo].[TBL_EXT_GSTR2_NIL] (
    [nilid]        INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [niltype]      VARCHAR (50)    NULL,
    [cpddr]        DECIMAL (18, 2) NULL,
    [exptdsply]    DECIMAL (18, 2) NULL,
    [ngsply]       DECIMAL (18, 2) NULL,
    [nilsply]      DECIMAL (18, 2) NULL,
    [rowstatus]    TINYINT         NULL,
    [sourcetype]   VARCHAR (15)    NULL,
    [referenceno]  VARCHAR (50)    NULL,
    [createddate]  DATETIME        NULL,
    [errormessage] VARCHAR (255)   NULL,
    [fileid]       INT             NULL,
    [CompCode]     VARCHAR (50)    NULL,
    [UnitCode]     VARCHAR (50)    NULL,
    [ReceivedBy]   VARCHAR (50)    NULL,
    [ReceivedDate] VARCHAR (10)    NULL,
    PRIMARY KEY CLUSTERED ([nilid] ASC)
);



