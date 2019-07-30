CREATE TABLE [dbo].[TBL_TRP_UserAccess_Customer] (
    [UserAccessId]     INT            IDENTITY (1, 1) NOT NULL,
    [trpUserId]        INT            NULL,
    [trpId]            INT            NULL,
    [CustId]           INT            NULL,
    [custRefno]        NVARCHAR (150) NOT NULL,
    [Password]         VARCHAR (50)   NOT NULL,
    [Email]            NVARCHAR (250) NOT NULL,
    [ValidFrom]        DATETIME       NULL,
    [ValidTo]          DATETIME       NULL,
    [CreatedBy]        INT            NULL,
    [CreatedDate]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedDate] DATETIME       NULL,
    [rowstatus]        BIT            NULL
);

