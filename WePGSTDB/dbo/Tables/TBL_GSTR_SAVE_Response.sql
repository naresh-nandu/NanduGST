CREATE TABLE [dbo].[TBL_GSTR_SAVE_Response] (
    [GSTRSaveId]  INT          IDENTITY (1, 1) NOT NULL,
    [GSTINNo]     VARCHAR (15) NULL,
    [GSTRName]    VARCHAR (50) NULL,
    [TransId]     VARCHAR (50) NULL,
    [RefId]       VARCHAR (50) NULL,
    [CustomerId]  INT          NULL,
    [UserId]      INT          NULL,
    [CreatedDate] DATETIME     NULL,
    [fp]          VARCHAR (6)  NULL,
    [ActionType]  VARCHAR (10) NULL,
    [batchid]     INT          NULL,
    CONSTRAINT [PK_TBL_GSTR_SAVE_Response] PRIMARY KEY CLUSTERED ([GSTRSaveId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR_SAVE_Response_C1C5C61485F47AF80ECCA36351A75749]
    ON [dbo].[TBL_GSTR_SAVE_Response]([fp] ASC, [GSTRName] ASC)
    INCLUDE([GSTINNo]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_TBL_GSTR_SAVE_Response_F342A0964BAF476AF0A2E4227C3AA261]
    ON [dbo].[TBL_GSTR_SAVE_Response]([GSTRName] ASC)
    INCLUDE([CreatedDate], [GSTINNo]);

