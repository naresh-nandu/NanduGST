CREATE TABLE [dbo].[TBL_GSTR1_NIL] (
    [nilid]       INT          IDENTITY (1, 1) NOT NULL,
    [gstr1id]     INT          NOT NULL,
    [flag]        VARCHAR (1)  NULL,
    [chksum]      VARCHAR (75) NULL,
    [gstinid]     INT          NULL,
    [createddate] DATETIME     NULL,
    [createdby]   INT          NULL,
    [custid]      INT          NULL,
    CONSTRAINT [PK_TBL_GSTR1_NIL] PRIMARY KEY CLUSTERED ([nilid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_NIL_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

