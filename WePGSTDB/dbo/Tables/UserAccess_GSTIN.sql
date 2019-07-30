CREATE TABLE [dbo].[UserAccess_GSTIN] (
    [UserAccessId]     INT      IDENTITY (1, 1) NOT NULL,
    [GSTINId]          INT      NOT NULL,
    [CustId]           INT      NOT NULL,
    [UserId]           INT      NOT NULL,
    [CreatedDate]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedBy]   INT      NULL,
    [LastModifiedDate] DATETIME NULL,
    [Rowstatus]        BIT      CONSTRAINT [DF_UserAccess_GSTIN_Rowstatus] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_UserAccess_GSTIN] PRIMARY KEY CLUSTERED ([UserAccessId] ASC),
    CONSTRAINT [FK_UserAccess_GSTIN_TBL_Cust_GSTIN] FOREIGN KEY ([GSTINId]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId]),
    CONSTRAINT [FK_UserAccess_GSTIN_TBL_Customer] FOREIGN KEY ([CustId]) REFERENCES [dbo].[TBL_Customer] ([CustId]),
    CONSTRAINT [FK_UserAccess_GSTIN_UserList] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserList] ([UserId])
);

