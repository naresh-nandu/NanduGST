CREATE VIEW [dbo].[View_TBL_METRO_TRADE_OUTWARD]
AS
SELECT BillToCustomerID, ShipToCustomerID, TXNType, DocumentType, EntityID, PlaceOfSupplyStateID, DocumentNo, DocumentDate, OriginalInvoiceNo, OriginalInvoiceDate,
		TransportMode, TransporterName, TransporterCode, VehicleNo, TransportDocRefNo, EwayBillNo, EwayBillDate, Freight, Insurance, PackingForwardingCharges,
		ValueOfSuppliesReturned, TotalDocumentValue, EcommOperatorGSTIN, Terms_Conditions, ShippingBillNo, ShippingBillDate, ItemID_ServiceID, NoOfUnits, UnitOfMeasure,
		PricePerUnit, Price, OctroiRate, OctroiAmount, LocalLevyRate, LocalLevyAmount, Discount, OtherLineCharges3, OtherLineCharges4, OtherLineDeduction3, OtherLineDeduction4,
		OtherLineItemTax3, OtherLineItemTax4, TaxableValue, CGSTRate, CGSTAmount, SGSTRate, SGSTAmount, IGSTRate, IGSTAmount, TotalLineItemValue, UTGSTRate, UTGSTAmount,
		GSTCessRate, GSTCessAmount FROM [TBL_METRO_TRADE_OUTWARD]