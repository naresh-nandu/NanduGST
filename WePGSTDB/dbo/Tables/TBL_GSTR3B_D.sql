CREATE TABLE [dbo].[TBL_GSTR3B_D] (
    [gstr3bdid]  INT          IDENTITY (1, 1) NOT NULL,
    [gstin]      VARCHAR (15) NOT NULL,
    [ret_period] VARCHAR (10) NULL,
    [gstinid]    INT          NULL,
    PRIMARY KEY CLUSTERED ([gstr3bdid] ASC)
);

