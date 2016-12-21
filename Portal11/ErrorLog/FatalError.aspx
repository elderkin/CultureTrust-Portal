<%@ Page Title="Fatal Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FatalError.aspx.cs" Inherits="Portal11.ErrorLog.FatalError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Internal Logic Error in Portal</h2>
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
                  <strong>Oh snap!</strong> The Portal has encountered an internal logic error - a bug - named 
                    &quot;<asp:Literal runat="server" ID="litErrorText" />&quot; 
                    <br /> on page 
                    &quot;<asp:Literal runat="server" ID="litPageName" />&quot; 
                    <br /> and method 
                    &quot;<asp:Literal runat="server" ID="litMethodName" />&quot; 
                    <br /> from page 
                    &quot;<asp:Literal runat="server" ID="litFromPathAndQuery" />&quot; at: &quot;<asp:Literal runat="server" ID="litCurrentTime" />&quot;
                    <br /> The Portal cannot complete your task. 
                    <br /> 
                  <br />Please write down this information - including the time - and report it to CultureWorks staff. Thank you.
                </div>
            </div>
            <div class="col-xs-3">
                <a href="../Default" class="btn btn-default">Return to Home page</a>
            </div>
        </div>
    </div>
</asp:Content>
