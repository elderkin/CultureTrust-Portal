<%@ Page Title="Select Entity" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectEntity.aspx.cs" 
    Inherits="Portal11.Select.SelectEntity" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<%@ Import Namespace="Portal11.Logic" %>

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
            <h4>Entities</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Entities. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnEntitySearch">
                <asp:TextBox ID="txtEntity" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnEntityHelper" runat="server" Style="display: none" OnClick="btnEntitySearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnEntitySearch" runat="server" CssClass="btn btn-default" OnClick="btnEntitySearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

            <asp:Panel ID="pnlInactive" runat="server">
        <div class="panel panel-default col-md-3 col-xs-7">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Entities" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Entities in this list" />
        </div>
            </asp:Panel>
    </div>

     <div class="row">
 
       <!-- GridView of All Entities -->

       <asp:Panel ID="pnlAllEntity" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that EntityID is the first column of this grid -->
                <asp:GridView ID="gvAllEntity" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.Entity"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="gvAllEntity_RowDataBound"
                    OnSelectedIndexChanged="gvAllEntity_SelectedIndexChanged"
                    OnPageIndexChanging="gvAllEntity_PageIndexChanging">

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
                                <td>There are no Entities available</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("EntityID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Entity Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email">
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email").ToString().TrimString(40) %>' />
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
    </div>

    <!-- Button array -->
    <div class="row">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-md-1 col-xs-2" Enabled="true"
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
        <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
            OnClick="btnNew_Click" CausesValidation="false" ToolTip="Create a new Entity" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this Entity for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>


</asp:Content>
