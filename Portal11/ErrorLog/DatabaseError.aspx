<%@ Page Title="Database Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DatabaseError.aspx.cs" Inherits="Portal11.ErrorLog.DatabaseError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Database Error</h2>
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
                  <strong>Oh snap!</strong> The Portal has encountered an error while writing to its database. The name of the error is 
                    &quot;<asp:Literal runat="server" ID="litErrorText" />&quot; 
                    <br /> on page 
                    &quot;<asp:Literal runat="server" ID="litPageName" />&quot; and cannot complete your task. 
                  <br />Please report this problem to CultureWorks staff.
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-6">
                <div class="row">
                    <asp:Panel runat="server" ID="pnlValidationErrors" Visible="false">
                    <div class="col-xs-offset-1 col-xs-11">
                        <h4>Data Validation Error(s) while writing new database row(s)</h4>
                    </div>
                    <div class="col-xs-12">
                        <asp:ListBox ID="lstValidationErrors" runat="server" CssClass="form-control" CausesValidation="false" Rows="3" Enabled="false" />
                    </div>
                    </asp:Panel>
                </div>
            </div>

            <div class="col-xs-3">
                <a href="../Default" class="btn btn-default">Return to Home page</a>
            </div>
        </div>
    </div>

</asp:Content>
