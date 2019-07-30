CREATE TABLE [dbo].[TBL_EWB_GENERATION_VehiclDetails] (
    [vchid]            INT           IDENTITY (1, 1) NOT NULL,
    [ewbid]            INT           NULL,
    [ewbNo]            VARCHAR (12)  NULL,
    [updMode]          VARCHAR (50)  NULL,
    [vehicleNo]        VARCHAR (50)  NULL,
    [fromPlace]        VARCHAR (50)  NULL,
    [fromState]        INT           NULL,
    [tripshtNo]        NVARCHAR (50) NULL,
    [userGSTINTransin] VARCHAR (15)  NULL,
    [enteredDate]      VARCHAR (30)  NULL,
    [transMode]        INT           NULL,
    [transDocNo]       NVARCHAR (20) NULL,
    [transDocDate]     VARCHAR (30)  NULL,
    [vehicleType]      VARCHAR (50)  NULL,
    CONSTRAINT [PK_TBL_EWB_GENERATION_VehiclDetails] PRIMARY KEY CLUSTERED ([vchid] ASC)
);

