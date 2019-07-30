CREATE TABLE [dbo].[TBL_GSTR1_B2CS_N] (
    [b2csid]  INT         IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT         NOT NULL,
    [pos]     VARCHAR (2) NULL,
    [gstinid] INT         NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CS_N] PRIMARY KEY CLUSTERED ([b2csid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CS_N_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

