CREATE TABLE [dbo].[TBL_GSTR6_ISDA_INELGLST] (
    [inelgid]      INT          IDENTITY (1, 1) NOT NULL,
    [isdaid]       INT          NOT NULL,
    [typ]          VARCHAR (64) NULL,
    [cpty]         VARCHAR (15) NULL,
    [rcpty]        VARCHAR (15) NULL,
    [statecd]      VARCHAR (2)  NULL,
    [rstatecd]     VARCHAR (2)  NULL,
    [gstinid]      INT          NULL,
    [gstr6id]      INT          NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    [CompCode]     VARCHAR (50) NULL,
    [UnitCode]     VARCHAR (50) NULL,
    [ReceivedBy]   VARCHAR (50) NULL,
    [ReceivedDate] VARCHAR (10) NULL,
    CONSTRAINT [PK_TBL_GSTR6_ISDA_INELGLST] PRIMARY KEY CLUSTERED ([inelgid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_ISDA_INELGLST_TBL_GSTR6_ISDA] FOREIGN KEY ([isdaid]) REFERENCES [dbo].[TBL_GSTR6_ISDA] ([isdaid])
);

