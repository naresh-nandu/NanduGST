CREATE TABLE [dbo].[TBL_GSTR1] (
    [gstr1id] INT             IDENTITY (1, 1) NOT NULL,
    [gstin]   VARCHAR (15)    NULL,
    [gstinid] INT             NULL,
    [fp]      VARCHAR (10)    NULL,
    [gt]      DECIMAL (18, 2) NULL,
    [cur_gt]  DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_TBL_GSTR1] PRIMARY KEY CLUSTERED ([gstr1id] ASC),
    CONSTRAINT [FK_TBL_GSTR1_TBL_Cust_GSTIN] FOREIGN KEY ([gstinid]) REFERENCES [dbo].[TBL_Cust_GSTIN] ([GSTINId]) ON DELETE CASCADE ON UPDATE CASCADE
);

