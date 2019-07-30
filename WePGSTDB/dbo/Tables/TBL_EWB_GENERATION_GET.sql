﻿CREATE TABLE [dbo].[TBL_EWB_GENERATION_GET] (
    [ewbid]            INT             IDENTITY (1, 1) NOT NULL,
    [userGSTIN]        NVARCHAR (50)   NULL,
    [supplyType]       NVARCHAR (1)    NULL,
    [subSupplyType]    NVARCHAR (2)    NULL,
    [docType]          NVARCHAR (3)    NULL,
    [docNo]            NVARCHAR (50)   NULL,
    [docDate]          VARCHAR (30)    NULL,
    [fromGstin]        NVARCHAR (15)   NULL,
    [fromTrdName]      NVARCHAR (100)  NULL,
    [fromAddr1]        NVARCHAR (MAX)  NULL,
    [fromAddr2]        NVARCHAR (MAX)  NULL,
    [fromPlace]        NVARCHAR (50)   NULL,
    [fromPinCode]      INT             NULL,
    [fromStateCode]    INT             NULL,
    [toGstin]          NVARCHAR (15)   NULL,
    [toTrdName]        NVARCHAR (100)  NULL,
    [toAddr1]          NVARCHAR (MAX)  NULL,
    [toAddr2]          NVARCHAR (MAX)  NULL,
    [toPlace]          NVARCHAR (50)   NULL,
    [toPincode]        INT             NULL,
    [toStateCode]      INT             NULL,
    [totalValue]       DECIMAL (18, 2) NULL,
    [igstValue]        DECIMAL (18, 2) NULL,
    [cgstValue]        DECIMAL (18, 2) NULL,
    [sgstValue]        DECIMAL (18, 2) NULL,
    [cessValue]        DECIMAL (18, 2) NULL,
    [transMode]        INT             NULL,
    [transDistance]    NVARCHAR (50)   NULL,
    [transporterId]    NVARCHAR (15)   NULL,
    [transporterName]  NVARCHAR (100)  NULL,
    [transDocNo]       NVARCHAR (15)   NULL,
    [transDocDate]     VARCHAR (30)    NULL,
    [vehicleNo]        NVARCHAR (20)   NULL,
    [vehicleType]      NVARCHAR (10)   NULL,
    [status]           VARCHAR (5)     NULL,
    [errorCodes]       NVARCHAR (MAX)  NULL,
    [ewayBillNo]       NVARCHAR (50)   NULL,
    [ewayBillDate]     NVARCHAR (50)   NULL,
    [createdby]        INT             NULL,
    [createddate]      DATETIME        NULL,
    [CustId]           INT             NULL,
    [validUpto]        NVARCHAR (50)   NULL,
    [flag]             NVARCHAR (1)    NULL,
    [errorDescription] NVARCHAR (MAX)  NULL,
    [genMode]          NVARCHAR (50)   NULL,
    [noValidDays]      NVARCHAR (50)   NULL,
    [extendedTimes]    NVARCHAR (50)   NULL,
    [rejectedStatus]   NCHAR (10)      NULL
);

