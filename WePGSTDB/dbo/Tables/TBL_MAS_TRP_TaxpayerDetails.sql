CREATE TABLE [dbo].[TBL_MAS_TRP_TaxpayerDetails] (
    [RegId]        INT            IDENTITY (1, 1) NOT NULL,
    [BusinessType] NVARCHAR (100) NULL,
    [Taxpayer]     NVARCHAR (50)  NULL,
    [State]        NVARCHAR (100) NULL,
    [District]     NVARCHAR (50)  NULL,
    [LegalName]    NVARCHAR (40)  NULL,
    [PanNo]        NVARCHAR (10)  NULL,
    [MobNumber]    NVARCHAR (50)  NULL,
    [Mail]         NVARCHAR (60)  NULL,
    [TRN]          NVARCHAR (60)  NULL,
    [ARN]          NVARCHAR (60)  NULL,
    [gstinnum]     NVARCHAR (60)  NULL,
    [TradeName]    NVARCHAR (50)  NULL,
    [custid]       INT            NULL,
    [createdby]    INT            NULL,
    [createdDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [RowStatus]    INT            NULL
);

