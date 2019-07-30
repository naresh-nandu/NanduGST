CREATE TABLE [dbo].[TBL_RECVD_FILE_RECORDS] (
    [slno]           INT            IDENTITY (1, 1) NOT NULL,
    [fileid]         INT            NULL,
    [recordid]       INT            NULL,
    [recordcontents] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TBL_RECVD_FILE_RECORDS] PRIMARY KEY CLUSTERED ([slno] ASC),
    CONSTRAINT [FK_TBL_RECVD_FILE_RECORDS_TBL_RECVD_FILES] FOREIGN KEY ([fileid]) REFERENCES [dbo].[TBL_RECVD_FILES] ([fileid])
);

