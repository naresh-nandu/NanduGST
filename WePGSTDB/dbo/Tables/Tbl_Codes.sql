CREATE TABLE [dbo].[Tbl_Codes] (
    [CodeId]         SMALLINT     NOT NULL,
    [CodeCategoryId] SMALLINT     NOT NULL,
    [CodeName]       VARCHAR (72) NOT NULL,
    [CodeShortName]  VARCHAR (35) NULL,
    [CodeInUse]      BIT          NOT NULL,
    PRIMARY KEY CLUSTERED ([CodeId] ASC, [CodeCategoryId] ASC)
);

