CREATE TABLE [dbo].[TBL_GSTR6A_B2B] (
    [b2bid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr6aid] INT          NOT NULL,
    [ctin]     VARCHAR (15) NULL,
    [cfs]      VARCHAR (1)  NULL,
    [gstinid]  INT          NULL,
    PRIMARY KEY CLUSTERED ([b2bid] ASC)
);

