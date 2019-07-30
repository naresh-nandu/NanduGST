CREATE TABLE [dbo].[TBL_GSTR3B_inter_sup_unreg_det] (
    [unreg_detid] INT             IDENTITY (1, 1) NOT NULL,
    [inter_supid] INT             NOT NULL,
    [pos]         VARCHAR (2)     NULL,
    [txval]       DECIMAL (18, 2) NULL,
    [iamt]        DECIMAL (18, 2) NULL,
    [gstr3bid]    INT             NOT NULL,
    [gstinid]     INT             NULL,
    PRIMARY KEY CLUSTERED ([unreg_detid] ASC)
);

