CREATE TABLE [dbo].[TBL_Cust_PAN] (
    [PANId]        INT           IDENTITY (1, 1) NOT NULL,
    [PANNo]        NVARCHAR (10) NULL,
    [CustId]       INT           NULL,
    [CreatedBy]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    [rowstatus]    BIT           NULL,
    [CompanyName]  VARCHAR (50)  NULL,
    CONSTRAINT [PK_TBL_Cust_PAN] PRIMARY KEY CLUSTERED ([PANId] ASC)
);

