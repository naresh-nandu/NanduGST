CREATE TABLE [dbo].[TBL_GSTR1_B2CL] (
    [b2clid]  INT         IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT         NOT NULL,
    [pos]     VARCHAR (2) NULL,
    [gstinid] INT         NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CL] PRIMARY KEY CLUSTERED ([b2clid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CL_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

