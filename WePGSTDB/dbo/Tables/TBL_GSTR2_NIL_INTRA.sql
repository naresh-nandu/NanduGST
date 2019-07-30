CREATE TABLE [dbo].[TBL_GSTR2_NIL_INTRA] (
    [intraid]   INT             IDENTITY (1, 1) NOT NULL,
    [nilid]     INT             NOT NULL,
    [cpddr]     DECIMAL (18, 2) NULL,
    [exptdsply] DECIMAL (18, 2) NULL,
    [ngsply]    DECIMAL (18, 2) NULL,
    [nilsply]   DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr2id]   INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_NIL_INTRA] PRIMARY KEY CLUSTERED ([intraid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_NIL_INTRA_TBL_GSTR2_NIL] FOREIGN KEY ([nilid]) REFERENCES [dbo].[TBL_GSTR2_NIL] ([nilid])
);

