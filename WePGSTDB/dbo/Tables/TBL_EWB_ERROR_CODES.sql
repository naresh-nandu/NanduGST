CREATE TABLE [dbo].[TBL_EWB_ERROR_CODES] (
    [EWBErrorId]       INT            IDENTITY (1, 1) NOT NULL,
    [ErrorCode]        NVARCHAR (50)  NULL,
    [ErrorDescription] NVARCHAR (MAX) NULL
);

