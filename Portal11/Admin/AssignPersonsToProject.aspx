<%@ Page Title="Assign Persons To Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssignPersonsToProject.aspx.cs" 
    Inherits="Portal11.Admin.AssignPersonsToProject" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %> <asp:Literal runat="server" ID="litProjectName" /></h2>
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

        <%--The Project Person grid gets a width of 4. The Person Role buttons in the middle get a width of 4. The Person grid gets a width of 4.--%>
        <%--Left column contains Project Persons, a search box, and a grid view to list project persons--%>
        <div class="col-xs-4">
            <div class="row">
                <div class="col-xs-12">
                    <h4>Persons on Project in Role</h4>
                </div>

                <%--Create a search box with a search button for Project Persons. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button.--%>
                <div class="col-xs-12">
                    <asp:Panel runat="server" DefaultButton="btnProjectPersonSearch">
                        <div class="col-xs-9">
                            <asp:TextBox ID="txtProjectPerson" runat="server" CssClass="form-control has-success"></asp:TextBox>
                            <asp:Button ID="btnProjectPersonHelper" runat="server" Style="display: none" OnClick="btnProjectPersonSearch_Click" />
                        </div>
                    </asp:Panel>
                    <asp:LinkButton ID="btnProjectPersonSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectPersonSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>
                <br />
                &nbsp;


                <%--GridView of Persons assigned to this Project--%>
                <div class="col-xs-12">
                    <asp:Panel ID="pnlProjectPerson" runat="server">

                        <!-- Code assumes that ProjectPersonID is the first column of this grid -->
                        <asp:GridView ID="gvProjectPerson" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.ProjectPerson"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="25"
                            OnRowDataBound="gvProjectPerson_RowDataBound"
                            OnSelectedIndexChanged="gvProjectPerson_SelectedIndexChanged"
                            OnPageIndexChanging="gvProjectPerson_PageIndexChanging">

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
                                        <td>There are no Persons assigned to this Project in the selected Role</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="ID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectPersonID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Person.Name" HeaderText="Name" />
                                <asp:BoundField DataField="Person.Email" HeaderText="Email" />
                                <asp:BoundField DataField="Person.W9Present" HeaderText="W-9 Present" />
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
                    <h4>Person Role</h4>
                </div>

                <%--Radio buttons to select the role of person to be processed--%>
                <div class="col-xs-12">
                    <div class="panel panel-default">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoPersonRole" runat="server" AutoPostBack="true"
                                Style="margin-left: 40px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                
                                OnSelectedIndexChanged="rdoPersonRole_SelectedIndexChanged">
                                <asp:ListItem Text="Contractor (for Contractor Invoice)" Value="Contractor" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Donor/Customer (for Deposit)" Value="Donor"></asp:ListItem>
                                <asp:ListItem Text="Employee (for Payroll)" Value="Employee"></asp:ListItem>
                                <asp:ListItem Text="Recipient (for Reimbursement)" Value="Recipient"></asp:ListItem>
                                <asp:ListItem Text="Responsible Person (for Gift Card)" Value="ResponsiblePerson"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <%--Buttons to move names from one list to the other. They wake up disabled until something gets selected.--%>
                <div class="col-xs-12">
                    <div class="row">
                        <div class="col-xs-12">
                            <asp:Button ID="btnAdd" runat="server" Text="< Add" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnAdd_Click" Enabled="false" ToolTip="Assign selected person to project in role" />
                        </div>
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnRemove" runat="server" Text="Remove >" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnRemove_Click" Enabled="false" ToolTip="Remove selected person from project" />
                        </div>
<%--                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnRemoveAll" runat="server" Text="All >>" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnRemoveAll_Click" Enabled="false" ToolTip="Assign all persons to project in role" />
                        </div>--%>
                        <br />
                        &nbsp;
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnNew" runat="server" Text="New" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnNew_Click" Enabled="true" ToolTip="Create a new person" />
                        </div>
                        <br />
                        &nbsp;
                        <br />
                        &nbsp;
                        <div class="col-xs-12">
                            <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn btn-default col-md-offset-4 col-md-4 col-xs-offset-3 col-xs-6"
                                OnClick="btnDone_Click" Enabled="true" ToolTip="Exit with no further changes" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%--Right column contains search box and list of all persons--%>
        <div class="col-xs-4">
            <div class="row">
                <div class="col-xs-12">
                    <h4>All Other Persons</h4>
                </div>

                <%--Create a search box with a search button for All Persons. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button--%>
                <div class="col-xs-12">
                    <asp:Panel runat="server" DefaultButton="btnAllPersonSearch">
                        <div class="col-xs-9">
                            <asp:TextBox ID="txtAllPerson" runat="server" CssClass="form-control has-success"></asp:TextBox>
                            <asp:Button ID="btnAllPersonHelper" runat="server" Style="display: none" OnClick="btnAllPersonSearch_Click" />
                        </div>
                    </asp:Panel>
                    <asp:LinkButton ID="btnAllPersonSearch" runat="server" CssClass="btn btn-default" OnClick="btnAllPersonSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>

                <br />
                &nbsp;

            <%--GridView of All Persons--%>
                <div class="col-xs-12">
                    <asp:Panel ID="pnlAllPerson" runat="server">
                        <!-- Code assumes that PersonID is the first column of this grid -->
                        <asp:GridView ID="gvAllPerson" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.Person"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="25"
                            OnRowDataBound="gvAllPerson_RowDataBound"
                            OnSelectedIndexChanged="gvAllPerson_SelectedIndexChanged"
                            OnPageIndexChanging="gvAllPerson_PageIndexChanging">

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
                                        <td>Please enter a name above to find a Person within our database</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="ID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("PersonID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="Email" HeaderText="Email" />
                                <asp:BoundField DataField="W9Present" HeaderText="W-9 Present" />
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <%--</div>--%>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />

    </div>

</asp:Content>
