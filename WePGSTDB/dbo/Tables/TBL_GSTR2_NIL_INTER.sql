CREATE TABLE [dbo].[TBL_GSTR2_NIL_INTER] (
    [interid]   INT             IDENTITY (1, 1) NOT NULL,
    [nilid]     INT             NOT NULL,
    [cpddr]     DECIMAL (18, 2) NULL,
    [exptdsply] DECIMAL (18, 2) NULL,
    [ngsply]    DECIMAL (18, 2) NULL,
    [nilsply]   DECIMAL (18, 2) NULL,
    [gstinid]   INT             NULL,
    [gstr2id]   INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_NIL_INTER] PRIMARY KEY CLUSTERED ([interid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_NIL_INTER_TBL_GSTR2_NIL] FOREIGN KEY ([nilid]) REFERENCES [dbo].[TBL_GSTR2_NIL] ([nilid])
);

