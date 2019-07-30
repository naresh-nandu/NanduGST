CREATE TABLE [dbo].[TBL_GSTR1_CDNRA] (
    [cdnraid] INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [cfs]     VARCHAR (1)  NULL,
    [gstinid] INT          NULL,
    PRIMARY KEY CLUSTERED ([cdnraid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_CDNRA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

