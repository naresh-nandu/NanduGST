CREATE TABLE [dbo].[TBL_Metro_Summary_Output_Data] (
    [OutputId]     INT             IDENTITY (1, 1) NOT NULL,
    [gstin]        VARCHAR (15)    NULL,
    [fp]           VARCHAR (10)    NULL,
    [summarytype]  VARCHAR (25)    NULL,
    [documenttype] VARCHAR (25)    NULL,
    [invcnt]       INT             NULL,
    [txval]        DECIMAL (18, 2) NULL,
    [iamt]         DECIMAL (18, 2) NULL,
    [camt]         DECIMAL (18, 2) NULL,
    [samt]         DECIMAL (18, 2) NULL,
    [csamt]        DECIMAL (18, 2) NULL
);

