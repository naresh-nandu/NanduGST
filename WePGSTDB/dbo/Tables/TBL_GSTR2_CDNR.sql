CREATE TABLE [dbo].[TBL_GSTR2_CDNR] (
    [cdnrid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr2id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR2_CDNR] PRIMARY KEY CLUSTERED ([cdnrid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_CDNR_TBL_GSTR2] FOREIGN KEY ([gstr2id]) REFERENCES [dbo].[TBL_GSTR2] ([gstr2id])
);

