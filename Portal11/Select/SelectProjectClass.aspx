﻿<%@ Page Title="Select Project Class" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectProjectClass.aspx.cs" 
    Inherits="Portal11.Select.SelectProjectClass" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
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
            <h4>Project Classes</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Project Classes. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnProjectClassSearch">
                <asp:TextBox ID="txtProjectClass" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnProjectClassHelper" runat="server" Style="display: none" OnClick="btnProjectClassSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnProjectClassSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectClassSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

        <div class="panel panel-default col-md-3 col-xs-7">
            <asp:Panel runat="server">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Project Classes" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Project Classess in this list" />
            </asp:Panel>
        </div>
    </div>

 

    <!-- GridView of Cs -->
    <div class="row">
        <asp:Panel ID="pnlProjectClass" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that ProjectClassID is the first column of this grid -->
                <asp:GridView ID="ProjectClassView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectClass"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="ProjectClassView_RowDataBound"
                    OnSelectedIndexChanged="ProjectClassView_SelectedIndexChanged"
                    OnPageIndexChanging="ProjectClassView_PageIndexChanging">

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
                                <td>There are no Project Classes available for this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectClassID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="CreatedFromMaster" HeaderText="From Master" />
                        <asp:BoundField DataField="Default" HeaderText="Default" />
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
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without selecting an Project Class" />
        <asp:Button ID="btnNew" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
            OnClick="btnNew_Click" ToolTip="Create a new Project Class" Text="New" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this Project Class for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
    </div>

</asp:Content>
