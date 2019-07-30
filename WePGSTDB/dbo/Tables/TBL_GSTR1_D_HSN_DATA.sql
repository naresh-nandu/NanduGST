CREATE TABLE [dbo].[TBL_GSTR1_D_HSN_DATA] (
    [dataid]   INT             IDENTITY (1, 1) NOT NULL,
    [hsnid]    INT             NOT NULL,
    [num]      INT             NULL,
    [hsn_sc]   VARCHAR (10)    NULL,
    [descs]    VARCHAR (50)    NULL,
    [uqc]      VARCHAR (50)    NULL,
    [qty]      DECIMAL (18, 2) NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_HSN_DATA] PRIMARY KEY CLUSTERED ([dataid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_HSN_DATA_TBL_GSTR1_D_HSN] FOREIGN KEY ([hsnid]) REFERENCES [dbo].[TBL_GSTR1_D_HSN] ([hsnid])
);

