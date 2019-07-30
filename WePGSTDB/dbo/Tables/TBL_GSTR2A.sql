CREATE TABLE [dbo].[TBL_GSTR2A] (
    [gstr2aid] INT          IDENTITY (1, 1) NOT NULL,
    [gstin]    VARCHAR (15) NULL,
    [gstinid]  INT          NULL,
    [fp]       VARCHAR (10) NULL,
    PRIMARY KEY CLUSTERED ([gstr2aid] ASC)
);

