<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="ASPTask2.Help" MasterPageFile="../main.Master" %>


<asp:Content runat="server" ID="headContent" ContentPlaceHolderID="head">
    <title>Помощь</title>
</asp:Content>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="content">
    <div class="content">
        <h2>Задание:</h2>
        <p>Создать страницу для отображения статистики web-приложения. Она должна содержать в себе следующую информацию:</p>
        <ol>
            <li>Количество запросов к web-приложению (за все время).</li>
            <li>Количество запросов к web-приложению (за день).</li>
            <li>Количество запросов к конкретной странице (за все время).</li>
            <li>Количество уникальных посетителей (за все время).</li>
            <li>Количество посетителей (за день).</li>
        </ol>
        <p>
            Нельзя использовать IIS Counters. Реализация должна быть основана на 
            основании материала из текущего модуля (Session, Application и так далее)<br />
            Список страниц, для которых необходимо отображать статистику, должен хранится в 
            текстовом файле. Путь к файлу хранить в файле web.config. Для тестирования создать не менее 3 страниц.
        </p>

    </div>
</asp:Content>
