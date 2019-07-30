<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanDocuments.aspx.cs" Inherits="SmartAdminMvc.PanDocuments" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered table-hover"
                HorizontalAlign="Justify" DataKeyNames="PANdocId" OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                ToolTip="File Download Tool" CellPadding="4">
                <RowStyle BackColor="#E3EAEB" />
                <Columns>
                    <asp:CommandField ShowSelectButton="True" SelectText="Download" ControlStyle-ForeColor="Blue" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
