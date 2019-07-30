CREATE TABLE [dbo].[TBL_RECVD_FILE_ERRORS] (
    [slno]         INT           IDENTITY (1, 1) NOT NULL,
    [fileid]       INT           NULL,
    [recordid]     INT           NULL,
    [errorcode]    SMALLINT      NULL,
    [errormessage] VARCHAR (255) NULL,
    CONSTRAINT [PK_TBL_RECVD_FILE_ERRORS] PRIMARY KEY CLUSTERED ([slno] ASC)
);

