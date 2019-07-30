﻿CREATE TABLE [dbo].[TBL_CSV_EWB_GEN_CONS_RECS] (
    [fileid]           INT           NULL,
    [slno]             VARCHAR (50)  NULL,
    [userGSTIN]        VARCHAR (50)  NULL,
    [vehicleno]        VARCHAR (50)  NULL,
    [fromplace]        VARCHAR (50)  NULL,
    [transmode]        VARCHAR (50)  NULL,
    [transdocno]       VARCHAR (50)  NULL,
    [transdocdate]     VARCHAR (50)  NULL,
    [fromstate]        VARCHAR (50)  NULL,
    [ewbno]            VARCHAR (50)  NULL,
    [branch] VARCHAR(50) NULL, 
    [AllowDuplication] VARCHAR(50) NULL, 
    [errorcode] SMALLINT NULL, 
    [errormessage] VARCHAR(255) NULL
);



