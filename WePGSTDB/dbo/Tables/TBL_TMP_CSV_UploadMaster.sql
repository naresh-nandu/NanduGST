﻿CREATE TABLE [dbo].[TBL_TMP_CSV_UploadMaster] (
    [UploadId]      INT            IDENTITY (1, 1) NOT NULL,
    [Uploadcontent] NVARCHAR (MAX) NOT NULL,
    [MasterType]    VARCHAR (50)   NOT NULL,
    [FileName]      VARCHAR (MAX)  NOT NULL,
    [CustomerId]    INT            NOT NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [rowstatus]     BIT            NULL
);

