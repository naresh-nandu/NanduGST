CREATE TABLE [dbo].[TBL_GSTR2_HSNSUM_DET] (
    [detid]    INT             IDENTITY (1, 1) NOT NULL,
    [hsnsumid] INT             NOT NULL,
    [num]      INT             NULL,
    [hsn_sc]   VARCHAR (10)    NULL,
    [uqc]      VARCHAR (50)    NULL,
    [qty]      DECIMAL (18, 2) NULL,
    [val]      DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [desc]     VARCHAR (50)    NULL,
    [gstinid]  INT             NULL,
    [gstr2id]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_HSNSUM_DET] PRIMARY KEY CLUSTERED ([detid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_HSNSUM_DET_TBL_GSTR2_HSNSUM] FOREIGN KEY ([hsnsumid]) REFERENCES [dbo].[TBL_GSTR2_HSNSUM] ([hsnsumid])
);

