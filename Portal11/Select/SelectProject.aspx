<%@ Page Title="Select Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectProject.aspx.cs" 
    Inherits="Portal11.Staff.SelectProject" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="Portal11.Logic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .panel {
            padding-bottom: 10px;
        }
    </style>

<%--This is the Admin version --%>

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
            <h4>Projects (Name or Code)</h4>
        </div>
    </div>

    <%--Create a search box with a search button for Projects. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button,--%>
    <div class="row">
        <div class="form-group col-xs-12">
            <div class="col-xs-3">
                <asp:Panel runat="server" DefaultButton="btnProjectSearch">
                    <asp:TextBox ID="txtProject" runat="server" CssClass="form-control has-success"></asp:TextBox>
                    <asp:Button ID="btnProjectHelper" runat="server" Style="display: none" OnClick="btnProjectSearch_Click" />
                </asp:Panel>
            </div>
            
            <div class="col-xs-1">
            <asp:LinkButton ID="btnProjectSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
            </div>

            <div class="col-xs-3">
                <asp:Panel ID="pnlInactive" runat="server">
                    <div class="panel panel-default col-xs-12">
                            <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Projects" CssClass="checkbox col-xs-12"
                                OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                                ToolTip="Check to include Inactive Projects in this list" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

     <div class="row">
 
       <%--GridView of All Projects--%>

       <asp:Panel ID="pnlAllProject" runat="server">
            <div class="col-md-10 col-xs-12">

                <%--Code assumes that ProjectID is the first column of this grid--%>
                <asp:GridView ID="gvAllProject" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.rowSelectProjectAllView"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="gvAllProject_RowDataBound"
                    OnSelectedIndexChanged="ProjectView_SelectedIndexChanged"
                    OnPageIndexChanging="gvAllProject_PageIndexChanging">

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
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Project Code">
                            <ItemTemplate>
                                <asp:Label ID="lblCode" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Project Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description").ToString().TrimString(80) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="To Review">
                            <ItemTemplate>
                                <asp:Label ID="lblCount" runat="server" Text='<%# Bind("Count") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Inactive">
                            <ItemTemplate>
                                <asp:Label ID="lblInactive" runat="server" Text='<%# Bind("Inactive") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>

        <%--GridView of User Projects--%>

        <asp:Panel ID="pnlUserProject" runat="server">
            <div class="col-md-10 col-xs-12">

                <%--Code assumes that ProjectID is the first column of this grid--%>
                <asp:GridView ID="gvUserProject" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.rowSelectProjectUserView"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="gvUserProject_RowDataBound"
                    OnSelectedIndexChanged="ProjectView_SelectedIndexChanged"
                    OnPageIndexChanging="gvUserProject_PageIndexChanging">

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
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Project Code">
                            <ItemTemplate>
                                <asp:Label ID="lblCode" runat="server" Text='<%# Bind("Code") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Project Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description").ToString().TrimString(40) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ProjectRole"  HeaderText="Your Role" />
                        <asp:TemplateField HeaderText="To Review">
                            <ItemTemplate>
                                <asp:Label ID="lblCount" runat="server" Text='<%# Bind("Count") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>

    </div>

    <%--Button array--%>

    <div class="row">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-md-1 col-xs-2" Enabled="true"
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
        <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnNew_Click" CausesValidation="false" ToolTip="Create a new Project" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this Project for further processing" Text="Select" />
    </div>

    <%--"Scratch" storage used during form processing--%>

    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
    <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
    <asp:Literal ID="litSavedRole" runat="server" Visible="false" />
    <asp:Literal ID="litSavedProjectSelector" runat="server" Visible="false" />

    </div>
</asp:Content>
