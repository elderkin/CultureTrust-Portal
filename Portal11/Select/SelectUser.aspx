<%@ Page Title="Select User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectUser.aspx.cs" 
    Inherits="Portal11.Admin.SelectUser" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>
    <hr />
    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">


    <div class="row">
        <div class="col-xs-6">
            <h4>Users</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Users. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnUserSearch">
                <asp:TextBox ID="txtUser" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnUserHelper" runat="server" Style="display: none" OnClick="btnUserSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnUserSearch" runat="server" CssClass="btn btn-default" OnClick="btnUserSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

        <div class="panel panel-default col-md-3 col-xs-7">
            <asp:Panel runat="server">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Users" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Users in this list" />
            </asp:Panel>
        </div>
    </div>

 

    <!-- GridView of Users -->
    <div class="row">
        <asp:Panel ID="pnlUser" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that UserID is the first column of this grid -->
                <asp:GridView ID="UserView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ApplicationUser"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="UserView_RowDataBound"
                    OnSelectedIndexChanged="UserView_SelectedIndexChanged"
                    OnPageIndexChanging="UserView_PageIndexChanging">

                    <SelectedRowStyle CssClass="success" />

                    <PagerStyle CssClass="active" HorizontalAlign="Center"></PagerStyle>
                    <PagerTemplate>
                        <asp:Button ID="ButtonFirst" runat="server" Text="<<" CommandName="Page"
                            CommandArgument="First"
                            CssClass="btn btn-sm btn-default"></asp:Button>
                        <asp:Button ID="ButtonPrev" runat="server" Text="<" CommandName="Page"
                            CommandArgument="Prev"
                            CssClass="btn btn-sm btn-default"></asp:Button>
                        <asp:Button ID="ButtonNext" runat="server" Text=">" CommandName="Page"
                            CommandArgument="Next"
                            CssClass="btn btn-sm btn-default"></asp:Button>
                        <asp:Button ID="ButtonLast" runat="server" Text=">>" CommandName="Page"
                            CommandArgument="Last" Enabled="false"
                            CssClass="btn btn-sm btn-default"></asp:Button>
                    </PagerTemplate>

                    <EmptyDataTemplate>
                        <table>
                            <tr>
                                <td>There are no Users available. How did you manage to login?</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Name">
                            <ItemTemplate>
                                <asp:Label ID="lblFullName" runat="server" Text='<%# Bind("FullName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>
    </div>

    <!-- Button array -->
    <div class="row">
        <asp:Button ID="btnCancelx" runat="server" Text="Cancel" CssClass="btn btn-default col-md-1 col-xs-2" Enabled="true"
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
        <asp:Button ID="btnSelectx" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this User for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>
</asp:Content>
