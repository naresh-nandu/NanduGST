CREATE TABLE [dbo].[TBL_VIEW_TRACK_STATUS] (
    [ViewTrackId] INT           IDENTITY (1, 1) NOT NULL,
    [gstin]       VARCHAR (15)  NULL,
    [period]      VARCHAR (50)  NULL,
    [status_res]  INT           NULL,
    [arn]         VARCHAR (50)  NULL,
    [ret_prd]     VARCHAR (50)  NULL,
    [mof]         VARCHAR (50)  NULL,
    [dof]         VARCHAR (50)  NULL,
    [rtntype]     VARCHAR (20)  NULL,
    [status]      VARCHAR (50)  NULL,
    [valid]       VARCHAR (50)  NULL,
    [Message]     VARCHAR (250) NULL,
    [UserId]      INT           NULL,
    [CustId]      INT           NULL,
    [CreatedDate] DATETIME      NULL
);

