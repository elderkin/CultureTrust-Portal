<%@ Page Title="Select Vendor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectVendor.aspx.cs" 
    Inherits="Portal11.Select.SelectVendor" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
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
            <h4>Vendors</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Vendors. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnVendorSearch">
                <asp:TextBox ID="txtVendor" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnVendorHelper" runat="server" Style="display: none" OnClick="btnVendorSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnVendorSearch" runat="server" CssClass="btn btn-default" OnClick="btnVendorSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

            <asp:Panel ID="pnlInactive" runat="server">
        <div class="panel panel-default col-md-3 col-xs-7">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Vendors" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Vendors in this list" />
        </div>
            </asp:Panel>
    </div>

     <div class="row">
 
       <!-- GridView of All Vendors -->

       <asp:Panel ID="pnlAllVendor" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that VendorID is the first column of this grid -->
                <asp:GridView ID="AllVendorView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.Vendor"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="AllVendorView_RowDataBound"
                    OnSelectedIndexChanged="AllVendorView_SelectedIndexChanged"
                    OnPageIndexChanging="AllVendorView_PageIndexChanging">

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
                                <td>There are no Vendors available</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("VendorID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Vendor Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email">
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email").ToString().TrimString(40) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Inactive" HeaderText="Inactive" />
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
            OnClick="btnNew_Click" CausesValidation="false" ToolTip="Create a new Vendor" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this Vendor for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>

</asp:Content>
