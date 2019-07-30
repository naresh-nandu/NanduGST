CREATE TABLE [dbo].[TBL_TMP_SUPPLIER] (
    [SupplierId]      INT            IDENTITY (1, 1) NOT NULL,
    [SupplierDetails] NVARCHAR (MAX) NOT NULL,
    [CustomerId]      INT            NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [rowstatus]       BIT            NULL
);

