CREATE TABLE [dbo].[TBL_STATE_MASTER] (
    [StateId]       INT          IDENTITY (1, 1) NOT NULL,
    [StateName]     VARCHAR (50) NULL,
    [StateType]     VARCHAR (10) NULL,
    [StateCode]     VARCHAR (2)  NULL,
    [VehicleSeries] VARCHAR (2)  NULL,
    [StateCapital]  VARCHAR (50) NULL,
    CONSTRAINT [PK_TBL_STATE_MASTER] PRIMARY KEY CLUSTERED ([StateId] ASC)
);

