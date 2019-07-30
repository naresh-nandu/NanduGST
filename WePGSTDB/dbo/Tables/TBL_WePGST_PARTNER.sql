CREATE TABLE [dbo].[TBL_WePGST_PARTNER] (
    [PartnerId]    INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (250) NULL,
    [Company]      VARCHAR (250) NULL,
    [Email]        VARCHAR (250) NULL,
    [Mobile]       VARCHAR (10)  NULL,
    [Logo]         VARCHAR (250) NULL,
    [LogoPath]     VARCHAR (250) NULL,
    [DomainName]   VARCHAR (250) NULL,
    [IPAddress]    VARCHAR (50)  NULL,
    [LogoutURL]    VARCHAR (250) NULL,
    [RowStatus]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedDate] DATETIME      NULL,
    [referenceno]  NVARCHAR (50) NULL
);

