CREATE TABLE [dbo].[TBL_EWB_UPDATE_VEHICLENO] (
    [ewbVehUpdid]          INT            IDENTITY (1, 1) NOT NULL,
    [userGSTIN]            NVARCHAR (15)  NULL,
    [EwbNo]                NVARCHAR (15)  NULL,
    [vehicleNo]            NVARCHAR (20)  NULL,
    [FromPlace]            NVARCHAR (50)  NULL,
    [FromState]            NVARCHAR (2)   NULL,
    [ReasonCode]           NVARCHAR (1)   NULL,
    [ReasonRem]            NVARCHAR (MAX) NULL,
    [TransMode]            NVARCHAR (1)   NULL,
    [TransDocNo]           NVARCHAR (15)  NULL,
    [TransDocDate]         NVARCHAR (10)  NULL,
    [CustId]               INT            NULL,
    [createdby]            INT            NULL,
    [createddate]          DATETIME       NULL,
    [UPD_status]           NVARCHAR (1)   NULL,
    [vehUpdDate]           NVARCHAR (50)  NULL,
    [validUpto]            NVARCHAR (50)  NULL,
    [UPD_errorCodes]       NVARCHAR (MAX) NULL,
    [UPD_errorDescription] NVARCHAR (MAX) NULL,
    [updMode]              NVARCHAR (50)  NULL,
    [tripshtNo]            NVARCHAR (50)  NULL,
    [userGSTINTransin]     NVARCHAR (50)  NULL,
    [enteredDate]          NVARCHAR (50)  NULL,
    [ewayBillNo]           NVARCHAR (50)  NULL,
    [ewayBillDate]         NVARCHAR (50)  NULL
);



