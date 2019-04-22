<%@ Page Title="Edit Deposit Notification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditDeposit.aspx.cs"
    Inherits="Portal11.Rqsts.EditDeposit" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Title %></h2>

    <hr />
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>

    <style>
        .rdoColWidth tr td {
            width: 40%;
        }
        .panel.col-md-3, .panel.col-md-4 {
            margin-bottom: 0px;
        }
    </style>

    <div class="form-horizontal">
        <asp:Panel runat="server" DefaultButton="btnSave">
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" AssociatedControlID="rdoDepType"
                    CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Deposit Type</asp:Label>
                <div class="panel panel-default col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                    <div class="radio">
                        <asp:RadioButtonList ID="rdoDepType" runat="server" AutoPostBack="true"
                            Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                            RepeatLayout="Table" RepeatDirection="Vertical" RepeatColumns="2"
                            OnSelectedIndexChanged="rdoDepType_SelectedIndexChanged">
                            <asp:ListItem Text="Check" Value="Check"></asp:ListItem>
                            <asp:ListItem Text="EFT" Value="EFT"></asp:ListItem>
                            <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                            <asp:ListItem Text="In kind" Value="InKind"></asp:ListItem>
                            <asp:ListItem Text="Pledge/Contract" Value="Pledge"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="rdoDepType" SetFocusOnError="true"
                        CssClass="text-danger  col-xs-12" ErrorMessage="Please select a type of the new Request." />
                </div>
            </div>
        </div>

        <!-- Current Status -->
        <asp:Panel ID="pnlState" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtState" CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Current Status</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtState" CssClass="form-control has-success" Enabled="false"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Description -->
        <asp:Panel ID="pnlDescription" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtDescription" CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Description</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" SetFocusOnError="true"
                            CssClass="text-danger  col-xs-12" ErrorMessage="Please supply a Description of the new Request." />
                    </div>
                </div>
            </div>
        </asp:Panel>

    <!-- Date of Deposit -->
        <asp:Panel ID="pnlDateOfDeposit" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label ID="lblDateOfDeposit" runat="server" AssociatedControlID="txtDateOfDeposit" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Date of Check</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtDateOfDeposit" CssClass="form-control has-success text-right"
                                 ToolTip="Enter the date that is written on the check" ></asp:TextBox>
                            <span class="input-group-addon">
                                <asp:LinkButton runat="server" ID="btnDateOfDeposit" CssClass="btn-xs btn-default"
                                    OnClick="btnDateOfDeposit_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                </asp:LinkButton>
                            </span>
                        </div>

                        <asp:Calendar ID="calDateOfDeposit" runat="server" Visible="false"
                            OnSelectionChanged="calDateOfDeposit_SelectionChanged"
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
                    <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator 
                            ControlToValidate="txtDateOfDeposit" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Please supply a date."
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator
                            ControlToValidate="txtDateOfDeposit"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The format for a date is mm/dd/yyyy."
                            Operator="DataTypeCheck" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:RangeValidator
                            ControlToValidate="txtDateOfDeposit"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The date must be between 1900 and 2099."
                            MinimumValue="01/01/1900" MaximumValuE="12/31/2099" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Options -->
        <asp:Panel ID="pnlOptions" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="cblOptions" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Options</asp:Label>
                    <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="checkbox" style="padding-top: 10pt; padding-bottom: 10pt">
                            <asp:CheckBoxList ID="cblOptions" runat="server" Style="margin-left: 20px">
                                <asp:ListItem Text="Pledge Payment" Value="PledgePayment" data-toggle="tooltip"
                                    title="This deposit is credited toward a prior pledge; it is not new money"></asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Source of Funds and Entity or Donor -->
        <asp:Panel ID="pnlSourceOfFunds" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoSourceOfFunds" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Source of Funds</asp:Label>
                    <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoSourceOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                OnSelectedIndexChanged="rdoSourceOfFunds_SelectedIndexChanged">
                                <asp:ListItem Text="Not Applicable" Value="NA" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Entity (EIN)" Value="Entity"></asp:ListItem>
                                <asp:ListItem Text="Individual (SSN)" Value="Individual"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlEntity" runat="server" Visible="false">
                    <div class="row">
                        <asp:Label ID="lblEntity" runat="server" AssociatedControlID="cblEntity" 
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Entity</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="checkbox" style="padding-top: 10pt; padding-bottom: 10pt">
                                <asp:CheckBoxList ID="cblEntity" runat="server" Style="margin-left: 20px" 
                                    OnSelectedIndexChanged="cblEntity_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Anonymous" Value="Anonymous" data-toggle="tooltip"
                                        title="This entity wishes to remain anonymous."></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Panel ID="pnlEntityDDL" runat="server">
                            <div class="col-md-3 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                                <!-- Fill this control from code-behind -->
                                <asp:DropDownList runat="server" ID="ddlEntity" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-1 col-xs-2">
                                <asp:Button ID="btnNewEntity" runat="server" Text="New" CssClass="btn btn-default col-xs-12" Visible="true"
                                    Enabled="true" OnClick="btnNewEntity_Click" CausesValidation="false" ToolTip="Add a new entity to this project. Save changes before pressing this button." />
                            </div>
                            <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                                <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlEntity" SetFocusOnError="true"
                                    CssClass="text-danger col-xs-12" ErrorMessage="Please select an Entity from the list" />
                            </div>
                        </asp:Panel>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlPerson" runat="server" Visible="false">
                    <div class="row">
                        <asp:Label ID="lblPerson" runat="server" AssociatedControlID="cblPerson" 
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Donor or Customer</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="checkbox" style="padding-top: 10pt; padding-bottom: 10pt">
                                <asp:CheckBoxList ID="cblPerson" runat="server" Style="margin-left: 20px" OnSelectedIndexChanged="cblPerson_SelectedIndexChanged" 
                                    AutoPostBack="true">
                                    <asp:ListItem Text="Anonymous" Value="Anonymous" data-toggle="tooltip"
                                        title="This person wishes to remain anonymous."></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <asp:Panel ID="pnlPersonDDL" runat="server">
                            <div class="col-md-3 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                                <!-- Fill this control from code-behind -->
                                <asp:DropDownList runat="server" ID="ddlPerson" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-1 col-xs-2">
                                <asp:Button ID="btnNewPerson" runat="server" Text="New" CssClass="btn btn-default col-xs-12" Visible="true"
                                    Enabled="true" OnClick="btnNewPerson_Click" CausesValidation="false" ToolTip="Add a new person to this project. Save changes before pressing this button." />
                            </div>
                            <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                                <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlPerson" SetFocusOnError="true" 
                                    CssClass="text-danger  col-xs-12" ErrorMessage="Please select a Donor or Customer from the list" />
                            </div>

                        </asp:Panel>
                    </div>
                </asp:Panel>

            </div>
        </asp:Panel>

        <!-- Destination of Funds and Project Class -->
<%--        <asp:Panel ID="pnlDestOfFunds" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoDestOfFunds" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Destination of Funds</asp:Label>
                    <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoDestOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                OnSelectedIndexChanged="rdoDestOfFunds_SelectedIndexChanged">
                                <asp:ListItem Text="Unrestricted (Project Activities)" Value="Unrestricted" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Restricted (Designated Revenue)" Value="Restricted"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>
            </div>--%>

            <asp:Panel ID="pnlProjectClass" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="ddlProjectClass" 
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Project Class</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <!-- Fill this control from code-behind -->
                            <asp:DropDownList runat="server" ID="ddlProjectClass" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>

        <!-- Dollar Amount -->
        <asp:Panel ID="pnlAmount" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblAmount" AssociatedControlID="txtAmount" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Dollar Amount</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control text-right" 
                            OnTextChanged="txtAmount_TextChanged" AutoPostBack="true"/>
                    </div>
                    <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ID="rfvAmount" ControlToValidate="txtAmount" SetFocusOnError="true"
                            CssClass="text-danger  col-xs-12" ErrorMessage="Please supply a Dollar Amount value." />
<%--                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAmount" SetFocusOnError="true"
                            CssClass="text-danger  col-xs-12" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\-\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$"--%>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- GL Code -->
        <asp:Panel ID="pnlGLCode" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label ID="labGLCode" runat="server" AssociatedControlID="ddlGLCode" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Income Account</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:DropDownList runat="server" ID="ddlGLCode" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-md-1 col-xs-2">
                        <asp:Button ID="btnSplit" runat="server" Text="Split" CssClass="btn btn-default col-xs-12" Visible="false"
                            Enabled="true" OnClick="btnSplit_Click" CausesValidation="false" ToolTip="Divide the deposit into multiple Accounts" />
                    </div>
                    <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator ID="rfvGLCode" runat="server" InitialValue="" ControlToValidate="ddlGLCode" SetFocusOnError="true"
                            CssClass="text-danger  col-xs-12" ErrorMessage="Please select a General Ledger Code from the list" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Account Split -->
        <asp:Panel ID="pnlDepSplit" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <div class="col-xs-offset-0 col-xs-12">
 
                        <div class="col-xs-12">
                        <asp:GridView ID="gvDepSplit" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.rowGLCodeSplit"
                            AutoGenerateColumns="false"
                            AllowPaging="false"
                            OnRowDataBound="gvDepSplit_RowDataBound">

                            <SelectedRowStyle CssClass="success" />

                            <EmptyDataTemplate>
                                <table>
                                    <tr>
                                        <td>There are no Splits for this Request</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="txtTotalRows" runat="server" Text='<%# Bind("TotalRows") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="txtSelectedProjectClassID" runat="server" Text='<%# Bind("SelectedProjectClassID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="txtSelectedGLCodeID" runat="server" Text='<%# Bind("SelectedGLCodeID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Project Class">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlSplitProjectClass" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Account">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlSplitGLCode" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dollar Amount" HeaderStyle-CssClass="text-right" ItemStyle-HorizontalAlign="Right" >
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitAmount" runat="server" CssClass="form-control" Text='<%# Bind("Amount") %>' 
                                            style="text-align:right" OnTextChanged="txtSplitAmount_TextChanged" AutoPostBack="true"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Note">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitNote" runat="server" CssClass="form-control"  Text='<%# Bind("Note") %>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Row Actions">
                                    <ItemTemplate>
                                        <asp:Button ID="btnSplitAdd" runat="server" Text="Add" CssClass="btn btn-default"
                                            OnClick="btnSplitAdd_Click" CausesValidation="false" />
                                        <asp:Button ID="btnSplitRemove" runat="server" Text="Rem" CssClass="btn btn-default"
                                            OnClick="btnSplitRemove_Click" CausesValidation="false" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </div>
                    </div>
                    <div class="text-danger col-xs-offset-1 col-xs-11">
                        <asp:Literal ID="litSplitError" runat="server" Visible="false" 
                            Text="Each split row must have a selected Account and a valid Dollar Amount" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Supporting Docs -->
        <asp:Panel ID="pnlSupporting" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="lstSupporting" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Supporting Docs</asp:Label>
                    <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:ListBox runat="server" ID="lstSupporting" CssClass="form-control" Rows="2" SelectionMode="Single"
                            OnSelectedIndexChanged="lstSupporting_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-lg-5 col-md-offset-0 col-md-6 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                        <asp:Panel ID="pnlAdd" runat="server">
                            <asp:FileUpload ID="fupUpload" runat="server" ClientIDMode="Static" onchange="this.form.submit()"
                                CssClass="hidden" />
                            <div id="btnAdd" class="btn btn-default col-md-2 col-xs-3">Add</div>
                        </asp:Panel>
                        <asp:HyperLink ID="btnViewLink" runat="server" CssClass="btn btn-default col-md-2 col-xs-offset-1 col-xs-3" Enabled="false" ToolTip="Select a row then click here to view the document"
                            NavigateUrl="overwrite from code behind" Text="View" Target="_blank" />
                        <asp:Button ID="btnRem" runat="server" Text="Remove" CssClass="btn btn-default col-md-2 col-xs-offset-1 col-xs-3"
                            Enabled="false" OnClick="btnRemove_Click" CausesValidation="false" ToolTip="Remove the selected Supporting Document from the Expense Request" />
                        <div class="col-xs-12 text-danger">
                            <asp:Literal ID="litSupportingError" runat="server" Text="Fill from code behind" Visible="false" />
                        </div>
                    </div>
                    <div class="col-xs-6 text-danger">
                        <asp:Literal runat="server" ID="litSDError" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtNotes" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Note</asp:Label>
                    <div class="col-sm-offset-0 col-sm-5 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtNotes" TextMode="MultiLine" Rows="6" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-xs-5">
                        <asp:Button ID="btnNotesClear" runat="server" Text="Clear" CssClass="btn btn-default col-md-2 col-xs-3"
                            Enabled="true" OnClick="btnNotesClear_Click" CausesValidation="false" ToolTip="Clear the text of the Note" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Return Note -->
        <asp:Panel ID="pnlReturnNote" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtReturnNote" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Return Note</asp:Label>
                    <div class="col-sm-offset-0 col-sm-5 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtReturnNote" CssClass="form-control" TextMode="MultiLine" Rows="6" Enabled="false" />
                    </div>
                    <div class="col-xs-5">
                        <asp:Button ID="btnReturnNoteClear" runat="server" Text="Clear" CssClass="btn btn-default col-md-2 col-xs-3"
                            Enabled="true" OnClick="btnReturnNoteClear_Click" CausesValidation="false" ToolTip="Clear the text of the Return Note" />
                        <div class="col-xs-12">
                            <br />
                            <asp:RequiredFieldValidator ID="rfvReturnNote" runat="server" InitialValue="" ControlToValidate="txtReturnNote" SetFocusOnError="true"
                                CssClass="text-danger  col-xs-12" ErrorMessage="Please supply a reason for revising the request"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Staff Note -->
        <asp:Panel ID="pnlStaffNote" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtStaffNote" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">
                            Staff Note<br />(visible only to other staff)</asp:Label>
                    <div class="col-sm-offset-0 col-sm-5 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtStaffNote" CssClass="form-control" TextMode="MultiLine" Rows="6"/>
                    </div>
                    <div class="col-xs-5">
                        <asp:Button ID="btnStaffNoteClear" runat="server" Text="Clear" CssClass="btn btn-default col-md-2 col-xs-3"
                            Enabled="true" OnClick="btnStaffNoteClear_Click" CausesValidation="false" ToolTip="Clear the text of the Staff Note" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- History Grid -->
        <asp:Panel ID="pnlHistory" runat="server" Visible="false">
            <div class="row col-xs-12" style="margin-bottom:10px">

                    <!-- Code assumes that RowID is the first column of this grid -->
                    <asp:GridView ID="gvEDHistory" runat="server"
                        CssClass="table table-striped table-hover"
                        ItemType="Portal11.Models.rowEDHistory"
                        AutoGenerateColumns="false"
                        AllowPaging="true" PageSize="20"
                        OnPageIndexChanging="gvEDHistory_PageIndexChanging">

                        <SelectedRowStyle CssClass="success" />

                        <PagerStyle CssClass="active" HorizontalAlign="Center"></PagerStyle>
                        <PagerTemplate>
                            <asp:Button ID="ButtonFirst" runat="server" Text="<<" CommandName="Page"
                                CommandArgument="First" CausesValidation="false" 
                                CssClass="btn btn-sm btn-default"></asp:Button>
                            <asp:Button ID="ButtonPrev" runat="server" Text="<" CommandName="Page"
                                CommandArgument="Prev" CausesValidation="false" 
                                CssClass="btn btn-sm btn-default"></asp:Button>
                            <asp:Button ID="ButtonNext" runat="server" Text=">" CommandName="Page"
                                CommandArgument="Next" CausesValidation="false" 
                                CssClass="btn btn-sm btn-default"></asp:Button>
                            <asp:Button ID="ButtonLast" runat="server" Text=">>" CommandName="Page"
                                CommandArgument="Last" CausesValidation="false" Enabled="false"
                                CssClass="btn btn-sm btn-default"></asp:Button>
                        </PagerTemplate>

                        <EmptyDataTemplate>
                            <table>
                                <tr>
                                    <td>There are no History entries for this Deposit Request</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:g}" />
                            <asp:BoundField DataField="FormerStatus" HeaderText="Former Status" />
                            <asp:BoundField DataField="EstablishedBy" HeaderText="Established By" />
                            <asp:BoundField DataField="ReasonForChange" HeaderText="Reason For Change" />
                            <asp:BoundField DataField="UpdatedStatus" HeaderText="Updated Status" />
                            <asp:BoundField DataField="ReturnNote" HeaderText="Return Note" />
                        </Columns>
                    </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row col-xs-12">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger col-xs-offset-1 col-md-1 col-sm-offset-0 col-xs-offset-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-warning col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnSave_Click" CausesValidation="false" ToolTip="Save this Deposit Request and return to the Dashboard"/>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success col-xs-offset-1 col-md-1 col-xs-2" Enabled="true" 
                OnClick="btnSubmit_Click" ToolTip="Save this Deposit Request, submit it for approval and return to the Dashboard" />
            <asp:Button ID="btnShowHistory" runat="server" Text="History" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnShowHistory_Click" CausesValidation="false" ToolTip="List the changes that this Deposit Request has gone through" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDefaultProjectClassID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDepID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDepState" runat="server" Visible="false" />
        <asp:Literal ID="litSavedEntityEnum" runat="server" Visible="false" />
        <asp:Literal ID="litSavedEntityPersonFlag" runat="server" Visible="false" />
        <asp:Literal ID="litSavedPersonEnum" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectRole" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedStateEnum" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
        <asp:Literal ID="litSupportingDocMin" runat="server" Visible="false" Text="Minimum"></asp:Literal>

        </asp:Panel>
    </div>

    <!-- This little style helps us hide the File Upload button. And this piece of Javascript makes the click event of the Add button trigger the click event of the File Upload control. This is
    because the File Upload control's native button is too ugly, i.e., in a style that's inconsistent with the rest of the pages. -->

    <style>
        .hidden {
            display: none;
        }
    </style>

    <script type="text/javascript">
        $("#btnAdd").click(function () {
            $("#fupUpload").click();
        });

    </script>

    <%--Modal dialog box to tollboth our path to the Assign Entity/Person To Project Page--%>

    <div ID="divModalNew" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title text-success" style="text-align:center">Confirmation</h3>
                </div>
                <div class="modal-body">
                    <h4>This operation will save the current request.</h4>
                    <h4>Are you sure you want to proceed?</h4>
                </div>
                <div class="modal-footer">
                    <div class="row col-xs-12">
                    <asp:Button ID="btnModalCancel" runat="server" Text="Cancel" CssClass="btn btn-danger col-xs-2" data-dismiss="modal" CausesValidation="false"
                        ToolTip="Close this window without saving." />
                    <asp:Button ID="btnModalNo" runat="server" Text="No" CssClass="btn btn-warning col-xs-offset-1 col-xs-2" data-dismiss="modal" CausesValidation="false"
                        ToolTip="Do not proceed." />
                    <asp:Button ID="btnModalYes" runat="server" Text="Yes" CssClass="btn btn-success col-xs-offset-1 col-xs-2" CausesValidation="false"
                        OnClick="btnModalYes_Click" ToolTip="Save the request, then add a new entity or person." />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
