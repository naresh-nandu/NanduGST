CREATE TABLE [dbo].[TBL_TRP_Userlist] (
    [trpUserId]        INT             IDENTITY (1, 1) NOT NULL,
    [trpCustId]        INT             NOT NULL,
    [Name]             NVARCHAR (150)  NOT NULL,
    [Designation]      VARCHAR (50)    NULL,
    [Username]         VARCHAR (50)    NULL,
    [Password]         VARCHAR (50)    NOT NULL,
    [Email]            NVARCHAR (250)  NOT NULL,
    [MobileNo]         VARCHAR (50)    NOT NULL,
    [RoleId]           INT             NOT NULL,
    [ValidFrom]        DATETIME        NULL,
    [ValidTo]          DATETIME        NULL,
    [Status]           BIT             NOT NULL,
    [CreatedBy]        INT             NULL,
    [CreatedDate]      DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedDate] DATETIME        NULL,
    [EsignedUser]      NVARCHAR (MAX)  NULL,
    [HashPassword]     VARBINARY (MAX) NULL,
    [rowstatus]        BIT             NULL,
    CONSTRAINT [PK_TBL_TRPUserlist] PRIMARY KEY CLUSTERED ([trpUserId] ASC)
);

