CREATE TABLE [dbo].[Tbl_CodeCategories] (
    [CodeCategoryId]        SMALLINT     NOT NULL,
    [CodeCategoryName]      VARCHAR (72) NOT NULL,
    [CodeCategoryShortName] VARCHAR (35) NULL,
    [CodeCategoryInUse]     BIT          NOT NULL,
    PRIMARY KEY CLUSTERED ([CodeCategoryId] ASC)
);

