CREATE TABLE [dbo].[TBL_EXT_EWB_UPDATE_VEHICLENO] (
    [ewbVehUpdid]   INT            IDENTITY (1, 1) NOT NULL,
    [userGSTIN]     NVARCHAR (15)  NULL,
    [ewbno]         NVARCHAR (15)  NULL,
    [vehicleno]     NVARCHAR (20)  NULL,
    [fromplace]     NVARCHAR (50)  NULL,
    [fromstate]     NVARCHAR (2)   NULL,
    [reasoncode]    NVARCHAR (1)   NULL,
    [reasonremarks] NVARCHAR (MAX) NULL,
    [transmode]     NVARCHAR (1)   NULL,
    [transdocno]    NVARCHAR (15)  NULL,
    [transdocdate]  NVARCHAR (10)  NULL,
    [rowstatus]     TINYINT        NULL,
    [sourcetype]    NVARCHAR (50)  NULL,
    [referenceno]   NVARCHAR (50)  NULL,
    [fileid]        INT            NULL,
    [errormessage]  VARCHAR (255)  NULL,
    [createdby]     INT            NULL,
    [createddate]   DATETIME       NULL
);

