CREATE TABLE [dbo].[TBL_GSTR6] (
    [gstr6id] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]   VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    [fp]      VARCHAR (10) NULL,
    CONSTRAINT [PK_TBL_GSTR6] PRIMARY KEY CLUSTERED ([gstr6id] ASC)
);

