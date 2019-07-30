CREATE TABLE [dbo].[TBL_GSTR6_CDNR] (
    [cdnrid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr6id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR6_CDNR] PRIMARY KEY CLUSTERED ([cdnrid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_CDNR_TBL_GSTR6] FOREIGN KEY ([gstr6id]) REFERENCES [dbo].[TBL_GSTR6] ([gstr6id])
);

