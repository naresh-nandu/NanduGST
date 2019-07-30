CREATE TABLE [dbo].[TBL_GSTR1_D_B2CL] (
    [b2clid]   INT          IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT          NOT NULL,
    [pos]      VARCHAR (50) NULL,
    [gstinid]  INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_B2CL] PRIMARY KEY CLUSTERED ([b2clid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2CL_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

