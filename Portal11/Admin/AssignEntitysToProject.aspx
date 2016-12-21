<%@ Page Title="Assign Entitys to Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignEntitysToProject.aspx.cs" 
    Inherits="Portal11.Admin.AssignEntityToProject" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %> '<asp:Literal runat="server" ID="litProjectName" />'</h2>
    <hr />

    <style>
        .rdoColWidth tr td {
            width: 30%;
        }
        hr {
          border: 0;
          clear:both;
          display:block;
          width: 96%;               
          background-color:#e8e8e8;
          height: 1px;
        }
    </style>

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">

        <%--The Project Entity grid gets a width of 4. The Entity Role buttons in the middle get a width of 4. The Entity grid gets a width of 4.--%>
        <%--Left column contains Project Entitys, a search box, and a grid view to list project Entitys--%>
        <div class="col-xs-4">
            <div class="row">
                <div class="col-xs-12">
                    <h4>Entitys on Project in Role</h4>
                </div>

                <%--Create a search box with a search button for Project Entitys. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button.--%>
                <div class="col-xs-12">
                    <asp:Panel runat="server" DefaultButton="btnProjectEntitySearch">
                        <asp:TextBox ID="txtProjectEntity" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                        <asp:Button ID="btnProjectEntityHelper" runat="server" Style="display: none" OnClick="btnProjectEntitySearch_Click" />
                    </asp:Panel>
                    <asp:LinkButton ID="btnProjectEntitySearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectEntitySearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>
                <br />
                &nbsp;


                <%--GridView of Entitys assigned to this Project--%>
                <div class="col-xs-12">
                    <asp:Panel ID="pnlProjectEntity" runat="server">

                        <!-- Code assumes that ProjectEntityID is the first column of this grid -->
                        <asp:GridView ID="ProjectEntityView" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.ProjectEntity"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="25"
                            OnRowDataBound="ProjectEntityView_RowDataBound"
                            OnSelectedIndexChanged="ProjectEntityView_SelectedIndexChanged"
                            OnPageIndexChanging="ProjectEntityView_PageIndexChanging">

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
                                        <td>There are no Entitys assigned to this Project in the selected Role</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="ID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectEntityID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Entity.Name" HeaderText="Name" />
                                <asp:BoundField DataField="Entity.Email" HeaderText="Email" />
                            </Columns>
                        </asp:GridView>

                    </asp:Panel>
                </div>
            </div>
        </div>

        <%--Center column contains radio buttons and movement buttons--%>
        <div class="col-xs-4">
            <div class="row">
                <div class="col-xs-12">
                    <h4>Entity Role</h4>
                </div>

                <%--Radio buttons to select the role of Entity to be processed--%>
                <div class="col-xs-12">
                    <div class="panel panel-default">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoEntityRole" runat="server" AutoPostBack="true"
                                Style="margin-left: 40px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                
                                OnSelectedIndexChanged="rdoEntityRole_SelectedIndexChanged">
                                <asp:ListItem Text="Entity (for Deposit)" Value="DepositEntity" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Vendor (for Expense)" Value="ExpenseVendor"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <%--Buttons to move names from one list to the other. They wake up disabled until something gets selected.--%>
                <div class="col-xs-12">
                    <div class="row">
                        <div class="col-xs-12">
                            <asp:Button ID="btnAdd" runat="server" Text="< Add" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnAdd_Click" Enabled="false" />
                        </div>
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnRemove" runat="server" Text="Remove >" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnRemove_Click" Enabled="false" />
                        </div>
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnRemoveAll" runat="server" Text="All >>" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnRemoveAll_Click" Enabled="false" />
                        </div>
                        <br />
                        &nbsp;
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnDone_Click" Enabled="true" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--Right column contains search box and list of all Entitys--%>
        <div class="col-xs-4">
            <div class="row">
                <div class="col-xs-12">
                    <h4>All Other Entitys</h4>
                </div>

                <%--Create a search box with a search button for All Entitys. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button--%>
                <div class="col-xs-12">
                    <asp:Panel runat="server" DefaultButton="btnAllEntitySearch">
                        <asp:TextBox ID="txtAllEntity" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                        <asp:Button ID="btnAllEntityHelper" runat="server" Style="display: none" OnClick="btnAllEntitySearch_Click" />
                    </asp:Panel>
                    <asp:LinkButton ID="btnAllEntitySearch" runat="server" CssClass="btn btn-default" OnClick="btnAllEntitySearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>

                <br />
                &nbsp;

            <%--GridView of All Entitys--%>
                <div class="col-xs-12">
                    <asp:Panel ID="pnlAllEntity" runat="server">
                        <!-- Code assumes that EntityID is the first column of this grid -->
                        <asp:GridView ID="AllEntityView" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.Entity"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="25"
                            OnRowDataBound="AllEntityView_RowDataBound"
                            OnSelectedIndexChanged="AllEntityView_SelectedIndexChanged"
                            OnPageIndexChanging="AllEntityView_PageIndexChanging">

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
                                        <td>There are no Entitys that match this search criteria</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="ID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("EntityID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <%--</div>--%>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />

    </div>

</asp:Content>
