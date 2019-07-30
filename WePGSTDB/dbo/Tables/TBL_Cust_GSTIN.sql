CREATE TABLE [dbo].[TBL_Cust_GSTIN] (
    [GSTINId]       INT           IDENTITY (1, 1) NOT NULL,
    [GSTINNo]       NVARCHAR (15) NULL,
    [Statecode]     NVARCHAR (2)  NULL,
    [PANNo]         NVARCHAR (10) NULL,
    [CustId]        INT           NULL,
    [CreatedBy]     INT           NULL,
    [CreatedDate]   DATETIME      NULL,
    [ModifiedBy]    INT           NULL,
    [ModifiedDate]  DATETIME      NULL,
    [rowstatus]     BIT           CONSTRAINT [DF_TBL_Cust_GSTIN_rowstatus] DEFAULT ((1)) NULL,
    [GSTINUserName] NVARCHAR (50) NULL,
    [Address]       VARCHAR (255) NULL,
    [EWBUserId]     NVARCHAR (50) NULL,
    [EWBUserName]   NVARCHAR (50) NULL,
    [EWBPassword]   NVARCHAR (50) NULL,
    CONSTRAINT [PK_TBL_Cust_GSTIN] PRIMARY KEY CLUSTERED ([GSTINId] ASC),
    CONSTRAINT [FK_TBL_Cust_GSTIN_TBL_Customer] FOREIGN KEY ([CustId]) REFERENCES [dbo].[TBL_Customer] ([CustId])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_Cust_GSTIN_82FEC21FA85F5F60CE5FC08A33087F8D]
    ON [dbo].[TBL_Cust_GSTIN]([CustId] ASC, [rowstatus] ASC)
    INCLUDE([GSTINNo]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_Cust_GSTIN_AB86A947F2A6AD2304EE9C1DADC19CFA]
    ON [dbo].[TBL_Cust_GSTIN]([GSTINNo] ASC, [rowstatus] ASC, [CustId] ASC);

