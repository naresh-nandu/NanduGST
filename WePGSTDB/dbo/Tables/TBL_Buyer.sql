CREATE TABLE [dbo].[TBL_Buyer] (
    [BuyerId]                INT            IDENTITY (1, 1) NOT NULL,
    [BuyerName]              NVARCHAR (255) NOT NULL,
    [POCName]                NVARCHAR (255) NULL,
    [NatureOfBusiness]       NVARCHAR (255) NULL,
    [EmailId]                NVARCHAR (150) NULL,
    [MobileNo]               NVARCHAR (50)  NULL,
    [GSTINno]                NVARCHAR (100) NULL,
    [StateCode]              VARCHAR (5)    NULL,
    [PANNO]                  NVARCHAR (50)  NULL,
    [DateofCompRegistered]   VARCHAR (50)   NULL,
    [ConstitutionOfBusiness] NVARCHAR (250) NULL,
    [Address]                NVARCHAR (250) NULL,
    [CustomerId]             INT            NOT NULL,
    [CreatedBy]              INT            NOT NULL,
    [CreatedDate]            DATETIME       NOT NULL,
    [LastmodifiedBy]         INT            NULL,
    [LastModifiedDate]       DATETIME       NULL,
    [RowStatus]              BIT            NULL,
    CONSTRAINT [PK_TBL_Buyer] PRIMARY KEY CLUSTERED ([BuyerId] ASC)
);

