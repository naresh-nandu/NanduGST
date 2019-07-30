CREATE TABLE [dbo].[TBL_Cust_Settings] (
    [Cust_SettingId]        INT      IDENTITY (1, 1) NOT NULL,
    [CustId]                INT      NULL,
    [InvoicePrintRequired]  BIT      CONSTRAINT [DF_TBL_Cust_Settings_InvoicePrintRequired] DEFAULT ((0)) NULL,
    [CreatedBy]             INT      NULL,
    [CreatedDate]           DATETIME NULL,
    [ModifiedBy]            INT      NULL,
    [ModifiedDate]          DATETIME NULL,
    [rowstatus]             BIT      CONSTRAINT [DF_TBL_Cust_Settings_rowstatus] DEFAULT ((1)) NULL,
    [GSTR3BAutoPopulate]    BIT      CONSTRAINT [DF_TBL_Cust_Settings_GSTR3BAutoPopulate] DEFAULT ((0)) NULL,
    [CtinValdnCustMgmtReqd] BIT      CONSTRAINT [DF_TBL_Cust_Settings_CtinValdnCustMgmtReqd] DEFAULT ((0)) NULL,
    [CdnValdnOrigInum]      BIT      CONSTRAINT [DF_TBL_Cust_Settings_CdnValdnOrigInum] DEFAULT ((0)) NULL,
    [ReconcilationSetting]  BIT      CONSTRAINT [DF_TBL_Cust_Settings_ReconcilationSetting] DEFAULT ((0)) NULL,
    [CtinValdnSupMgmtReqd]  BIT      CONSTRAINT [DF_TBL_Cust_Settings_CtinValdnSupMgmtReqd] DEFAULT ((0)) NULL,
    [HsnValdnHsnMstrReqd]   BIT      CONSTRAINT [DF_TBL_Cust_Settings_HsnValdnHsnMstrReqd] DEFAULT ((0)) NULL,
    [TaxValCalnReqd]        BIT      CONSTRAINT [DF_TBL_Cust_Settings_TaxValCalnReqd] DEFAULT ((0)) NULL,
    [Eway_To_GSTR1]         BIT      CONSTRAINT [DF_TBL_Cust_Settings_Eway_To_GSTR1] DEFAULT ((0)) NULL,
    [GSTR1_to_Eway]         BIT      CONSTRAINT [DF_TBL_Cust_Settings_GSTR1_to_Eway] DEFAULT ((0)) NULL,
    [EwayPrint]             BIT      CONSTRAINT [DF_TBL_Cust_Settings_EwayPrint] DEFAULT ((0)) NULL,
    [EwbEmailReqd]          BIT      CONSTRAINT [DF_TBL_Cust_Settings_EwbEmailReqd] DEFAULT ((0)) NULL,
    [LocationReqd]          BIT      CONSTRAINT [DF_TBL_Cust_Settings_LocationReqd] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_TBL_Cust_Settings] PRIMARY KEY CLUSTERED ([Cust_SettingId] ASC),
    CONSTRAINT [FK_TBL_Cust_Settings_TBL_Customer] FOREIGN KEY ([CustId]) REFERENCES [dbo].[TBL_Customer] ([CustId])
);



