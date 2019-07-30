CREATE TABLE [dbo].[TBL_EWB_UPDATE_TRANSPORTER] (
    [TansId]       INT            IDENTITY (1, 1) NOT NULL,
    [UserGstin]    NVARCHAR (15)  NULL,
    [EwbNo]        NVARCHAR (12)  NULL,
    [TransId]      NVARCHAR (15)  NULL,
    [UpdTransDate] NVARCHAR (30)  NULL,
    [Status]       BIT            NULL,
    [ErrorCode]    NVARCHAR (MAX) NULL,
    [ErrorDesc]    NVARCHAR (MAX) NULL,
    [CreatedBy]    INT            NULL,
    [CreatedDate]  NVARCHAR (30)  NULL,
    [CustId]       INT            NULL,
    CONSTRAINT [PK_TBL_EWB_UPDATE_TRANSPORTER] PRIMARY KEY CLUSTERED ([TansId] ASC)
);

