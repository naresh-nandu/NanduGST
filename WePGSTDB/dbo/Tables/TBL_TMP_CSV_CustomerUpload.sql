CREATE TABLE [dbo].[TBL_TMP_CSV_CustomerUpload] (
    [UploadId]      INT            IDENTITY (1, 1) NOT NULL,
    [Uploadcontent] NVARCHAR (MAX) NOT NULL,
    [FileName]      VARCHAR (MAX)  NOT NULL,
    [TrpId]         INT            NOT NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [rowstatus]     BIT            NULL
);

