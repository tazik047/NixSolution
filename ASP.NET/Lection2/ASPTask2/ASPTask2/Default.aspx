<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASPTask2.Default" Trace="true" TraceMode="SortByCategory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="menu">
                <asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1" MaximumDynamicDisplayLevels="0" RenderingMode="List" StaticDisplayLevels="3"></asp:Menu>
                <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" />
            </div>
            <div class="content">
                <asp:Label ID="Label1" runat="server" Text="Label" EnableViewState="False"></asp:Label>
                <asp:Button ID="Button1" runat="server" Text="Button" />
                <%=Application["count"] %>
            </div>


        </div>
    </form>
</body>
</html>
