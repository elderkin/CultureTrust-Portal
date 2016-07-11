<%@ Page Title="Assign Grant to Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="AssignGrantToProject.aspx.cs" Inherits="Portal11.Admin.AssignGrantToProject" EnableEventValidation="false" %>
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

<%--The Project Grant grid gets a width of 4. The buttons in the middle get a width of 3. The Grant grid gets a width of 4. --%>
        <div class="row">
            <div class="col-xs-6">
                <h4>Selected Project: <asp:Literal ID="litProjectName" runat="server"></asp:Literal></h4>
            </div>
        </div>
       <div class="row">
           <div class="col-xs-4">
               <h4>Project Grants</h4>
           </div>
           <div class=" col-md-offset-3 col-xs-offset-4 col-xs-4">
               <h4>All Grants</h4>
           </div>
       </div>

        <div class="form-group">
            <!-- Create a search box with a search button for Project Grants. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button  -->
            <div class="col-lg-4 col-md-5 col-xs-6">
                <asp:Panel runat="server" DefaultButton="btnProjectGrantSearch">
                    <asp:TextBox ID="txtProjectGrant" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                    <asp:Button ID="btnProjectGrantHelper" runat="server" style="display:none" OnClick="btnProjectGrantSearch_Click" />
                </asp:Panel>
                <asp:LinkButton ID="btnProjectGrantSearch" runat="server" CssClass="btn btn-default" OnClick="btnProjectGrantSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                </asp:LinkButton>
            </div>

            <!-- Create a search box with a search button for All Grants. There's a hidden button that lets an "Enter" in the
                Text Box trigger the search just like the Search button -->
                <div class="col-lg-offset-3 col-lg-4 col-md-offset-2 col-md-5 col-xs-6">
                <asp:Panel runat="server" DefaultButton="btnAllGrantSearch">
                    <asp:TextBox ID="txtAllGrant" runat="server" CssClass="form-control has-success col-xs-4"></asp:TextBox>
                    <asp:Button ID="btnAllGrantHelper" runat="server" style="display:none" OnClick="btnAllGrantSearch_Click" />
                </asp:Panel>
                <asp:LinkButton ID="btnAllGrantSearch" runat="server" CssClass="btn btn-default" OnClick="btnAllGrantSearch_Click">
                    <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                </asp:LinkButton>
                </div>
            </div>

    <div class="row">
        <!-- GridView of Grants assigned to this Project -->
        <asp:Panel ID="pnlProjectGrant" runat="server">
            <div class="col-xs-4">

                <!-- Code assumes that ProjectGrantID is the first column of this grid -->
                <asp:GridView ID="ProjectGrantView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectGrant"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnRowDataBound="ProjectGrantView_RowDataBound"
                    OnSelectedIndexChanged="ProjectGrantView_SelectedIndexChanged"
                    OnPageIndexChanging="ProjectGrantView_PageIndexChanging">

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
                                <td>There are no Grants assigned to this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("ProjectGrantID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Grant.Name" HeaderText="Name" />
                        <asp:BoundField DataField="Grant.Description" HeaderText="Description" ItemStyle-Width="50%" ItemStyle-Wrap="false" />
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

        <asp:Panel ID="pnlAllGrant" runat="server">
                <!-- GridView to list all Grants that match search criteria -->

            <div class="col-xs-offset-1 col-xs-4">

                    <!-- Code assumes that GrantID is the first column of this grid -->
                    <asp:GridView ID="AllGrantView" runat="server"
                        CssClass="table table-striped table-hover"
                        ItemType="Portal11.Models.Grant"
                        AutoGenerateColumns="false"
                        AllowPaging="true" PageSize="25"
                        OnRowDataBound="AllGrantView_RowDataBound"
                        OnSelectedIndexChanged="AllGrantView_SelectedIndexChanged"
                        OnPageIndexChanging="AllGrantView_PageIndexChanging">

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
                                    <td>There are no Grants that match this search criterion</td>
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
                            <asp:BoundField DataField="CurrentFunds" HeaderText="Current Funds" 
                                DataFormatString="${0:###,###,###.00}" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="20"/>
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />


</asp:Content>
