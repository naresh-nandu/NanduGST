CREATE TABLE [dbo].[TBL_GSTR2_D_NIL_ITMS] (
    [nilitmid]  INT             IDENTITY (1, 1) NOT NULL,
    [nilid]     INT             NOT NULL,
    [chksum]    VARCHAR (50)    NULL,
    [ty]        VARCHAR (10)    NULL,
    [cpdddr]    DECIMAL (18, 2) NULL,
    [exptdsply] DECIMAL (18, 2) NULL,
    [ngsply]    DECIMAL (18, 2) NULL,
    [nilsply]   DECIMAL (18, 2) NULL,
    [gstr2did]  INT             NULL,
    [gstinid]   INT             NULL,
    CONSTRAINT [PK_TBL_GSTR2_D_NIL_NIL] PRIMARY KEY CLUSTERED ([nilitmid] ASC),
    CONSTRAINT [FK_TBL_GSTR2_D_NIL_ITMS_TBL_GSTR2_D_NIL] FOREIGN KEY ([nilid]) REFERENCES [dbo].[TBL_GSTR2_D_NIL] ([nilid])
);

