CREATE TABLE [dbo].[TBL_EXT_GSTR2_ITCRVSL] (
    [itcrvslid]    INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [flag]         VARCHAR (1)     NULL,
    [chksum]       VARCHAR (64)    NULL,
    [ruletype]     VARCHAR (50)    NULL,
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
    [CompCode]     VARCHAR (50)    NULL,
    [UnitCode]     VARCHAR (50)    NULL,
    [ReceivedBy]   VARCHAR (50)    NULL,
    [ReceivedDate] VARCHAR (10)    NULL,
    PRIMARY KEY CLUSTERED ([itcrvslid] ASC)
);



