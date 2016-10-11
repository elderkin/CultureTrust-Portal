<%@ Page Title="List Portal Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListPortalUsers.aspx.cs" Inherits="Portal11.Lists.ListPortalUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Make a printable list of all Portal Users --%>
    <h2><%: Title %></h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>

    <div class="form-horizontal">

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

        <!-- Make a grid of Portal Users. Wrap it in a Panel to make it printable. -->
        
    <asp:Panel ID="pnlGridView" runat="server">
        <div class="row">
            <div class="col-xs-12">

                <asp:GridView ID="AllPortalUsersView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.AllPortalUsersRow"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnPageIndexChanging="AllPortalUsersView_PageIndexChanging"
                    OnRowDataBound="AllPortalUsersView_RowDataBound">

                    <HeaderStyle HorizontalAlign="Center" />

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
                                <td>There are no Portal Users</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblUserID" runat="server" Text='<%# Bind("UserID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="FullName" HeaderText="Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="UserRoleDesc" HeaderText="User Role" />
                        <asp:BoundField DataField="Administrator" HeaderText="Admin" />
                        <asp:BoundField DataField="LoginCount" HeaderText="Login Count" />
                        <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:G}" />
                        <asp:TemplateField HeaderText="Inactive">
                            <ItemTemplate>
                                <asp:Label ID="lblInactive" runat="server" Text='<%# Bind("Inactive") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        </div>
    </asp:Panel>

        <!-- Button array -->

        <div class="row">
            <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-sm-offset-0 col-xs-offset-1 col-xs-2" Enabled="true"
                OnClick="btnDone_Click" ToolTip="Return to the Dashboard" />
            <asp:Button ID="btnPrint" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClientClick="PrintPage();" ToolTip="Print this grid of information" Text="Print" CausesValidation="false"/>
        </div>

    </div>

    <!-- Little script to make Print button work -->

    <script type="text/javascript">

        function PrintPage() {
            var printContent = document.getElementById('<%= pnlGridView.ClientID %>');
            var printWindow = window.open("All Records", "Print Panel", 'left=50000,top=50000,width=0,height=0');
            printWindow.document.write(printContent.innerHTML);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
        }

    </script>

</asp:Content>
