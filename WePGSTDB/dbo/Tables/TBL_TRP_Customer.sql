﻿CREATE TABLE [dbo].[TBL_TRP_Customer] (
    [trpCustId]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (50)  NOT NULL,
    [Designation]  VARCHAR (50)  NULL,
    [Company]      VARCHAR (50)  NOT NULL,
    [GSTINNo]      VARCHAR (50)  NOT NULL,
    [Email]        VARCHAR (50)  NOT NULL,
    [MobileNo]     VARCHAR (50)  NOT NULL,
    [PANNo]        VARCHAR (50)  NOT NULL,
    [Statecode]    VARCHAR (50)  NOT NULL,
    [ValidFrom]    DATETIME      NULL,
    [ValidTo]      DATETIME      NULL,
    [StatusCode]   INT           NOT NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    [RowStatus]    BIT           NULL,
    [Approvedby]   INT           NULL,
    [ApprovedDate] DATETIME      NULL,
    [ReferenceNo]  VARCHAR (50)  NULL,
    [AadharNo]     VARCHAR (12)  NULL,
    [Address]      VARCHAR (255) NULL,
    [WalletPack]   BIT           NULL,
    CONSTRAINT [PK_TBL_TRPCustomer] PRIMARY KEY CLUSTERED ([trpCustId] ASC)
);

