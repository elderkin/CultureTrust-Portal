<%@ Page Title="Assign Employees to Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AssignEmployees.aspx.cs" Inherits="Portal11.Rqsts.AssignEmployees" EnableEventValidation="false" %>

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

    <!-- The Project Employee grid gets a width of 4. The buttons in the middle get a width of 3. The Employee grid gets a width of 4. -->
       <div class="row">
           <div class="col-xs-4">
               <h4>Project Employees</h4>
           </div>
           <div class=" col-md-offset-3 col-xs-offset-4 col-xs-4">
               <h4>All Employees</h4>
           </div>
       </div>

        <div class="form-group">
            <!-- Create a search box with a search button for Project Employees. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button  -->
            <div class="col-lg-4 col-md-5 col-xs-6">
                <asp:Panel runat="server" DefaultButton="btnProjectEmployeeSearch">
                    <asp:TextBox ID="txtProjectEmployee" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                    <asp:Button ID="btnProjectEmployeeHelper" runat="server" style="display:none" OnClick="btnProjectEmployeeSearch_Click" />
                </asp:Panel>
                <asp:LinkButton ID="btnProjectEmployeeSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectEmployeeSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                </asp:LinkButton>
                </div>

            <!-- Create a search box with a search button for All Employees. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button -->
            <div class="col-lg-offset-3 col-md-offset-2 col-md-5 col-xs-6">
                <asp:Panel runat="server" DefaultButton="btnAllEmployeeSearch">
                    <asp:TextBox ID="txtAllEmployee" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                    <asp:Button ID="btnAllEmployeeHelper" runat="server" style="display:none" OnClick="btnAllEmployeeSearch_Click" />
                </asp:Panel>
                <asp:LinkButton ID="btnAllEmployeeSearch" runat="server" CssClass="btn btn-default" OnClick="btnAllEmployeeSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                </asp:LinkButton>

            </div>
        </div>

    <div class="row">
        <!-- GridView of Employees assigned to this Project -->
        <asp:Panel ID="pnlProjectEmployee" runat="server">
            <div class="col-xs-4">

                <!-- Code assumes that ProjectEmployeeID is the first column of this grid -->
                <asp:GridView ID="ProjectEmployeeView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectEmployee"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnRowDataBound="ProjectEmployeeView_RowDataBound"
                    OnSelectedIndexChanged="ProjectEmployeeView_SelectedIndexChanged"
                    OnPageIndexChanging="ProjectEmployeeView_PageIndexChanging">

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
                                <td>There are no Employees assigned to this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectEmployeeID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Employee.Name" HeaderText="Name" />
                        <asp:BoundField DataField="Employee.Email" HeaderText="Email" />
                    </Columns>
                </asp:GridView>

            </div>
            </asp:Panel>

        <!-- Buttons to move names from one list to the other. They wake up disabled until something gets selected. -->
            <div class="col-md-offset-1 col-md-1 col-xs-offset-1 col-xs-2">
                <div class="row">
                    <asp:Button ID="btnAddx" runat="server" Text="< Add" CssClass="btn btn-default col-xs-12" 
                        OnClick="btnAdd_Click" Enabled="false" />
                    <br /> &nbsp;
                    <asp:Button ID="btnRemovex" runat="server" Text="Remove >" CssClass="btn btn-default col-xs-12" 
                        OnClick="btnRemove_Click" Enabled="false" />
                    <br /> &nbsp;
                    <asp:Button ID="btnRemoveAllx" runat="server" Text="All >>" CssClass="btn btn-default col-xs-12" 
                        OnClick="btnRemoveAll_Click" Enabled="false" />
                    <br /> &nbsp; <br /> &nbsp;
                    <asp:Button ID="btnDone" runat="server" Text="Done" CssClass="btn btn-default col-xs-12" 
                        OnClick="btnDone_Click" Enabled="true" />
                </div>
            </div>

        <asp:Panel ID="pnlAllEmployee" runat="server">
                <!-- GridView to list all Employees that match search criteria -->

            <div class="col-xs-offset-1 col-xs-4">

                    <!-- Code assumes that EmployeeID is the first column of this grid -->
                    <asp:GridView ID="AllEmployeeView" runat="server"
                        CssClass="table table-striped table-hover"
                        ItemType="Portal11.Models.Employee"
                        AutoGenerateColumns="false"
                        AllowPaging="true" PageSize="25"
                        OnRowDataBound="AllEmployeeView_RowDataBound"
                        OnSelectedIndexChanged="AllEmployeeView_SelectedIndexChanged"
                        OnPageIndexChanging="AllEmployeeView_PageIndexChanging">

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
                                    <td>There are no Employees that match this search criteria</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="ID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("EmployeeID") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />

    </div>
</asp:Content>
