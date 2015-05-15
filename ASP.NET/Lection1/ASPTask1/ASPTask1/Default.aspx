<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASPTask1.Default" Trace="true" TraceMode="SortByTime" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <hr/>
    <form id="form1" runat="server">
    <div>
        <asp:Table ID="Table1" runat="server" BorderStyle="Double" CellPadding="5" GridLines="Both">
            <asp:TableRow runat="server" HorizontalAlign="Center">
                <asp:TableCell runat="server" HorizontalAlign="Center">Keys</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Values</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <hr/>
        <asp:Button ID="Button1" runat="server" Text="Отправить запрос" OnClick="Button1_OnClick"/>
    </div>
    </form>
</body>
</html>
