<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page.aspx.cs" Inherits="ASPTask2.Page" MasterPageFile="main.Master"%>

<asp:Content runat="server" ID="headContent" ContentPlaceHolderID="head">
    <title>Страница</title>
</asp:Content>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="content">
    <div class="content">
        <asp:Label runat="server" Text="Это обычная страница, для которой не ведется статистика"></asp:Label>
    </div>
</asp:Content>
