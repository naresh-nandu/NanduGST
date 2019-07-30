CREATE TABLE [dbo].[TBL_Aggregator] (
    [TrpId]          INT          IDENTITY (1, 1) NOT NULL,
    [AggregatorName] VARCHAR (50) NULL,
    [AggregatorId]   VARCHAR (50) NULL,
    [Email]          VARCHAR (50) NULL,
    [MobileNo]       VARCHAR (50) NULL,
    [Password]       VARCHAR (50) NULL,
    [CustomerId]     INT          NULL,
    [CreatedBy]      INT          NULL,
    [CreatedDate]    DATETIME     NULL,
    [rowstatus]      BIT          CONSTRAINT [DF_TBL_Aggregator_rowstatus] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_TBL_Aggregator] PRIMARY KEY CLUSTERED ([TrpId] ASC)
);

