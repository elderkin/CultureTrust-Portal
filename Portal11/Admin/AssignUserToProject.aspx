<%@ Page Title="Assign User to Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AssignUserToProject.aspx.cs" Inherits="Portal11.Admin.AssignUserToProject" EnableEventValidation="false" %>
<%@ Import Namespace="Portal11.Logic" %>

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
            <h4>Projects</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Projects. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->

    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnProjectSearch">
                <asp:TextBox ID="txtProject" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnProjectHelper" runat="server" Style="display: none" OnClick="btnProjectSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnProjectSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

<%--  Can't assign a User to an Inactive Project, so no need for this control
        <div class="panel panel-default col-md-3 col-xs-7">
            <asp:Panel runat="server">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Projects" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Projects in this list" />
            </asp:Panel>
        </div>
--%>
    </div>

    <div class="row">
        <asp:Panel ID="pnlProject" runat="server">
            <div class="col-md-10 col-xs-12">

                <!-- Code assumes that ProjectID is the first column of this grid -->
                <asp:GridView ID="AssignUserToProjectView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.AssignUserToProjectViewRow"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="AssignUserToProjectView_RowDataBound"
                    OnSelectedIndexChanged="AssignUserToProjectView_SelectedIndexChanged"
                    OnPageIndexChanging="AssignUserToProjectView_PageIndexChanging">

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
                                <td>There are no Projects available</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("RowID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Internal" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblProjectRole" runat="server" Text='<%# Bind("ProjectRole") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Project Name" />
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description").ToString().TrimString(40) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CurrentPD" HeaderText="Current Project Director" />
                        <asp:BoundField DataField="UserRole" HeaderText="Selected User Role" />
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>
    </div>

    <!-- Button array -->
    <div class="row">
        <asp:Button ID="btnCancel" runat="server" Text="Done" CssClass="btn btn-default col-md-1 col-xs-2" Enabled="true"
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
        <asp:Button ID="btnAddDirector" runat="server" CssClass="btn btn-default col-xs-offset-1 col-xs-2" Enabled="false"
            OnClick="btnAddDirector_Click" ToolTip="Make selected user the Project Director of this Project" Text="Set as Director" />
        <asp:Button ID="btnAddStaff" runat="server" CssClass="btn btn-default col-xs-offset-1 col-xs-2" Enabled="false"
            OnClick="btnAddStaff_Click" ToolTip="Make selected user Project Staff of this Project" Text="Add as Staff" />
        <asp:Button ID="btnRemove" runat="server" CssClass="btn btn-default col-xs-offset-1 col-xs-2" Enabled="false"
            OnClick="btnRemove_Click" ToolTip="Remove selected user from current role in Project" Text="Remove" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
    <asp:Literal ID="litSavedFullName" runat="server" Visible="false" />

    </div>

</asp:Content>
