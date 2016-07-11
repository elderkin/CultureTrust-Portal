<%@ Page Title="Select Grant" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectGrant.aspx.cs" 
    Inherits="Portal11.Select.SelectGrant" EnableEventValidation="false" %>
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
            <h4>Grants</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for Grants. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnGrantSearch">
                <asp:TextBox ID="txtGrant" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnGrantHelper" runat="server" Style="display: none" OnClick="btnGrantSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnGrantSearch" runat="server" CssClass="btn btn-default" OnClick="btnGrantSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

        <div class="panel panel-default col-md-3 col-xs-7">
            <asp:Panel runat="server">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive Grants" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkInactive_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive Grants in this list" />
            </asp:Panel>
        </div>
    </div>

 

    <!-- GridView of Grants -->
    <div class="row">
        <asp:Panel ID="pnlGrant" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that GrantID is the first column of this grid -->
                <asp:GridView ID="GrantView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.Grant"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="GrantView_RowDataBound"
                    OnSelectedIndexChanged="GrantView_SelectedIndexChanged"
                    OnPageIndexChanging="GrantView_PageIndexChanging">

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
                                <td>There are no Grants available</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("GrantID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Name" HeaderText="Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="OriginalFunds" HeaderText="Original Funds" 
                            DataFormatString="${0:###,###,###.00}" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20"/>
                        <asp:BoundField DataField="CurrentFunds" HeaderText="Current Funds" 
                            DataFormatString="${0:###,###,###.00}" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20"/>
                        <asp:BoundField DataField="Inactive" HeaderText="Inactive" ItemStyle-Width="8"/>
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>
    </div>

    <!-- Button array -->
    <div class="row">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-md-1 col-xs-2" Enabled="true"
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this Grant for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

</div>
</asp:Content>
