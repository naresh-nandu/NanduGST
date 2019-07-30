<%@ Page EnableEventValidation="false" Language="C#" AutoEventWireup="true" CodeBehind="Invoice_update.aspx.cs" Inherits="SmartAdminMvc.Invoice_update" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="grdInvoice" runat="server" AutoGenerateColumns="False" Visible="False" BackColor="White" 
                BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical"
                OnRowDataBound="grdInvoice_RowDataBound">
                <AlternatingRowStyle BackColor="#DCDCDC" />
                <Columns>
                    <%--<asp:BoundField DataField="itmsid" HeaderText="HSN" ReadOnly="True" />--%>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                            <asp:Label ID="lblitmsid" runat="server" Width="40px" Visible="false"
                                Text='<%# Eval("itmsid")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tax Value">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxval" runat="server" Width="80px" CssClass="decimalcheck"
                                Text='<%# Eval("txval")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tax rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtirt" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("rt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IGST amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtiamt" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("iamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="CGST Amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcamt" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("camt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="SGST amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtsamt" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("samt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="cess amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcsamt" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("csamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Eligibility">
                        <ItemTemplate>
                            <asp:label ID="lblelg" runat="server" Text='<%# Eval("elg")%>'></asp:label>
                            <asp:DropDownList ID="txtelg" runat="server" Width="60%" OnSelectedIndexChanged="txtelg_SelectedIndexChanged" AutoPostBack="true">                                
                                <asp:ListItem>IP</asp:ListItem>
                                <asp:ListItem>NO</asp:ListItem>                                
                                <asp:ListItem>IS</asp:ListItem>
                                <asp:ListItem>CP</asp:ListItem>                                
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TXI">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxi" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("tx_i")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="TXC">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxc" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("tx_c")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="TXS">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxs" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("tx_s")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                   
                    <asp:TemplateField HeaderText="TXCS">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxcs" runat="server" Width="60px" CssClass="decimalcheck"
                                Text='<%# Eval("tx_cs")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField ItemStyle-Width="80px" HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button runat="server" ID="button1" Text='Update' OnClick="Edit" CssClass="btn btn-primary" />
                        </ItemTemplate>
                        <ItemStyle Width="100px" />
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#0000A9" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#000065" />
            </asp:GridView>
            <asp:GridView ID="gridview2" runat="server" AutoGenerateColumns="False" Visible="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical">
                <AlternatingRowStyle BackColor="#DCDCDC" />
                <Columns>

                    <asp:BoundField DataField="rsn" HeaderText="RSN" ReadOnly="True" />
                    <asp:TemplateField HeaderText="IGST rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtirt" runat="server" Width="60px"
                                Text='<%# Eval("irt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IGST amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtiamt" runat="server" Width="60px"
                                Text='<%# Eval("iamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CGST rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcrt" runat="server" Width="60px"
                                Text='<%# Eval("crt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="CGST Amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcamt" runat="server" Width="60px"
                                Text='<%# Eval("camt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SGST rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtsrt" runat="server" Width="60px"
                                Text='<%# Eval("srt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SGST amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtsamt" runat="server" Width="60px"
                                Text='<%# Eval("samt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cess rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcsrt" runat="server" Width="60px"
                                Text='<%# Eval("csrt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="cess amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcsamt" runat="server" Width="60px"
                                Text='<%# Eval("csamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField ItemStyle-Width="80px" HeaderText="Action">
                        <ItemTemplate>
                            <%--    <asp:Button ID="btn" Text="Edit" runat="server" OnClick="Edit"/>--%>
                            <asp:Button runat="server" ID="button1" Text='Update' OnClick="EditCDNR" />
                        </ItemTemplate>

                        <ItemStyle Width="100px"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#0000A9" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#000065" />
            </asp:GridView>
            <asp:GridView ID="gridview3" runat="server" AutoGenerateColumns="False" Visible="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical">
                <AlternatingRowStyle BackColor="#DCDCDC" />
                <Columns>
                    <asp:BoundField DataField="itmsid" HeaderText="" ReadOnly="True" />
                    <asp:BoundField DataField="hsn_sc" HeaderText="HSN" ReadOnly="True" />
                    <asp:TemplateField HeaderText="Tax Value">
                        <ItemTemplate>
                            <asp:TextBox ID="txttxval" runat="server" Width="80px"
                                Text='<%# Eval("txval")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IGST rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtirt" runat="server" Width="60px"
                                Text='<%# Eval("irt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IGST amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtiamt" runat="server" Width="60px" 
                                Text='<%# Eval("iamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cess rate">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcrt" runat="server" Width="60px"
                                Text='<%# Eval("csrt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cess Amount">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcamt" runat="server" Width="60px"
                                Text='<%# Eval("csamt")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TXI">
                        <ItemTemplate>
                            <asp:TextBox ID="txtsrt" runat="server" Width="60px"
                                Text='<%# Eval("tx_i")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TXCS">
                        <ItemTemplate>
                            <asp:TextBox ID="txtsamt" runat="server" Width="60px"
                                Text='<%# Eval("tx_cs")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TCI">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcsrt" runat="server" Width="60px"
                                Text='<%# Eval("tc_i")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="TCCS">
                        <ItemTemplate>
                            <asp:TextBox ID="txtcsamt" runat="server" Width="60px"
                                Text='<%# Eval("tc_cs")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField ItemStyle-Width="80px" HeaderText="Action">
                        <ItemTemplate>
                            <%--    <asp:Button ID="btn" Text="Edit" runat="server" OnClick="Edit"/>--%>
                            <asp:Button runat="server" ID="button1" Text='Update' OnClick="EditIMPG" />
                        </ItemTemplate>

                        <ItemStyle Width="100px"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                <SortedAscendingHeaderStyle BackColor="#0000A9" />
                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                <SortedDescendingHeaderStyle BackColor="#000065" />
            </asp:GridView>
        </div>
    </form>
</body>
</html>
<script src="http://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>

   <script type="text/javascript">



       $('.decimalcheck').on('keypress', function (e) {
            return validateFloatKeyPress(this, e);
        });


        function validateFloatKeyPress(el, evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            var number = el.value.split('.');
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            //just one dot (thanks ddlab)
            if (number.length > 1 && charCode == 46) {
                return false;
            }
            //get the carat position
            var caratPos = getSelectionStart(el);
            var dotPos = el.value.indexOf(".");
            if (caratPos > dotPos && dotPos > -1 && (number[1].length > 1)) {
                return false;
            }
            return true;
        }
       </script>