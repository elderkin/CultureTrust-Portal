<%@ Page Title="Staff Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StaffDashboard.aspx.cs"
    Inherits="Portal11.Rqsts.StaffDashboard" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
      .table {
        margin-bottom: 0px;
      }
    </style>

    <h2><%: Title %></h2>
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

        <%--Filters that affect all request types. Pack them in row by row --%>

            <%-- Project Name --%>
            <div class="panel panel-default col-lg-4 col-md-3 col-xs-6" style="margin-bottom: 2px;">
                <h4>Filter: Project Name</h4>
                <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
                    <div class="col-xs-12">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlProjectName" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <%-- Next Reviewer --%>
            <asp:Panel ID="pnlNextReviewer" runat="server" Visible="true">
            <div class="panel panel-default col-lg-4 col-xs-6" style="margin-bottom: 2px;">
                        <h4>Filter: Next Reviewer</h4>
                <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
                    <div class="checkbox" style="padding-top: 0px;">
                        <asp:CheckBox ID="ckRFinanceDirector" runat="server" Text="Finance Director" CssClass="col-xs-7" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRUnsubmitted" runat="server" Text="Unsubmitted" CssClass="col-xs-5" Checked="false"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRInternalCoordinator" runat="server" Text="Internal Coord" CssClass="col-xs-7" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRCompleted" runat="server" Text="Completed" CssClass="col-xs-5" Checked="false"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRTrustDirector" runat="server" Text="Trust Director" CssClass="col-xs-7" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRReturned" runat="server" Text="Returned" CssClass="col-xs-5" Checked="false"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckRTrustExecutive" runat="server" Text="Trust Executive" CssClass="col-xs-7" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    </div>
                </div>
            </div>
            </asp:Panel>

            <%-- Date Range --%>
            <div class="panel panel-default col-lg-4 col-md-3 col-xs-6" style="margin-bottom: 2px;">
                        <h4>Filter: Date Range</h4>

             <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
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

            <%-- Entity (Deposit Entity, Expense Vendor) --%>
            <div class="panel panel-default col-md-4 col-xs-6" style="margin-bottom: 2px;">
                <h4>Filter: Entity Name</h4>
                <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
                    <div class="col-xs-12" title="abcdef">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlEntityName" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="panel panel-default col-md-4 col-xs-6" style="margin-bottom: 2px;">
                <h4>Filter: Person Name</h4>
                <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
                    <div class="col-xs-12">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlPersonName" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <div class="panel panel-default col-md-4 col-xs-6" style="margin-bottom: 2px;">
                <h4>Filter: General Ledger Code</h4>
                <div class="panel-body" style="min-height: 100px; max-height: 100px; padding-top: 0px;">
                    <div class="col-xs-12">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlGLCode" CssClass="form-control"
                            OnSelectedIndexChanged="SearchCriteriaChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

        <%-- Because this is the 7th panel in the row and 7 is prime, this panel always sits by itself. So it doesn't have to be the same height; it can be shorter --%>
            <div class="panel panel-default col-md-4 col-xs-6" style="margin-bottom: 0px;">
                <h4>Filter: Archive Status</h4>
                <div class="panel-body" style="min-height: 70px; max-height: 70px; padding-top: 0px;">
                <div class="checkbox" style="padding-top: 0px;">
                    <asp:CheckBox ID="ckRActive" runat="server" Text="Show Active Requests" CssClass="col-xs-6" Checked="true"
                        OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    <asp:CheckBox ID="ckRArchived" runat="server" Text="Show Archived Requests" CssClass="col-xs-6" Checked="false"
                        OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                </div>
                </div>
            </div>

            </div>
            </div>
            </asp:Panel>
        </div>
        <%--End of panel--%>

        <%--And now a panel for Approvals--%>

        <div class="panel panel-success">
            <div class="panel-heading">
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

                <div class="row">
                    <div class="panel panel-default col-xs-offset-1 col-lg-4 col-md-4 col-sm-5 col-xs-6">
                        Approval Type
                    <div class="checkbox">
                        <asp:CheckBox ID="ckAExpress" runat="server" Text="Express" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckAFull" runat="server" Text="Full Review" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    </div>
                    </div>
                </div>

                <!-- Make a grid of Approvals. Allow Staff to select one for further study. -->
                <div class="row">
                    <div class="col-xs-12">

                        <!-- Code assumes that RowID is the first column of this grid -->
                        <asp:GridView ID="gvStaffApp" runat="server"
                            CssClass="table table-striped table-hover" 
                            ItemType="Portal11.Models.rowStaffApp"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="10"
                            OnRowDataBound="gvStaffApp_RowDataBound"
                            OnPageIndexChanging="gvStaffApp_PageIndexChanging">

                            <SelectedRowStyle CssClass="success" />
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
                                <asp:Label runat="server" >
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    Page <%# gvStaffApp.PageIndex+1 %> of <%# gvStaffApp.PageCount %>
                                </asp:Label>
                            </PagerTemplate>

                            <EmptyDataTemplate>
                                <table>
                                    <tr>
                                        <td>There are no Approval Requests that match these criteria</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="CurrentTime" ItemStyle-HorizontalAlign="Right" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="ProjectName" ItemStyle-HorizontalAlign="Left" HeaderText="Project" HeaderStyle-HorizontalAlign="Right"/>
                                <asp:BoundField DataField="AppTypeDesc" HeaderText="Approval Type" />
                                <asp:BoundField DataField="AppReviewType" HeaderText="Review Type" />
                                <asp:BoundField DataField="CurrentStateDesc" HeaderText="Status" />
                                <asp:BoundField DataField="Owner" HeaderText="Next Reviewer" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnAppGridReview" runat="server" CssClass="btn btn-xs btn-primary" Text="Review"
                                            OnClick="btnAppGridReview_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Row Actions" Visible="false">
                                    <ItemTemplate>
                                        <asp:Button ID="btnAppDblClick" runat="server" Text='<%# Bind("RowID") %>' CssClass="btn btn-default"
                                            OnClick="btnAppDblClick_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Archived" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>' ></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>

<%--                <br />

                <div class="row">
                    <div class="col-xs-12">
                        <asp:Button ID="btnAppReview" runat="server" Text="Review" CssClass="btn btn-default  col-md-1 col-xs-2"
                            Enabled="false" OnClick="btnAppReview_Click" ToolTip="View the selected Approval Request and consider approval" />
                    </div>
                </div>--%>

            </asp:Panel>
        </div>
        <%--End of panel--%>

        <%--And now a panel for Deposits--%>

        <div class="panel panel-success">
            <div class="panel-heading">
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

                <div class="row">
                    <div class="panel panel-default col-xs-offset-1 col-lg-4 col-md-4 col-sm-5 col-xs-6">
                        Deposit Type
                    <div class="checkbox">
                        <asp:CheckBox ID="ckDCheck" runat="server" Text="Check" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDEFT" runat="server" Text="EFT" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDCash" runat="server" Text="Cash" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDInKind" runat="server" Text="In Kind" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckDPledge" runat="server" Text="Pledge/Contract" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    </div>
                    </div>
                </div>

                <!-- Make a grid of Deposits. Allow Staff to select one for further study. -->
                <div class="row">
                    <div class="col-xs-12">

                        <!-- Code assumes that RowID is the first column of this grid -->
                        <asp:GridView ID="gvStaffDep" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.rowStaffDep"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="10"
                            OnRowDataBound="gvStaffDep_RowDataBound"
                            OnPageIndexChanging="gvStaffDep_PageIndexChanging">

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
                                <asp:Label runat="server" >
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    Page <%# gvStaffDep.PageIndex+1 %> of <%# gvStaffDep.PageCount %>
                                </asp:Label>
                            </PagerTemplate>

                            <EmptyDataTemplate>
                                <table>
                                    <tr>
                                        <td>There are no Deposit Requests that match these criteria</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="CurrentTime" ItemStyle-HorizontalAlign="Right" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="ProjectName" HeaderText="Project" />
                                <asp:BoundField DataField="DepTypeDesc" HeaderText="Deposit Type" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="${0:###,###.00}"
                                    HtmlEncode="false" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="CurrentStateDesc" HeaderText="Status" />
                                <asp:BoundField DataField="Owner" HeaderText="Next Reviewer" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnDepGridReview" runat="server" CssClass="btn btn-xs btn-primary" Text="Review"
                                            OnClick="btnDepGridReview_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Archived" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Row Actions" Visible="false">
                                    <ItemTemplate>
                                        <asp:Button ID="btnDepDblClick" runat="server" Text='<%# Bind("RowID") %>' CssClass="btn btn-default"
                                            OnClick="btnDepDblClick_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>

<%--                <br />

                <div class="row">
                    <div class="col-xs-12">
                        <asp:Button ID="btnDepReview" runat="server" Text="Review" CssClass="btn btn-default  col-md-1 col-xs-2"
                            Enabled="false" OnClick="btnDepReview_Click" ToolTip="View the selected Deposit Request and consider approval" />
                    </div>
                </div>--%>

            </asp:Panel>
        </div>
        <%--End of panel--%>

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

                <div class="row">
                    <div class="panel panel-default col-xs-offset-1 col-lg-4 col-md-4 col-sm-5 col-xs-6">
                        Expense Type
                    <div class="checkbox">
                        <asp:CheckBox ID="ckEPayroll" runat="server" Text="Payroll" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEVendorInvoice" runat="server" Text="Vendor Invoice" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEContractorInvoice" runat="server" Text="Contractor Invoice" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEPurchaseOrder" runat="server" Text="Purchase Order" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEReimbursement" runat="server" Text="Reimbursement" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                        <asp:CheckBox ID="ckEPEXCard" runat="server" Text="PEX Card" CssClass="col-xs-6" Checked="true"
                            OnCheckedChanged="SearchCriteriaChanged" AutoPostBack="true" />
                    </div>
                    </div>
                </div>

                <!-- Make a grid of Exp. Allow Staff to select one for further study. -->
                <div class="row">
                    <div class="col-xs-12">

                        <!-- Code assumes that RowID is the first column of this grid -->
                        <asp:GridView ID="gvStaffExp" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.rowStaffExp"
                            AutoGenerateColumns="false"
                            AllowPaging="true" PageSize="10"
                            OnRowDataBound="gvStaffExp_RowDataBound"
                            OnPageIndexChanging="gvStaffExp_PageIndexChanging">

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
                                <asp:Label runat="server" >
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    Page <%# gvStaffExp.PageIndex+1 %> of <%# gvStaffExp.PageCount %>
                                </asp:Label>
                            </PagerTemplate>

                            <EmptyDataTemplate>
                                <table>
                                    <tr>
                                        <td>There are no Expense Requests that match these criteria</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:BoundField DataField="CurrentTime" ItemStyle-HorizontalAlign="Right" HeaderText="Last Modified" DataFormatString="{0:MM/dd/yyyy}" />
                                <asp:BoundField DataField="ProjectName" HeaderText="Project" />
                                <asp:BoundField DataField="ExpTypeDesc" HeaderText="Expense Type" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="${0:###,###.00}"
                                    HtmlEncode="false" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="CurrentStateDesc" HeaderText="Status" />
                                <asp:BoundField DataField="Owner" HeaderText="Next Reviewer" />
                                <asp:BoundField DataField="Target" HeaderText="Destination" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="btnExpGridReview" runat="server" CssClass="btn btn-xs btn-primary" Text="Review"
                                            OnClick="btnExpGridReview_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCurrentState" runat="server" Text='<%# Bind("CurrentState") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Archived" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblArchived" runat="server" Text='<%# Bind("Archived") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rush" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRush" runat="server" Text='<%# Bind("Rush") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Row Actions" Visible="false">
                                    <ItemTemplate>
                                        <asp:Button ID="btnExpDblClick" runat="server" Text='<%# Bind("RowID") %>' CssClass="btn btn-default"
                                            OnClick="btnExpDblClick_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>

<%--                <br />

                <div class="row">
                    <div class="col-xs-12">
                        <asp:Button ID="btnExpReview" runat="server" Text="Review" CssClass="btn btn-default  col-md-1 col-xs-2"
                            Enabled="false" OnClick="btnExpReview_Click" ToolTip="View the selected Expense Request and consider approval" />
                    </div>
                </div>--%>

            </asp:Panel>
        </div>
        <%--End of panel--%>
    </div>
    <%--End of form-horizontal--%>
    <!-- "Scratch" storage used during form processing -->
    <asp:Literal ID="litSavedUserRole" runat="server" Visible="false" />
    <asp:Literal ID="litSelectedAppRow" runat="server" Visible="false" />
    <asp:Literal ID="litSelectedDepRow" runat="server" Visible="false" />
    <asp:Literal ID="litSelectedExpRow" runat="server" Visible="false" />

</asp:Content>
