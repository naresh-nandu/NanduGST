CREATE TABLE [dbo].[TBL_Supplier] (
    [SupplierId]             INT            IDENTITY (1, 1) NOT NULL,
    [SupplierName]           NVARCHAR (255) NOT NULL,
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
    CONSTRAINT [PK_TBL_Supplier] PRIMARY KEY CLUSTERED ([SupplierId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_Supplier_1492FCAF10196DCC12FE98F4302ED3F3]
    ON [dbo].[TBL_Supplier]([RowStatus] ASC, [SupplierName] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_Supplier_EFA017D00D4634C8AD2007B5207C99AA]
    ON [dbo].[TBL_Supplier]([CustomerId] ASC, [RowStatus] ASC, [GSTINno] ASC)
    INCLUDE([Address], [ConstitutionOfBusiness], [CreatedBy], [CreatedDate], [DateofCompRegistered], [EmailId], [LastmodifiedBy], [LastModifiedDate], [MobileNo], [NatureOfBusiness], [PANNO], [POCName], [StateCode], [SupplierName]);

