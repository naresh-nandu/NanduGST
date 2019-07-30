CREATE TABLE [dbo].[TBL_ValidGSTIN] (
    [validGstinId]     INT            IDENTITY (1, 1) NOT NULL,
    [ValidGstin]       VARCHAR (20)   NULL,
    [Status]           INT            NULL,
    [Data]             NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NULL,
    [createdDate]      DATETIME       NULL,
    [CustId]           INT            NULL,
    [ModifiedBy]       INT            NULL,
    [ModifiedDate]     DATETIME       NULL,
    [LegalName]        NVARCHAR (MAX) NULL,
    [State_jur]        NVARCHAR (MAX) NULL,
    [Center_jur]       NVARCHAR (MAX) NULL,
    [RegDate]          NVARCHAR (MAX) NULL,
    [Business_Cont]    NVARCHAR (MAX) NULL,
    [taxpayerType]     NVARCHAR (MAX) NULL,
    [NatureOfBusiness] NVARCHAR (MAX) NULL,
    [CancellationDate] NVARCHAR (MAX) NULL,
    [LastUpdatedDate]  NVARCHAR (MAX) NULL,
    [State_Jur_Code]   NVARCHAR (MAX) NULL,
    [Center_Jur_Code]  NVARCHAR (MAX) NULL,
    [tradeName]        NVARCHAR (MAX) NULL,
    [strStatus]        NVARCHAR (MAX) NULL,
    [FileName]         NVARCHAR (MAX) NULL
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_ValidGSTIN_49EF5BE9BE397E8782CDFAF2D318115D]
    ON [dbo].[TBL_ValidGSTIN]([ValidGstin] ASC)
    INCLUDE([LegalName]);

