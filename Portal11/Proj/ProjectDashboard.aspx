<%@ Page Title="Project Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectDashboard.aspx.cs"
    Inherits="Portal11.Proj.ProjectDashboard" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
      .col-md-offset-15 {
        margin-left: 6.92%;
  }
    </style>
    <br />
    <div class="row">
        <div class="col-md-4 col-xs-6">
            <h2><%: Title %></h2>
        </div>
        <div class="text-right col-md-offset-4 col-md-4 col-xs-6">
            <div class="col-xs-12">
                <h4>Balance as of:&nbsp;<asp:Literal ID="litBalance" runat="server"></asp:Literal></h4>
            </div>
            <div class="col-xs-12">
                <h4>Current Funds:&nbsp;<asp:Literal ID="litCurrentFunds" runat="server" >$xxx,xxx.xx</asp:Literal></h4>
            </div>
        </div>
    </div>

    <hr />
    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">

        <%--A panel for Search Criteria--%>

        <div class="panel panel-success">
            <div class="panel-heading">
                <asp:LinkButton ID="btnSearchCollapse" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnSearchCollapse_Click" Visible="false"
                    Text="<i aria-hidden='true' class='glyphicon glyphicon-chevron-up'></i>">
                </asp:LinkButton>
                <asp:LinkButton ID="btnSearchExpand" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnSearchExpand_Click">
                    <span class="glyphicon glyphicon-chevron-down"></span>
                </asp:LinkButton>
                <strong>Search Filters</strong>
            </div>
            <asp:Panel ID="pnlSearch" runat="server" Visible="false">
                <br />

            <div class="row">
            <div class="col-xs-12">

            <%--Request Status--%>

            <div class="panel panel-default col-md-3 col-xs-6">
                <div class="panel-body" style="min-height: 108px; max-height: 108px;">
                <h4>Filter: Request Status</h4>
                    <div class="checkbox">
                        <asp:CheckBox ID="ckRAwaitingProjectStaff" runat="server" Text="Awaiting Project Staff" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRAwaitingCWStaff" runat="server" Text="Awaiting CW Staff" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRApproved" runat="server" Text="Approved/Complete/Paid" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRReturned" runat="server" Text="Returned" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    </div>
                    </div>
            </div>

            <%--Date Range--%>
            <div class="panel panel-default col-md-offset-1 col-md-3 col-xs-6">
                <h4>Filter: Date Range</h4>

             <div class="panel-body" style="min-height: 112px; max-height: 112px;">
                 <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtBeginningDate" 
                        CssClass="col-xs-2 control-label">From</asp:Label>
                    <div class="col-xs-10">
                        <div class="input-group">

                            <asp:TextBox runat="server" ID="txtBeginningDate" CssClass="form-control has-success" 
                                OnTextChanged="txtBeginningDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <span class="input-group-addon">
                                <asp:LinkButton runat="server" ID="btnBeginningDate" CssClass="btn-xs btn-default"
                                    OnClick="btnBeginningDate_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                </asp:LinkButton>
                            </span>
                        </div>

                        <asp:Calendar ID="calBeginningDate" runat="server" Visible="false"
                            OnSelectionChanged="calBeginningDate_SelectionChanged"
                            DayNameFormat="Short"
                            SelectionMode="DayWeekMonth"
                            Font-Names="Verdana" Font-Size="12px"
                            Height="180px" Width="275px"
                            TodayDayStyle-Font-Bold="True"
                            DayHeaderStyle-Font-Bold="True"
                            OtherMonthDayStyle-ForeColor="gray"
                            TitleStyle-BackColor="#18bc9c"
                            TitleStyle-ForeColor="white"
                            TitleStyle-Font-Bold="True"
                            SelectedDayStyle-BackColor="#f39c12"
                            SelectedDayStyle-Font-Bold="True"
                            NextPrevFormat="ShortMonth"
                            NextPrevStyle-ForeColor="white"
                            NextPrevStyle-Font-Size="10px"
                            SelectorStyle-BackColor="#3498db"
                            SelectorStyle-ForeColor="white"
                            SelectorStyle-Font-Size="10px"
                            SelectWeekText="week"
                            SelectMonthText="month" />
                    </div>

                    <asp:Label runat="server" AssociatedControlID="txtEndingDate" 
                        CssClass="col-xs-2 control-label">To</asp:Label>
                    <div class="col-xs-10">

                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtEndingDate" CssClass="form-control has-success" 
                                OnTextChanged="txtEndingDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <span class="input-group-addon">
                                <asp:LinkButton runat="server" ID="btnEndingDate" CssClass="btn-xs btn-default"
                                    OnClick="btnEndingDate_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                </asp:LinkButton>
                            </span>
                        </div>

                        <asp:Calendar ID="calEndingDate" runat="server" Visible="false"
                            OnSelectionChanged="calEndingDate_SelectionChanged"
                            DayNameFormat="Short"
                            SelectionMode="Day"
                            Font-Names="Verdana" Font-Size="12px"
                            Height="180px" Width="275px"
                            TodayDayStyle-Font-Bold="True"
                            DayHeaderStyle-Font-Bold="True"
                            OtherMonthDayStyle-ForeColor="gray"
                            TitleStyle-BackColor="#18bc9c"
                            TitleStyle-ForeColor="white"
                            TitleStyle-Font-Bold="True"
                            SelectedDayStyle-BackColor="#f39c12"
                            SelectedDayStyle-Font-Bold="True"
                            NextPrevFormat="ShortMonth"
                            NextPrevStyle-ForeColor="white"
                            NextPrevStyle-Font-Size="10px"
                            SelectorStyle-BackColor="#3498db"
                            SelectorStyle-ForeColor="white"
                            SelectorStyle-Font-Size="10px"
                            SelectWeekText="week" 
                            SelectMonthText="month" />
                    </div>
                    </div>
                </div>
                </div>

        <%-- Archived --%>

            <div class="panel panel-default col-md-offset-1 col-md-4 col-xs-6">
                <h4>Filter: Archive Status</h4>
                <div class="panel-body" style="min-height: 108px; max-height: 108px;">
                <div class="checkbox">
                    <asp:CheckBox ID="ckRActive" runat="server" Text="Show Active Requests" CssClass="col-xs-6" Checked="true"
                        OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    <asp:CheckBox ID="ckRArchived" runat="server" Text="Show Archived Requests" CssClass="col-xs-6" Checked="false"
                        OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                </div>
                </div>
            </div>

            <%-- Entity (Deposit Entity, Expense Vendor) --%>

            <div class="col-xs-12">
            <div class="panel panel-default col-xs-offset-0 col-md-3 col-xs-6">
                <h4>Filter: Entity Name</h4>
                <div class="panel-body" style="min-height: 108px; max-height: 108px;">
                    <div class="col-xs-12" title="abcdef">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlEntityName" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="panel panel-default col-md-offset-1 col-md-3 col-xs-6">
                <h4>Filter: Person Name</h4>
                <div class="panel-body" style="min-height: 108px; max-height: 108px;">
                    <div class="col-xs-12">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlPersonName" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            </div>

            </div>
            </div>
            </asp:Panel>


        </div>

        <div class="panel panel-success">
            <div class="panel-heading ">
                <asp:LinkButton ID="btnAppCollapse" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnAppCollapse_Click"
                    Text="<i aria-hidden='true' class='glyphicon glyphicon-chevron-up'></i>">
                </asp:LinkButton>
                <asp:LinkButton ID="btnAppExpand" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnAppExpand_Click" Visible="false">
                    <span class="glyphicon glyphicon-chevron-down"></span>
                </asp:LinkButton>
                <strong>Approval Requests</strong>
            </div>
            <asp:Panel ID="pnlApp" runat="server">
            <br />

<%--        <div class="form-group">
            <div class="row">

                <div class="panel panel-default col-xs-offset-1 col-md-4 col-xs-5">
                    Filters
                    <div class="checkbox">
                        <asp:CheckBox ID="ckAAwaitingProjectStaff" runat="server" Text="Awaiting Project Manager" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckAFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckAAwaitingCWStaff" runat="server" Text="Awaiting CW Staff" CssClass="col-xs-12"
                            OnCheckedChanged="ckAFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckAApproved" runat="server" Text="Approved" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckAFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckAReturned" runat="server" Text="Returned" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckAFilters_CheckedChanged" AutoPostBack="true" />
                    </div>
                </div>

                <div class="col-md-offset-2 col-md-4 col-xs-5">
                    Since
                    <asp:DropDownList ID="ddlASince" runat="server" AutoPostBack="true"
                        CssClass="form-control"
                        OnSelectedIndexChanged="ddlASince_SelectedIndexChanged">
                        <asp:ListItem Text="Project Creation" Value="ProjectCreation" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Balance Date" Value="BalanceDate"></asp:ListItem>
                        <asp:ListItem Text="Last 30 days" Value="Last30"></asp:ListItem>
                        <asp:ListItem Text="Last 90 days" Value="Last90"></asp:ListItem>
                        <asp:ListItem Text="Last 12 months" Value="Last12"></asp:ListItem>

                    </asp:DropDownList>
                </div>
            </div>
        </div>--%>

        <!-- Make a grid of Approval Requests. Allow User to select one for further study. -->
        <div class="row">
            <div class="col-xs-12">

                <!-- Code assumes that RowID is the first column of this grid -->
                <asp:GridView ID="AllAppView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectAppViewRow"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnRowDataBound="AllAppView_RowDataBound"
                    OnSelectedIndexChanged="AllAppView_SelectedIndexChanged"
                    OnPageIndexChanging="AllAppView_PageIndexChanging">

                    <SelectedRowStyle CssClass="success" />

                    <HeaderStyle HorizontalAlign="Right" />

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
                                <td>There are no matching Approval Requests for this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("RowID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CurrentTime" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="AppTypeDesc" HeaderText="Approval Type" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:TemplateField HeaderText="Status" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentStateDesc" runat="server" Text='<%# Bind("CurrentStateDesc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Archived" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        <br />

            <div class="col-xs-12">
            <asp:Button ID="btnAppNew" runat="server" Text="New" CssClass="btn btn-default col-md-1 col-xs-2"
                Enabled="true" OnClick="btnAppNew_Click" ToolTip="Create a new Request" />
            <asp:Button ID="btnAppEdit" runat="server" Text="Edit" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnAppEdit_Click" ToolTip="Make changes to the selected Request" />
            <asp:Button ID="btnAppView" runat="server" Text="View" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnAppView_Click" ToolTip="View the selected Request without making changes" />
            <asp:Button ID="btnAppCopy" runat="server" Text="Copy" CssClass="btn btn-default col-md-offset-15 col-md-1 col-xs-2"
                Enabled="false" OnClick="btnAppCopy_Click" ToolTip="Copy the selected Request to create a new Request and edit it" />
            <asp:Button ID="btnAppDelete" runat="server" Text="Delete" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnAppDelete_Click" ToolTip="Delete the selected Request" />
            <asp:Button ID="btnAppReview" runat="server" Text="Review" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnAppReview_Click" ToolTip="Review and Approve the selected Request" />
            <asp:Button ID="btnAppArchive" runat="server" Text="Archive" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnAppArchive_Click" ToolTip="Move the selected Request to the Archive" />
            </div>
        </div>

        </asp:Panel>
        </div>

        <div class="panel panel-success">
            <div class="panel-heading ">
                <asp:LinkButton ID="btnDepCollapse" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnDepCollapse_Click"
                    Text="<i aria-hidden='true' class='glyphicon glyphicon-chevron-up'></i>">
                </asp:LinkButton>
                <asp:LinkButton ID="btnDepExpand" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnDepExpand_Click" Visible="false">
                    <span class="glyphicon glyphicon-chevron-down"></span>
                </asp:LinkButton>
                <strong>Deposit Requests</strong>
            </div>
            <asp:Panel ID="pnlDep" runat="server">
            <br />

<%--        <div class="form-group">
            <div class="row">

                <div class="panel panel-default col-xs-offset-1 col-md-4 col-xs-5">
                    Filters
                    <div class="checkbox">
                        <asp:CheckBox ID="ckDAwaitingProjectStaff" runat="server" Text="Awaiting Project Manager" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckDFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDAwaitingCWStaff" runat="server" Text="Awaiting CW Staff" CssClass="col-xs-12"
                            OnCheckedChanged="ckDFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDDepositComplete" runat="server" Text="Deposit Complete" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckDFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDReturned" runat="server" Text="Returned" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckDFilters_CheckedChanged" AutoPostBack="true" />
                    </div>
                </div>

                <div class="col-md-offset-2 col-md-4 col-xs-5">
                    Since
                    <asp:DropDownList ID="ddlDSince" runat="server" AutoPostBack="true"
                        CssClass="form-control"
                        OnSelectedIndexChanged="ddlDSince_SelectedIndexChanged">
                        <asp:ListItem Text="Project Creation" Value="ProjectCreation" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Balance Date" Value="BalanceDate"></asp:ListItem>
                        <asp:ListItem Text="Last 30 days" Value="Last30"></asp:ListItem>
                        <asp:ListItem Text="Last 90 days" Value="Last90"></asp:ListItem>
                        <asp:ListItem Text="Last 12 months" Value="Last12"></asp:ListItem>

                    </asp:DropDownList>
                </div>
            </div>
        </div>--%>

        <!-- Make a grid of Deposit Requests. Allow User to select one for further study. -->
        <div class="row">
            <div class="col-xs-12">

                <!-- Code assumes that RowID is the first column of this grid -->
                <asp:GridView ID="AllDepView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectDepViewRow"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnRowDataBound="AllDepView_RowDataBound"
                    OnSelectedIndexChanged="AllDepView_SelectedIndexChanged"
                    OnPageIndexChanging="AllDepView_PageIndexChanging">

                    <SelectedRowStyle CssClass="success" />

                    <HeaderStyle HorizontalAlign="Right" />

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
                                <td>There are no matching Deposit Requests for this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("RowID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CurrentTime" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="DepTypeDesc" HeaderText="Deposit Type" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="SourceOfFunds" HeaderText="Source of Funds" />
                        <asp:BoundField DataField="Amount" HeaderText="Amount" 
                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Status" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentStateDesc" runat="server" Text='<%# Bind("CurrentStateDesc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Archived" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        <br />

            <div class="col-xs-12">
            <asp:Button ID="btnDepNew" runat="server" Text="New" CssClass="btn btn-default col-md-1 col-xs-2"
                Enabled="false" OnClick="btnDepNew_Click" ToolTip="Create a new Request" />
            <asp:Button ID="btnDepEdit" runat="server" Text="Edit" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnDepEdit_Click" ToolTip="Make changes to the selected Request" />
            <asp:Button ID="btnDepView" runat="server" Text="View" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnDepView_Click" ToolTip="View the selected Request without making changes" />
            <asp:Button ID="btnDepCopy" runat="server" Text="Copy" CssClass="btn btn-default col-md-offset-15 col-md-1 col-xs-2"
                Enabled="false" OnClick="btnDepCopy_Click" ToolTip="Copy the selected Request to create a new Request and edit it" />
            <asp:Button ID="btnDepDelete" runat="server" Text="Delete" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnDepDelete_Click" ToolTip="Delete the selected Request" />
            <asp:Button ID="btnDepReview" runat="server" Text="Review" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnDepReview_Click" ToolTip="Review and Approve the selected Request" />
            <asp:Button ID="btnDepArchive" runat="server" Text="Archive" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnDepArchive_Click" ToolTip="Move the selected Request to the Archive" />
            </div>
        </div>

        </asp:Panel>
        </div>

        <div class="panel panel-success">
            <div class="panel-heading">
                <asp:LinkButton ID="btnExpCollapse" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnExpCollapse_Click"
                    Text="<i aria-hidden='true' class='glyphicon glyphicon-chevron-up'></i>">
                </asp:LinkButton>
                <asp:LinkButton ID="btnExpExpand" runat="server" CssClass="btn btn-default btn-xs" OnClick="btnExpExpand_Click" Visible="false">
                    <span class="glyphicon glyphicon-chevron-down"></span>
                </asp:LinkButton>
                <strong>Expense Requests</strong>
            </div>
            <asp:Panel ID="pnlExp" runat="server">
            <br />

<%--        <div class="form-group">
            <div class="row">
                <div class="panel panel-default col-xs-offset-1 col-md-4 col-xs-5">
                    Filters
                    <div class="checkbox">
                        <asp:CheckBox ID="ckEAwaitingProjectStaff" runat="server" Text="Awaiting Project Staff" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckEFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEAwaitingCWStaff" runat="server" Text="Awaiting CW Staff" CssClass="col-xs-12"
                            OnCheckedChanged="ckEFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEPaid" runat="server" Text="Paid" CssClass="col-xs-12"
                            OnCheckedChanged="ckEFilters_CheckedChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEReturned" runat="server" Text="Returned" CssClass="col-xs-12" Checked="true"
                            OnCheckedChanged="ckEFilters_CheckedChanged" AutoPostBack="true" />
                    </div>
                </div>

                <div class="col-md-offset-2 col-md-4 col-xs-5">
                    Since
                    <asp:DropDownList ID="ddlESince" runat="server" AutoPostBack="true"
                        CssClass="form-control"
                        OnSelectedIndexChanged="ddlESince_SelectedIndexChanged">
                        <asp:ListItem Text="Project Creation" Value="ProjectCreation" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Balance Date" Value="BalanceDate"></asp:ListItem>
                        <asp:ListItem Text="Last 30 days" Value="Last30"></asp:ListItem>
                        <asp:ListItem Text="Last 90 days" Value="Last90"></asp:ListItem>
                        <asp:ListItem Text="Last 12 months" Value="Last12"></asp:ListItem>

                    </asp:DropDownList>
                </div>
            </div>
        </div>--%>

        <!-- Make a grid of equests. Allow User to select one for further study. -->
        <div class="row">
            <div class="col-xs-12">

                <!-- Code assumes that RowID is the first column of this grid -->
                <asp:GridView ID="AllExpView" runat="server"
                    CssClass="table table-striped table-hover"
                    ItemType="Portal11.Models.ProjectExpViewRow"
                    AutoGenerateColumns="false"
                    AllowPaging="true" PageSize="25"
                    OnRowDataBound="AllExpView_RowDataBound"
                    OnSelectedIndexChanged="AllExpView_SelectedIndexChanged"
                    OnPageIndexChanging="AllExpView_PageIndexChanging">

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
                                <td>There are no matching Expense Requests for this Project</td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="ID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblRowID" runat="server" Text='<%# Bind("RowID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CurrentTime" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="ExpTypeDesc" HeaderText="Expense Type" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="Target" HeaderText="Destination" />
                        <asp:BoundField DataField="Amount" HeaderText="Amount" 
                            ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Status" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblCurrentStateDesc" runat="server" Text='<%# Bind("CurrentStateDesc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Archived" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        <br />

            <div class="col-xs-12">
            <asp:Button ID="btnExpNew" runat="server" Text="New" CssClass="btn btn-default col-md-1 col-xs-2"
                Enabled="true" OnClick="btnExpNew_Click" ToolTip="Create a new Request" />
            <asp:Button ID="btnExpEdit" runat="server" Text="Edit" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnExpEdit_Click" ToolTip="Make changes to the selected Request" />
            <asp:Button ID="btnExpView" runat="server" Text="View" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnExpView_Click" ToolTip="View the selected Request without making changes" />
            <asp:Button ID="btnExpCopy" runat="server" Text="Copy" CssClass="btn btn-default col-md-offset-15 col-md-1 col-xs-2"
                Enabled="false" OnClick="btnExpCopy_Click" ToolTip="Copy the selected Request to create a new Request and edit it" />
            <asp:Button ID="btnExpDelete" runat="server" Text="Delete" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnExpDelete_Click" ToolTip="Delete the selected Request" />
            <asp:Button ID="btnExpReview" runat="server" Text="Review" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnExpReview_Click" ToolTip="Review and Approve the selected Request" />
            <asp:Button ID="btnExpArchive" runat="server" Text="Archive" CssClass="btn btn-default col-md-offset-15 col-md-1  col-xs-2"
                Enabled="false" OnClick="btnExpArchive_Click" ToolTip="Move the selected Request to the Archive" />
            </div>
        </div>

        </asp:Panel>
        </div>
        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectRole" runat="server" Visible="false" />

    </div>
</asp:Content>
