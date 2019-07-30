CREATE TABLE [dbo].[TBL_GSTR1_B2CLA] (
    [b2claid] INT         IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT         NOT NULL,
    [pos]     VARCHAR (2) NULL,
    [gstinid] INT         NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CLA] PRIMARY KEY CLUSTERED ([b2claid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CLA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

