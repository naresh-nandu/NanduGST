CREATE TABLE [dbo].[TBL_GSTR6_ISDA] (
    [isdaid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstr6id] INT          NOT NULL,
    [ctin]    VARCHAR (15) NULL,
    [gstinid] INT          NULL,
    CONSTRAINT [PK_TBL_GSTR6_ISDA] PRIMARY KEY CLUSTERED ([isdaid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_ISDA_TBL_GSTR6] FOREIGN KEY ([gstr6id]) REFERENCES [dbo].[TBL_GSTR6] ([gstr6id])
);

