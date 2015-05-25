<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Stats.aspx.cs" Inherits="ASPTask2.Stats" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Statistics</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="menu">
            <ul>
                <li></li>
            </ul>
        </div>
    <div class ="content">
        <asp:Button ID="Button1" runat="server" Text="Обновить" />
        <asp:Label ID="Label1" runat="server" Text="Label" EnableViewState="False"></asp:Label>
    </div>
    </form>
</body>
</html>
