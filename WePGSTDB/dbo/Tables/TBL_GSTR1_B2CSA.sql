CREATE TABLE [dbo].[TBL_GSTR1_B2CSA] (
    [b2csaid]      INT            IDENTITY (1, 1) NOT NULL,
    [gstr1id]      INT            NOT NULL,
    [flag]         VARCHAR (1)    NULL,
    [chksum]       VARCHAR (75)   NULL,
    [omon]         VARCHAR (10)   NULL,
    [pos]          VARCHAR (2)    NULL,
    [sply_ty]      VARCHAR (5)    NULL,
    [typ]          VARCHAR (2)    NULL,
    [etin]         VARCHAR (15)   NULL,
    [diff_percent] DECIMAL (4, 2) NULL,
    [gstinid]      INT            NULL,
    [createddate]  DATETIME       NULL,
    [createdby]    INT            NULL,
    [custid]       INT            NULL,
    CONSTRAINT [PK_TBL_GSTR1_B2CSA] PRIMARY KEY CLUSTERED ([b2csaid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_B2CSA_TBL_GSTR1] FOREIGN KEY ([gstr1id]) REFERENCES [dbo].[TBL_GSTR1] ([gstr1id])
);

