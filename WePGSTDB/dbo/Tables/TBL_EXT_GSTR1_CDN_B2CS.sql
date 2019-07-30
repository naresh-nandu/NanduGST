CREATE TABLE [dbo].[TBL_EXT_GSTR1_CDN_B2CS] (
    [filename] VARCHAR (MAX)   NULL,
    [gstin]    VARCHAR (50)    NULL,
    [fp]       VARCHAR (50)    NULL,
    [pos]      VARCHAR (2)     NULL,
    [sply_ty]  VARCHAR (5)     NULL,
    [ntty]     VARCHAR (1)     NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL
);

