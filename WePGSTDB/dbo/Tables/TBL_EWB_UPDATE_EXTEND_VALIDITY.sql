CREATE TABLE [dbo].[TBL_EWB_UPDATE_EXTEND_VALIDITY] (
    [ExtnId]            INT            IDENTITY (1, 1) NOT NULL,
    [UserGstin]         NVARCHAR (15)  NULL,
    [EwbNo]             NVARCHAR (12)  NULL,
    [VehicleNo]         NVARCHAR (30)  NULL,
    [FromPlace]         NVARCHAR (50)  NULL,
    [FromStateCode]     INT            NULL,
    [RemainingDistance] NVARCHAR (10)  NULL,
    [TransDocNo]        NVARCHAR (50)  NULL,
    [TransDocDate]      NVARCHAR (50)  NULL,
    [TransMode]         NVARCHAR (1)   NULL,
    [ExtnRsnCode]       NVARCHAR (50)  NULL,
    [ExtnRmrk]          NVARCHAR (50)  NULL,
    [Status]            BIT            NULL,
    [ErrorCode]         NVARCHAR (MAX) NULL,
    [ErrorDesc]         NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NULL,
    [CreatedDate]       NVARCHAR (30)  NULL,
    [CustId]            INT            NULL,
    CONSTRAINT [PK_TBL_EWB_EXTEND_VALIDITY] PRIMARY KEY CLUSTERED ([ExtnId] ASC)
);

