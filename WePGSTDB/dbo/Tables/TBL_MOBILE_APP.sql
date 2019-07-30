CREATE TABLE [dbo].[TBL_MOBILE_APP] (
    [MobileAppId] INT          IDENTITY (1, 1) NOT NULL,
    [GSTIN]       VARCHAR (15) NULL,
    [ReferenceNo] VARCHAR (50) NULL,
    [DeviceType]  VARCHAR (50) NULL,
    [CreatedDate] DATETIME     NULL
);

