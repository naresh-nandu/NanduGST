CREATE TABLE [dbo].[TBL_GSTR1_D_NIL_INV] (
    [invid]     INT             IDENTITY (1, 1) NOT NULL,
    [nilid]     INT             NOT NULL,
    [nil_amt]   DECIMAL (18, 2) NULL,
    [expt_amt]  DECIMAL (18, 2) NULL,
    [ngsup_amt] DECIMAL (18, 2) NULL,
    [sply_ty]   VARCHAR (50)    NULL,
    [gstinid]   INT             NULL,
    [gstr1did]  INT             NULL,
    CONSTRAINT [PK_TBL_GSTR1_D_NIL_INV] PRIMARY KEY CLUSTERED ([invid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_NIL_INV_TBL_GSTR1_D_NIL] FOREIGN KEY ([nilid]) REFERENCES [dbo].[TBL_GSTR1_D_NIL] ([nilid])
);

