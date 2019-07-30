CREATE TABLE [dbo].[TBL_GSTR2_D_HSNSUM_DET] (
    [hsnsumdetid] INT             IDENTITY (1, 1) NOT NULL,
    [hsnsumid]    INT             NOT NULL,
    [num]         INT             NULL,
    [hsn_sc]      VARCHAR (50)    NULL,
    [uqc]         VARCHAR (50)    NULL,
    [descs]       VARCHAR (50)    NULL,
    [qty]         INT             NULL,
    [val]         DECIMAL (18, 2) NULL,
    [txval]       DECIMAL (18, 2) NULL,
    [iamt]        DECIMAL (18, 2) NULL,
    [camt]        DECIMAL (18, 2) NULL,
    [samt]        DECIMAL (18, 2) NULL,
    [csamt]       DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_HSNSUM_DET] PRIMARY KEY CLUSTERED ([hsnsumdetid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_HSNSUM_DET_TBL_GSTR2_D_HSNSUM] FOREIGN KEY ([hsnsumid]) REFERENCES [dbo].[TBL_GSTR2_D_HSNSUM] ([hsnsumid])
);

