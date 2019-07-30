CREATE TABLE [dbo].[TBL_CSV_TRP_Customer_RECS] (
    [fileid]        INT            NULL,
    [slno]          VARCHAR (50)   NULL,
    [Name]          VARCHAR (50)   NULL,
    [Designation]   VARCHAR (50)   NULL,
    [Company]       NVARCHAR (250) NULL,
    [GSTIN]         VARCHAR (50)   NULL,
    [GSTINUserName] VARCHAR (50)   NULL,
    [Email]         VARCHAR (50)   NULL,
    [MobileNo]      VARCHAR (50)   NULL,
    [pan]           VARCHAR (50)   NULL,
    [Statecode]     VARCHAR (50)   NULL,
    [AadharNo]      VARCHAR (50)   NULL,
    [Address]       NVARCHAR (MAX) NULL,
    [errorcode]     SMALLINT       NULL,
    [errormessage]  VARCHAR (255)  NULL,
    [Package]       VARCHAR (50)   NULL
);

