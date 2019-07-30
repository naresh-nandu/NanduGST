CREATE TABLE [dbo].[TBL_GSTR1_B2CS] (
    [b2csid]  INT             IDENTITY (1, 1) NOT NULL,
    [gstr1id] INT             NOT NULL,
    [flag]    VARCHAR (1)     NULL,
    [chksum]  VARCHAR (75)    NULL,
    [sply_ty] VARCHAR (5)     NULL,
    [txval]   DECIMAL (18, 2) NULL,
    [typ]     VARCHAR (2)     NULL,
    [etin]    VARCHAR (15)    NULL,
    [pos]     VARCHAR (2)     NULL,
    [rt]      DECIMAL (18, 2) NULL,
    [iamt]    DECIMAL (18, 2) NULL,
    [camt]    DECIMAL (18, 2) NULL,
    [samt]    DECIMAL (18, 2) NULL,
    [csamt]   DECIMAL (18, 2) NULL,
    [gstinid] INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CS] PRIMARY KEY CLUSTERED ([b2csid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CS_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR1_B2CS_F869F41EA0DFE66DA6FFDD1540BF0257]
    ON [dbo].[TBL_GSTR1_B2CS]([gstr1id] ASC, [pos] ASC, [rt] ASC, [sply_ty] ASC)
    INCLUDE([camt], [csamt], [etin], [flag], [iamt], [samt], [txval], [typ]);

