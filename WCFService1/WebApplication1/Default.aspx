<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <p><%: X + Y %></p>

    <div class="jumbotron">
        <h1>Becham | Brown | Clark</h1>
        <p class="lead">BBCLARK is using a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>
    <asp:TextBox AutoPostBack="false" ID="txtLogFilter" OnTextChanged="txtLogFilter_TextChanged" TextMode="SingleLine" ToolTip="Source name filter (leave blank for all logs)" runat="server" />
    <br />
    <asp:Label ID="lblWriteError" Text="Error: unable to write" ForeColor="Red" Visible="false" runat="server" />
    <br />
    <asp:button ID="btnRead" Text="Read" OnClick="btnread_Click" runat="server" />
    <asp:Label ID="lblReadError" Text="Error: unable to read" ForeColor="Red" Visible="false" runat="server" />
    <br />
    <asp:Table BorderColor="Black" BorderStyle="Double" BorderWidth="3" ID="tblReadLogs" Visible="false" runat="server">
        <asp:TableHeaderRow BorderColor="Black" BorderStyle="Solid" BorderWidth="2" ID="tblReadLogsHeader" runat="server">
            <asp:TableHeaderCell ID="tblReadLogsHeaderDate" Text="Date" Font-Bold="true" runat="server" />
            <asp:TableHeaderCell ID="tblReadLogsHeaderService" Text="Service name" Font-Bold="true" runat="server" />
            <asp:TableHeaderCell ID="tblReadLogsHeaderMachine" Text="Machine name" Font-Bold="true" runat="server" />
            <asp:TableHeaderCell ID="tblReadLogsHeaderUser" Text="User" Font-Bold="true" runat="server" />
            <asp:TableHeaderCell ID="tblReadLogsHeaderCategory" Text="Category" Font-Bold="true" runat="server" />
            <asp:TableHeaderCell ID="tblReadLogsHeaderMessage" Text="Message" Font-Bold="true" runat="server" />
        </asp:TableHeaderRow>
    </asp:Table>
    <div style="column-fill:balance;align-self:center">
        <asp:Button ID="btnReadLogsFirst" Text="<<" OnClick="btnReadLogsFirst_Click" runat="server" />
        <asp:Button ID="btnReadLogsPrevious" Text="<" OnClick="btnReadLogsPrevious_Click" runat="server" />
        <asp:Label ID="lblReadLogsPage" Visible="false" runat="server" />
        <asp:Button ID="btnReadLogsNext" Text=">" OnClick="btnReadLogsNext_Click" runat="server" />
        <asp:Button ID="btnReadLogsLast" Text=">>" OnClick="btnReadLogsLast_Click" runat="server" />
        <span style="width:50px" />
        <asp:Button ID="btnReadLogsGoto" Text="Go to page:" OnClick="btnReadLogsGoto_Click" runat="server" />
        <asp:TextBox ID="txtReadLogsGoto" TextMode="Number" runat="server" />
    </div>
    <br />
    <div class="row">
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                ASP.NET Web Forms lets you build dynamic websites using a familiar drag-and-drop, event-driven model.
            A design surface and hundreds of controls and components let you rapidly build sophisticated, powerful UI-driven sites with data access.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
            <p>
                NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
