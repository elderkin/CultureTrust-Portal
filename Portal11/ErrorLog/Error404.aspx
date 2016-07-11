<%@ Page Title="Error404" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error404.aspx.cs" Inherits="Portal11.Error404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Error: Requested web page not found</h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">
        <div class="row">
            <div class="col-xs-12">
                <div class="alert alert-dismissible alert-danger">
                  <strong>Oh snap!</strong> The requested web page 
                    '<asp:Literal runat="server" ID="litPageName" />' could not be found. 
                  <br />  If you think that the Portal reached this page in error, please contact CultureWorks staff.
                </div>
            </div>
            <div class="col-xs-3">
                <a href="../Default" class="btn btn-default">Return to Home page</a>
            </div>
        </div>
    </div>
</asp:Content>
