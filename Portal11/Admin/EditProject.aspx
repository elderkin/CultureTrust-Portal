<%@ Page Title="Edit Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditProject.aspx.cs" 
    Inherits="Portal11.Admin.EditProject" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>

        <hr />
        <p class="text-danger">
            <asp:Literal runat="server" ID="litDangerMessage" />
        </p>
        <p class="text-success">
            <asp:Literal runat="server" ID="litSuccessMessage" />
        </p>
    <div class="form-horizontal">
    <asp:Panel runat="server" DefaultButton="btnSave">

        <asp:Panel ID="pnlName" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtName" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="true">Project Name</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                            CssClass="text-danger" ErrorMessage="Please supply a Project Name." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlInactive" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkInact" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">Project is Inactive</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkInact" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Turn the Project off, disabling its use. Note: This action is difficult to undo!" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlCode" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtCode" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="true">Project Code</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtCode" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode"
                            CssClass="text-danger" ErrorMessage="Please supply a Project Code." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCode" ValidationExpression="[\S\d]{3}"
                            CssClass="text-danger" ErrorMessage="Project Code must be exactly three characters in length." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDescription" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Description</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Balance Date -->
        <asp:Panel ID="pnlBalanceDate" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label ID="lblBalanceDate" runat="server" AssociatedControlID="txtBalanceDate" Font-Bold="false"
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Balance Date</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtBalanceDate" CssClass="form-control has-success"
                                 OnTextChanged="txtBalanceDate_TextChanged" AutoPostBack="true" ></asp:TextBox>
                            <span class="input-group-addon">
                                <asp:LinkButton runat="server" ID="btnBalanceDate" CssClass="btn-xs btn-default"
                                    OnClick="btnBalanceDate_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                </asp:LinkButton>
                            </span>
                        </div>

                        <asp:Calendar ID="calBalanceDate" runat="server" Visible="false"
                            OnSelectionChanged="calBalanceDate_SelectionChanged"
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
<%--                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBalanceDate"
                            CssClass="text-danger" ErrorMessage="Please supply a Date." />--%>
                        <asp:RegularExpressionValidator ID="valBalanceDate" runat="server" ControlToValidate="txtBalanceDate"
                            CssClass="text-danger" ErrorMessage="Date format is dd/mm/yyyy"
                            ValidationExpression="^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$">
                        </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Current Funds -->
        <asp:Panel ID="pnlCurrentFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtCurrentFunds" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="true">Current Funds</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtCurrentFunds" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCurrentFunds"
                            CssClass="text-danger" ErrorMessage="Please supply a Dollar Amount value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCurrentFunds"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
                    </div>
                </div>
            </div>
        </asp:Panel>

<%--        <!-- Restricted Grants -->

        <asp:Panel ID="pnlRestrictedGrants" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Restricted Grants</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:ListBox ID="lstRestrictedGrants" runat="server" CssClass="form-control" CausesValidation="false" Rows="3" Enabled="false" />
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Total Grants</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtTotalGrants" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>

        </asp:Panel>--%>

<%--        <asp:Panel ID="pnlRestrictedGrants" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Restricted Grants</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtRestrictedGrants" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRestrictedGrants"
                            CssClass="text-danger" ErrorMessage="Please supply a Dollar Amount value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtRestrictedGrants"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
                    </div>
                </div>
            </div>
        </asp:Panel>--%>

        <!-- Project Director Name -->
        <asp:Panel ID="pnlProjectDirector" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="ddlProjectDirector" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Project Director</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlProjectDirector" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Cancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Save_Click" ToolTip="Save this Request and continue editing" Text="Save" />
        </div>
        </asp:Panel>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />

    </div>

</asp:Content>
