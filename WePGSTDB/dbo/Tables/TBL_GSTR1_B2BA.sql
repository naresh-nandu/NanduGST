CREATE TABLE [dbo].[TBL_GSTR1_B2BA] (
    [b2baid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2BA] PRIMARY KEY CLUSTERED ([b2baid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2BA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

