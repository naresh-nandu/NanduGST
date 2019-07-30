CREATE TABLE [dbo].[TBL_GSTR2A_B2B] (
    [b2bid]    INT          IDENTITY (1, 1) NOT NULL,
    [gstr2aid] INT          NOT NULL,
    [ctin]     VARCHAR (15) NULL,
    [cfs]      VARCHAR (1)  NULL,
    [gstinid]  INT          NULL,
    PRIMARY KEY CLUSTERED ([b2bid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR2A_B2B_578282816EEBC488995BDE5AF077C7EF]
    ON [dbo].[TBL_GSTR2A_B2B]([gstr2aid] ASC, [ctin] ASC);

