<%@ Page Title="Select GL Code" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SelectGLCode.aspx.cs" 
    Inherits="Portal11.Select.SelectGLCode" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"%>
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
            <h4>GL Codes</h4>
        </div>
    </div>

    <!-- Create a search box with a search button for GL Codes. There's a hidden button that lets an "Enter" in the
        Text Box trigger the search just like the Search button  -->
    <div class="row">
        <div class="form-group col-lg-4 col-md-5 col-xs-12">
            <asp:Panel runat="server" DefaultButton="btnGLCodeSearch">
                <asp:TextBox ID="txtGLCode" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                <asp:Button ID="btnGLCodeHelper" runat="server" Style="display: none" OnClick="btnGLCodeSearch_Click" />
            </asp:Panel>
            <asp:LinkButton ID="btnGLCodeSearch" runat="server" CssClass="btn btn-default" OnClick="btnGLCodeSearch_Click">
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
            </asp:LinkButton>
        </div>

        <div class="panel panel-default col-md-3 col-xs-7">
            <asp:Panel runat="server">
                    <asp:CheckBox ID="chkInactive" runat="server" Text="Include Inactive GLCodes" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkAny_CheckedChanged" AutoPostBack="true"
                        ToolTip="Check to include Inactive GLCodes in this list" />
                    <asp:CheckBox ID="chkDeposit" runat="server" Text="Include Deposit GLCodes" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkAny_CheckedChanged" Checked="true" AutoPostBack="true"
                        ToolTip="Check to include Deposit GLCodes in this list" />
                    <asp:CheckBox ID="chkExpense" runat="server" Text="Include Expense GLCodes" CssClass="checkbox col-xs-12"
                        OnCheckedChanged="chkAny_CheckedChanged" Checked="true" AutoPostBack="true"
                        ToolTip="Check to include Expense GLCodes in this list" />
            </asp:Panel>
        </div>
    </div>

 

    <!-- GridView of GLCodes -->
    <div class="row">
        <asp:Panel ID="pnlGLCode" runat="server">
            <div class="col-md-7 col-xs-12">

                <!-- Code assumes that GLCodeID is the first column of this grid -->
                <asp:GridView ID="GLCodeView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.GLCode"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="10"
                    OnRowDataBound="GLCodeView_RowDataBound"
                    OnSelectedIndexChanged="GLCodeView_SelectedIndexChanged"
                    OnPageIndexChanging="GLCodeView_PageIndexChanging">

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
                                <td>There are no GLCodes that match your search and filter criteria</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("GLCodeID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Code" HeaderText="Code" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="DepCode" HeaderText="Deposit" ItemStyle-Width="8"/>
                        <asp:BoundField DataField="ExpCode" HeaderText="Expense" ItemStyle-Width="8"/>
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
            OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without selecting a GL Code" />
        <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
            OnClick="btnNew_Click" CausesValidation="false" ToolTip="Create a new GL Code" />
        <asp:Button ID="btnSelect" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="false"
            OnClick="btnSelect_Click" ToolTip="Choose this GLCode for further processing" Text="Select" />
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

</div>

</asp:Content>
