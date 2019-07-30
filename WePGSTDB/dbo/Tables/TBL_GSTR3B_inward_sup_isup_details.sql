CREATE TABLE [dbo].[TBL_GSTR3B_inward_sup_isup_details] (
    [isup_detid]   INT             IDENTITY (1, 1) NOT NULL,
    [inward_supid] INT             NOT NULL,
    [ty]           VARCHAR (10)    NULL,
    [inter]        DECIMAL (18, 2) NULL,
    [intra]        DECIMAL (18, 2) NULL,
    [gstr3bid]     INT             NULL,
    [gstinid]      INT             NULL,
    PRIMARY KEY CLUSTERED ([isup_detid] ASC)
);

