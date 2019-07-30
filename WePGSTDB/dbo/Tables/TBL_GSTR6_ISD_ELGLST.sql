CREATE TABLE [dbo].[TBL_GSTR6_ISD_ELGLST] (
    [elgid]        INT          IDENTITY (1, 1) NOT NULL,
    [isdid]        INT          NOT NULL,
    [typ]          VARCHAR (64) NULL,
    [cpty]         VARCHAR (15) NULL,
    [statecd]      VARCHAR (2)  NULL,
    [gstinid]      INT          NULL,
    [gstr6id]      INT          NULL,
    [createddate]  DATETIME     NULL,
    [createdby]    INT          NULL,
    [custid]       INT          NULL,
    [CompCode]     VARCHAR (50) NULL,
    [UnitCode]     VARCHAR (50) NULL,
    [ReceivedBy]   VARCHAR (50) NULL,
    [ReceivedDate] VARCHAR (10) NULL,
    CONSTRAINT [PK_TBL_GSTR6_ISD_ELGLST] PRIMARY KEY CLUSTERED ([elgid] ASC),
    CONSTRAINT [FK_TBL_GSTR6_ISD_ELGLST_TBL_GSTR6_ISD] FOREIGN KEY ([isdid]) REFERENCES [dbo].[TBL_GSTR6_ISD] ([isdid])
);

