<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="WebApplication1.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Contact Information.</h3>
    <address>
        One Bbclark Way<br />
        Alexandria, VA 98052-6399<br />
        <abbr title="Phone">P:</abbr>
        XXX.XXX.0100
    </address>

    <address>
        <strong>Support:</strong>   <a href="mailto:Support@bbclark.com">Support@bbclark.com</a><br />
        <strong>Marketing:</strong> <a href="mailto:Marketing@bbclark.com">Marketing@bbclark.com</a>
    </address>
</asp:Content>
