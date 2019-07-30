CREATE TABLE [dbo].[TBL_GSTR2] (
    [gstr2id] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]   VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    [fp]      VARCHAR (10) NULL,
    CONSTRAINT [PK_TBL_GSTR2] PRIMARY KEY CLUSTERED ([gstr2id] ASC)
);

