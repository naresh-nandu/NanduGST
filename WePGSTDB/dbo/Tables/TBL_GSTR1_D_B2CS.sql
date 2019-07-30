CREATE TABLE [dbo].[TBL_GSTR1_D_B2CS] (
    [b2csid]   INT             IDENTITY (1, 1) NOT NULL,
    [gstr1did] INT             NOT NULL,
    [flag]     VARCHAR (20)    NULL,
    [chksum]   VARCHAR (64)    NULL,
    [sply_ty]  VARCHAR (5)     NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [etin]     VARCHAR (15)    NULL,
    [pos]      VARCHAR (2)     NULL,
    [typ]      VARCHAR (2)     NULL,
    [gstinid]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_B2CS] PRIMARY KEY CLUSTERED ([b2csid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_B2CS_TBL_GSTR1_D] FOREIGN KEY ([gstr1did]) REFERENCES [dbo].[TBL_GSTR1_D] ([gstr1did])
);

