<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASPTask2.Default" MasterPageFile="main.Master" %>

<asp:Content runat="server" ID="headContent" ContentPlaceHolderID="head">
    <title>Главная</title>
</asp:Content>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="content">
    <div class="content">
        <asp:Label ID="Label1" runat="server" Text="Добро пожаловать на сайт со статистикой о самом себе." EnableViewState="False"></asp:Label>
    </div>
</asp:Content>
