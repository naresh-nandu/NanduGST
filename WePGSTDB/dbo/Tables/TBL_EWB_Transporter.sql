CREATE TABLE [dbo].[TBL_EWB_Transporter] (
    [Transid]       INT          IDENTITY (1, 1) NOT NULL,
    [userGstin]     VARCHAR (15) NULL,
    [ewbNo]         VARCHAR (50) NULL,
    [ewbDate]       VARCHAR (50) NULL,
    [genGstin]      VARCHAR (50) NULL,
    [docNo]         VARCHAR (50) NULL,
    [docDate]       VARCHAR (30) NULL,
    [delPinCode]    INT          NULL,
    [delStateCode]  INT          NULL,
    [delPlace]      VARCHAR (50) NULL,
    [validUpto]     VARCHAR (50) NULL,
    [extendedTimes] VARCHAR (50) NULL,
    [status]        VARCHAR (5)  NULL,
    [rejectStatus]  VARCHAR (50) NULL,
    [flag]          VARCHAR (1)  NULL,
    [custid]        INT          NULL,
    [createdby]     INT          NULL,
    [createddate]   DATETIME     NULL,
    [ModifiedBy]    INT          NULL,
    [ModifiedDate]  DATETIME     NULL,
    CONSTRAINT [PK_TBL_EWB_Transporter] PRIMARY KEY CLUSTERED ([Transid] ASC)
);



