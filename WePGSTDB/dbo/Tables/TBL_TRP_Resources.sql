CREATE TABLE [dbo].[TBL_TRP_Resources] (
    [Resource_ID]           INT            NOT NULL,
    [Resource_Name]         NVARCHAR (100) NOT NULL,
    [Resource_Display_Name] NVARCHAR (100) NOT NULL,
    [Resource_Page_Name]    NVARCHAR (100) NULL,
    [Resource_MenuItem_ID]  TINYINT        NOT NULL,
    [FK_Parent_Resource_ID] INT            NULL,
    CONSTRAINT [PK_TRP_Resource] PRIMARY KEY CLUSTERED ([Resource_ID] ASC)
);

