CREATE TABLE [dbo].[TBL_CSV_GSTR3B_RECS] (
    [fileid]             INT           NULL,
    [slno]               VARCHAR (50)  NULL,
    [gstin]              VARCHAR (50)  NULL,
    [fp]                 VARCHAR (50)  NULL,
    [natureofsupplies]   VARCHAR (100) NULL,
    [supply_num]         VARCHAR (50)  NULL,
    [pos]                VARCHAR (50)  NULL,
    [txval]              VARCHAR (50)  NULL,
    [iamt]               VARCHAR (50)  NULL,
    [camt]               VARCHAR (50)  NULL,
    [samt]               VARCHAR (50)  NULL,
    [csamt]              VARCHAR (50)  NULL,
    [interstatesupplies] VARCHAR (50)  NULL,
    [intrastatesupplies] VARCHAR (50)  NULL,
    [errorcode]          SMALLINT      NULL,
    [errormessage]       VARCHAR (255) NULL
);

