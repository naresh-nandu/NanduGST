CREATE TABLE [dbo].[TBL_GSTR3B_D_itc_elg_itc_inelg] (
    [itc_inelgid] INT             IDENTITY (1, 1) NOT NULL,
    [itc_elgid]   INT             NOT NULL,
    [ty]          VARCHAR (10)    NOT NULL,
    [iamt]        DECIMAL (18, 2) NOT NULL,
    [camt]        DECIMAL (18, 2) NULL,
    [samt]        DECIMAL (18, 2) NULL,
    [csamt]       DECIMAL (18, 2) NULL,
    [gstr3bdid]   INT             NULL,
    [gstinid]     INT             NULL,
    PRIMARY KEY CLUSTERED ([itc_inelgid] ASC)
);

