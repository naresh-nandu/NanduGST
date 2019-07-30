CREATE TABLE [dbo].[TBL_EXT_GSTR3B_DET] (
    [detid]              INT             IDENTITY (1, 1) NOT NULL,
    [gstin]              VARCHAR (15)    NULL,
    [fp]                 VARCHAR (10)    NULL,
    [natureofsupplies]   VARCHAR (100)   NULL,
    [supply_num]         INT             NULL,
    [pos]                VARCHAR (2)     NULL,
    [txval]              DECIMAL (18, 2) NULL,
    [iamt]               DECIMAL (18, 2) NULL,
    [camt]               DECIMAL (18, 2) NULL,
    [samt]               DECIMAL (18, 2) NULL,
    [csamt]              DECIMAL (18, 2) NULL,
    [interstatesupplies] DECIMAL (18, 2) NULL,
    [intrastatesupplies] DECIMAL (18, 2) NULL,
    [rowstatus]          TINYINT         NULL,
    [sourcetype]         VARCHAR (15)    NULL,
    [referenceno]        VARCHAR (50)    NULL,
    [createddate]        DATETIME        NULL,
    [errormessage]       VARCHAR (255)   NULL,
    [fileid]             INT             NULL,
    [createdby]          INT             NULL,
    PRIMARY KEY CLUSTERED ([detid] ASC)
);

