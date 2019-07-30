CREATE TABLE [dbo].[TBL_GSTR3B_inter_sup_comp_det] (
    [comp_detid]  INT             IDENTITY (1, 1) NOT NULL,
    [inter_supid] INT             NOT NULL,
    [pos]         VARCHAR (2)     NULL,
    [txval]       DECIMAL (18, 2) NULL,
    [iamt]        DECIMAL (18, 2) NULL,
    [gstr3bid]    INT             NULL,
    [gstinid]     INT             NULL,
    PRIMARY KEY CLUSTERED ([comp_detid] ASC)
);

