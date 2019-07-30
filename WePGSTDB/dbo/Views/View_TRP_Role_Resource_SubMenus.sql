CREATE VIEW [dbo].[View_TRP_Role_Resource_SubMenus]
AS
SELECT        dbo.TBL_TRP_Resources.Resource_ID, dbo.TBL_TRP_Resources.Resource_Name, dbo.TBL_TRP_Resources.Resource_Display_Name, dbo.TBL_TRP_Resources.Resource_Page_Name, 
                         dbo.TBL_TRP_Resources.Resource_MenuItem_ID, dbo.TBL_TRP_Resources.FK_Parent_Resource_ID, dbo.TBL_TRP_Roles_Resources.Role_Resource_ID, dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Role_ID, 
                         dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Resource_ID, dbo.TBL_TRP_Roles_Resources.Role_Resource_IsAssigned
FROM            dbo.TBL_TRP_Resources INNER JOIN
                         dbo.TBL_TRP_Roles_Resources ON dbo.TBL_TRP_Resources.Resource_ID = dbo.TBL_TRP_Roles_Resources.FK_Role_Resource_Resource_ID