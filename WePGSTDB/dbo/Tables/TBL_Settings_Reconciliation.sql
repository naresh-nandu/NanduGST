CREATE TABLE [dbo].[TBL_Settings_Reconciliation] (
    [settingId]        INT           IDENTITY (1, 1) NOT NULL,
    [CustId]           INT           NULL,
    [Gstinno]          NVARCHAR (15) NULL,
    [reconValueAdjust] INT           NULL,
    [rowstatus]        BIT           NULL,
    [createddate]      DATETIME      NULL,
    [createdby]        INT           NULL,
    [ModifiedDate]     DATETIME      NULL,
    [ModifiedBy]       INT           NULL
);

