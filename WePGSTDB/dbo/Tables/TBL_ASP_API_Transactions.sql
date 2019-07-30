CREATE TABLE [dbo].[TBL_ASP_API_Transactions] (
    [transId]     INT            IDENTITY (1, 1) NOT NULL,
    [Custid]      INT            NOT NULL,
    [Gstinno]     VARCHAR (15)   NULL,
    [Period]      VARCHAR (10)   NULL,
    [ApiName]     NVARCHAR (250) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Createdby]   INT            NULL,
    [Createddate] DATETIME       NULL,
    [rowstatus]   BIT            NULL
);

