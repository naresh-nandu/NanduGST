﻿CREATE TABLE [dbo].[TBL_METRO_TRADE_OUTWARD] (
    [BillToCustomerID]         VARCHAR (250)  NULL,
    [ShipToCustomerID]         VARCHAR (250)  NULL,
    [TXNType]                  VARCHAR (250)  NULL,
    [DocumentType]             VARCHAR (250)  NULL,
    [EntityID]                 VARCHAR (250)  NULL,
    [PlaceOfSupplyStateID]     VARCHAR (250)  NULL,
    [DocumentNo]               VARCHAR (250)  NULL,
    [DocumentDate]             VARCHAR (10)   NULL,
    [OriginalInvoiceNo]        VARCHAR (250)  NULL,
    [OriginalInvoiceDate]      VARCHAR (10)   NULL,
    [TransportMode]            VARCHAR (250)  NULL,
    [TransporterName]          VARCHAR (250)  NULL,
    [TransporterCode]          VARCHAR (250)  NULL,
    [VehicleNo]                VARCHAR (250)  NULL,
    [TransportDocRefNo]        VARCHAR (250)  NULL,
    [EwayBillNo]               VARCHAR (250)  NULL,
    [EwayBillDate]             VARCHAR (10)   NULL,
    [Freight]                  VARCHAR (250)  NULL,
    [Insurance]                VARCHAR (250)  NULL,
    [PackingForwardingCharges] VARCHAR (250)  NULL,
    [ValueOfSuppliesReturned]  VARCHAR (250)  NULL,
    [TotalDocumentValue]       VARCHAR (250)  NULL,
    [EcommOperatorGSTIN]       VARCHAR (15)   NULL,
    [Terms_Conditions]         NVARCHAR (MAX) NULL,
    [ShippingBillNo]           VARCHAR (250)  NULL,
    [ShippingBillDate]         VARCHAR (10)   NULL,
    [ItemID_ServiceID]         VARCHAR (250)  NULL,
    [NoOfUnits]                VARCHAR (250)  NULL,
    [UnitOfMeasure]            VARCHAR (50)   NULL,
    [PricePerUnit]             VARCHAR (250)  NULL,
    [Price]                    VARCHAR (250)  NULL,
    [OctroiRate]               VARCHAR (250)  NULL,
    [OctroiAmount]             VARCHAR (250)  NULL,
    [LocalLevyRate]            VARCHAR (250)  NULL,
    [LocalLevyAmount]          VARCHAR (250)  NULL,
    [Discount]                 VARCHAR (250)  NULL,
    [OtherLineCharges3]        VARCHAR (250)  NULL,
    [OtherLineCharges4]        VARCHAR (250)  NULL,
    [OtherLineDeduction3]      VARCHAR (250)  NULL,
    [OtherLineDeduction4]      VARCHAR (250)  NULL,
    [OtherLineItemTax3]        VARCHAR (250)  NULL,
    [OtherLineItemTax4]        VARCHAR (250)  NULL,
    [TaxableValue]             VARCHAR (250)  NULL,
    [CGSTRate]                 VARCHAR (250)  NULL,
    [CGSTAmount]               VARCHAR (250)  NULL,
    [SGSTRate]                 VARCHAR (250)  NULL,
    [SGSTAmount]               VARCHAR (250)  NULL,
    [IGSTRate]                 VARCHAR (250)  NULL,
    [IGSTAmount]               VARCHAR (250)  NULL,
    [TotalLineItemValue]       VARCHAR (250)  NULL,
    [UTGSTRate]                VARCHAR (250)  NULL,
    [UTGSTAmount]              VARCHAR (250)  NULL,
    [GSTCessRate]              VARCHAR (250)  NULL,
    [GSTCessAmount]            VARCHAR (250)  NULL,
    [TradeOutwardId]           INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_TBL_METRO_TRADE_OUTWARD_mar] PRIMARY KEY CLUSTERED ([TradeOutwardId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_METRO_TRADE_OUTWARD_EBEB98161F1508B19622072FC1CDAC36]
    ON [dbo].[TBL_METRO_TRADE_OUTWARD]([DocumentNo] ASC);

