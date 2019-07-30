CREATE TABLE [dbo].[TBL_RECVD_FILES] (
    [fileid]                INT           IDENTITY (1, 1) NOT NULL,
    [filename]              VARCHAR (255) NULL,
    [filestatus]            TINYINT       NULL,
    [gstrtypeid]            TINYINT       NULL,
    [createdby]             VARCHAR (255) NULL,
    [createddate]           DATETIME      NULL,
    [totalrecordscount]     INT           NULL,
    [processedrecordscount] INT           NULL,
    [errorrecordscount]     INT           NULL,
    CONSTRAINT [PK_TBL_RECVD_FILES] PRIMARY KEY CLUSTERED ([fileid] ASC)
);

