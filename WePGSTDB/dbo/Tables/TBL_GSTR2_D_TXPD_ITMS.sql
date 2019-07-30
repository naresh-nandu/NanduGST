CREATE TABLE [dbo].[TBL_GSTR2_D_TXPD_ITMS] (
    [txpdItmsid] INT             IDENTITY (1, 1) NOT NULL,
    [txpdid]     INT             NOT NULL,
    [num]        INT             NULL,
    [rt]         DECIMAL (18, 2) NULL,
    [adamt]      DECIMAL (18, 2) NULL,
    [iamt]       DECIMAL (18, 2) NULL,
    [camt]       DECIMAL (18, 2) NULL,
    [samt]       DECIMAL (18, 2) NULL,
    [csamt]      DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_TXPD_ITMS] PRIMARY KEY CLUSTERED ([txpdItmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_TXPD_ITMS_TBL_GSTR2_D_TXPD] FOREIGN KEY ([txpdid]) REFERENCES [dbo].[TBL_GSTR2_D_TXPD] ([txpdid])
);

