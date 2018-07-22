<%@ Page Title="Edit Expense Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="EditExpense.aspx.cs" Inherits="Portal11.Rqsts.EditExpense" MaintainScrollPositionOnPostback="true" %>

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

        .panel.col-md-3, .panel.col-md-4, .panel.col-md-5 {
            margin-bottom: 0px;
        }

        .table {
            margin-bottom: 0px;
        }
    </style>

    <div class="form-horizontal">
        <asp:Panel runat="server" DefaultButton="btnSave">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoExpType"
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Expense Type</asp:Label>
                    <div class="panel panel-default col-md-5 col-sm-offset-0 col-xs-offset-1 col-xs-11">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoExpType" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                RepeatLayout="Table" RepeatDirection="Vertical" RepeatColumns="2"
                                OnSelectedIndexChanged="rdoExpType_SelectedIndexChanged">
                                <asp:ListItem Text="Contractor Invoice" Value="ContractorInvoice"></asp:ListItem>
                                <asp:ListItem Text="Payroll" Value="Payroll"></asp:ListItem>
                                <asp:ListItem Text="Reimbursement" Value="Reimbursement"></asp:ListItem>
                                <asp:ListItem Text="Vendor Invoice" Value="VendorInvoice"></asp:ListItem>
                                <asp:ListItem Text="Purchase Order" Value="PurchaseOrder"></asp:ListItem>
                                <asp:ListItem Text="PEX Card" Value="PEXCard"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="col-sm-offset-0 col-xs-offset-1 col-xs-5">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="rdoExpType" SetFocusOnError="true"
                            CssClass="text-danger col-xs-12" ErrorMessage="Please select a type of the new Request." />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlState" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtState"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Current Status</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtState" CssClass="form-control has-success" Enabled="false" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Description -->
            <asp:Panel ID="pnlDescription" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtDescription"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Description</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter a Description of the new Request." />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Beginning/Ending Dates -->
            <asp:Panel ID="pnlBeginningEnding" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtBeginningDate"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Pay Period - Beginning</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="input-group">

                                <asp:TextBox runat="server" ID="txtBeginningDate" CssClass="form-control has-success text-right"
                                    ToolTip="Enter the date on which the pay period began" />
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
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator 
                            ControlToValidate="txtBeginningDate" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Please enter a date."
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator
                            ControlToValidate="txtBeginningDate"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The format for a date is mm/dd/yyyy."
                            Operator="DataTypeCheck" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:RangeValidator
                            ControlToValidate="txtBeginningDate"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The date must be between 1900 and 2099."
                            MinimumValue="01/01/1900" MaximumValue="12/31/2099" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtEndingDate"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Pay Period - Ending</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">

                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtEndingDate" CssClass="form-control has-success text-right"
                                    ToolTip="Enter the date of the end of the pay period" />
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
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator 
                            ControlToValidate="txtEndingDate" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Please enter a date."
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator
                            ControlToValidate="txtEndingDate"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The format for a date is mm/dd/yyyy."
                            Operator="DataTypeCheck" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:RangeValidator
                            ControlToValidate="txtEndingDate"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The date must be between 1900 and 2099."
                            MinimumValue="01/01/1900" MaximumValuE="12/31/2099" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator 
                            ControlToCompare="txtBeginningDate" 
                            ControlToValidate="txtEndingDate" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Ending Date must come after Beginning Date"
                            runat="server"  SetFocusOnError="true"
                            Type="Date" Operator="GreaterThanEqual" 
                            />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Date Needed -->
            <asp:Panel ID="pnlDateNeeded" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtDateNeeded"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Date Needed</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="input-group">

                                <asp:TextBox runat="server" ID="txtDateNeeded" CssClass="form-control has-success text-right"
                                    ToolTip="Enter the date when you need this item"></asp:TextBox>
                                <span class="input-group-addon">
                                    <asp:LinkButton runat="server" ID="btnDateNeeded" CssClass="btn-xs btn-default"
                                        OnClick="btnDateNeeded_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                    </asp:LinkButton>
                                </span>
                            </div>

                            <asp:Calendar ID="calDateNeeded" runat="server" Visible="false"
                                OnSelectionChanged="calDateNeeded_SelectionChanged"
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
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator 
                            ControlToValidate="txtDateNeeded" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Please enter a date."
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator
                            ControlToValidate="txtDateNeeded"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The format for a date is mm/dd/yyyy."
                            Operator="DataTypeCheck" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:RangeValidator
                            ControlToValidate="txtDateNeeded"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The date must be between 1900 and 2099."
                            MinimumValue="01/01/1900" MaximumValuE="12/31/2099" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Goods to Purchase -->
            <asp:Panel ID="pnlGoods" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtGoodsDescription"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Description of Goods</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox ID="txtGoodsDescription" runat="server" CssClass="form-control has-success" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsDescription" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter a Description of Goods for the new Request." />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtGoodsSKU"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">SKU or Model Number</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox ID="txtGoodsSKU" runat="server" CssClass="form-control has-success"></asp:TextBox>
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsSKU" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter a SKU or Model Number for the new Request." />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtGoodsQuantity"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Quantity</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox ID="txtGoodsQuantity" runat="server" CssClass="form-control has-success text-right" 
                                CausesValidation="true" ValidationGroup="valGoodsQuantity"
                                OnTextChanged="txtGoodsCostPerUnit_TextChanged" AutoPostBack="true">
                            </asp:TextBox>
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtGoodsQuantity" 
                                CssClass="text-danger col-xs-12"
                                Display="Dynamic" ErrorMessage="Please enter a value."
                                runat="server" SetFocusOnError="true" 
                                ValidationGroup="valGoodsQuantity" 
                                />
                            <asp:CompareValidator
                                ControlToValidate="txtGoodsQuantity"
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a number."
                                Operator="DataTypeCheck" Type="Integer"
                                runat="server" SetFocusOnError="true"
                                ValidationGroup="valGoodsQuantity" 
                                />
                            <asp:RangeValidator 
                                ControlToValidate="txtGoodsQuantity" 
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a value between 1 and 500." 
                                runat="server" SetFocusOnError="true" 
                                Type="Integer" MinimumValue="1" MaximumValue="500"
                                ValidationGroup="valGoodsQuantity" 
                                />
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtGoodsCostPerUnit"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Cost Per Unit</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox ID="txtGoodsCostPerUnit" runat="server" CssClass="form-control has-success text-right" 
                                CausesValidation="true" ValidationGroup="valGoodsCostPerUnit"
                                OnTextChanged="txtGoodsCostPerUnit_TextChanged" AutoPostBack="true">
                            </asp:TextBox>
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtGoodsCostPerUnit" 
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a value."
                                runat="server" SetFocusOnError="true" 
                                ValidationGroup="valGoodsCostPerUnit"
                                />
                            <asp:RegularExpressionValidator
                                ControlToValidate="txtGoodsCostPerUnit"
                                CssClass="text-danger col-xs-12"
                                Display="Dynamic" ErrorMessage="Please enter a currency value."
                                runat="server" SetFocusOnError="true"
                                ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$"
                                ValidationGroup="valGoodsCostPerUnit"
                                />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Cards -->
            <asp:Panel ID="pnlCards" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtNumberOfCards"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Number of Cards</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtNumberOfCards" CssClass="form-control has-success text-right"
                                OnTextChanged="txtNumberOfCards_TextChanged" AutoPostBack="true" 
                                Text="1" CausesValidation="true" ValidationGroup="valNumberOfCards" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtNumberOfCards" 
                                CssClass="text-danger col-xs-12"
                                Display="Dynamic" ErrorMessage="Please enter a value."
                                runat="server" SetFocusOnError="true" 
                                ValidationGroup="valNumberOfCards" 
                                />
                            <asp:CompareValidator
                                ControlToValidate="txtNumberOfCards"
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a number."
                                Operator="DataTypeCheck" Type="Integer"
                                runat="server" SetFocusOnError="true"
                                ValidationGroup="valNumberOfCards" 
                                />
                            <asp:RangeValidator 
                                ControlToValidate="txtNumberOfCards" 
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a value between 1 and 500." 
                                runat="server" SetFocusOnError="true" 
                                Type="Integer" MinimumValue="1" MaximumValue="500"
                                ValidationGroup="valNumberOfCards" 
                                />
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtEachCard"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Cash Value of Each Card</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtEachCard" CssClass="form-control has-success text-right"
                                OnTextChanged="txtNumberOfCards_TextChanged" AutoPostBack="true" 
                                CausesValidation="true" ValidationGroup="valEachCard" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator 
                                ControlToValidate="txtEachCard" 
                                CssClass="text-danger col-xs-12" 
                                Display="Dynamic" ErrorMessage="Please enter a value."
                                runat="server" SetFocusOnError="true" 
                                ValidationGroup="valEachCard"
                                />
                            <asp:RegularExpressionValidator
                                ControlToValidate="txtEachCard"
                                CssClass="text-danger col-xs-12"
                                Display="Dynamic" ErrorMessage="Please enter a currency value."
                                runat="server" SetFocusOnError="true"
                                ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$"
                                ValidationGroup="valEachCard"
                                />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Date of Invoice -->
            <asp:Panel ID="pnlDateOfInvoice" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtDateOfInvoice"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Date of Invoice</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtDateOfInvoice" CssClass="form-control has-success text-right"
                                    ToolTip="Enter the date on the Invoice"></asp:TextBox>
                                <span class="input-group-addon">
                                    <asp:LinkButton runat="server" ID="btnDateOfInvoice" CssClass="btn-xs btn-default"
                                        OnClick="btnDateOfInvoice_Click" CausesValidation="false">
                                    <span aria-hidden="true" class="glyphicon glyphicon-calendar" > </span>
                                    </asp:LinkButton>
                                </span>
                            </div>

                            <asp:Calendar ID="calDateOfInvoice" runat="server" Visible="false"
                                OnSelectionChanged="calDateOfInvoice_SelectionChanged"
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
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator 
                            ControlToValidate="txtDateOfInvoice" 
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="Please enter a date."
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:CompareValidator
                            ControlToValidate="txtDateOfInvoice"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The format for a date is mm/dd/yyyy."
                            Operator="DataTypeCheck" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                        <asp:RangeValidator
                            ControlToValidate="txtDateOfInvoice"
                            CssClass="text-danger col-xs-12" 
                            Display="Dynamic" ErrorMessage="The date must be between 1900 and 2099."
                            MinimumValue="01/01/1900" MaximumValuE="12/31/2099" Type="Date"
                            runat="server" SetFocusOnError="true"
                            />
                    </div>
                </div>
                </div>
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtInvoiceNumber"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Invoice Number</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control has-success text-right" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtInvoiceNumber" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter an Invoice Number of the new Request." />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Contract Questions -->
            <asp:Panel ID="pnlContractQuestions" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="rdoContractOnFile"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Contract Questions</asp:Label>
                        <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="col-xs-12">
                                <asp:Literal ID="litContractInvoice" runat="server">Is this Invoice applied to an existing contract currently on file?</asp:Literal>
                            </div>
                            <div class="radio col-xs-12">
                                <asp:RadioButtonList ID="rdoContractOnFile" runat="server" AutoPostBack="true"
                                    Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                    OnSelectedIndexChanged="rdoContractOnFile_SelectedIndexChanged">
                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                    <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ID="radRfv" ControlToValidate="rdoContractOnFile" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please select Yes or No" />
                        </div>
                    </div>
                    <asp:Panel ID="pnlContractReason" runat="server" Visible="false">
                        <div class="row">
                            <div class="panel panel-default col-md-3 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                                <div class="col-xs-12">
                                    <asp:Literal ID="litContractReason" runat="server">According to the contract, why are we getting this invoice?</asp:Literal>
                                </div>
                                <div class="col-xs-12">
                                    <asp:TextBox runat="server" ID="txtContractReason" CssClass="form-control" TextMode="MultiLine" placeholder="Reason for Invoice" />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlContractComing" runat="server" Visible="false">
                        <div class="row">
                            <div class="panel panel-default col-md-3 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                                <div class="col-xs-12">
                                    <asp:Literal ID="litContractComing" runat="server">Does the vendor intend to submit a contract?</asp:Literal>
                                </div>
                                <div class="radio col-xs-12">
                                    <asp:RadioButtonList ID="rdoContractComing" runat="server" AutoPostBack="true"
                                        Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                        OnSelectedIndexChanged="rdoContractComing_SelectedIndexChanged">
                                        <asp:ListItem Text="Yes" Value="Yes" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <div class="col-xs-12 text-danger">
                                    <asp:Literal ID="litContractNeeded" runat="server" Visible="false">Do Not Proceed. Contracts must be submitted and approved before Invoice payments are accepted</asp:Literal>
                                </div>
                                <div class="col-xs-12 text-success">
                                    <asp:Literal ID="litContractNone" runat="server" Visible="false">Proceed. Thank you.</asp:Literal>
                                </div>
                            </div>
                            <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="rdoContractComing" SetFocusOnError="true"
                                    CssClass="text-danger col-xs-12" ErrorMessage="Please select Yes or No" />
                            </div>
                        </div>
                    </asp:Panel>
                </div>

            </asp:Panel>

            <!-- Dollar Amount -->
            <asp:Panel ID="pnlAmount" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" ID="lblAmount" AssociatedControlID="txtAmount"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Dollar Amount</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control has-success text-right" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ID="rfvAmount" ControlToValidate="txtAmount" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter a Dollar Amount value." />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAmount" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Currency values only. For example, $123.45"
                                ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Fulfillment Questions -->
            <asp:Panel ID="pnlPOFulFillmentInstructions" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="rdoPOVendorMode"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Fulfillment Instructions</asp:Label>
                        <div class="panel panel-default col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="radio">
                                <asp:RadioButtonList ID="rdoPOVendorMode" runat="server" AutoPostBack="true"
                                    Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth" Visible="true"
                                    OnSelectedIndexChanged="rdoPOVendorMode_SelectedIndexChanged">
                                    <asp:ListItem Text="Any version of this item will do." Value="Yes" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Please use SKU/Model# and purchase from this vendor." Value="No"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Entity Name -->
            <asp:Panel ID="pnlEntity" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" ID="lblEntity" AssociatedControlID="ddlEntity"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Vendor</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <!-- Fill this control from code-behind -->
                            <asp:DropDownList runat="server" ID="ddlEntity" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-xs-3">
                            <asp:Button ID="btnNewEntity" runat="server" Text="New" CssClass="btn btn-default col-xs-12" Visible="true"
                                Enabled="true" OnClick="btnNewEntity_Click" CausesValidation="false" ToolTip="Add a new entity to this project. Save changes before pressing this button." />
                        </div>
                        <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlEntity" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please select a Vendor from the list" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Person Name -->
            <asp:Panel ID="pnlPerson" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" ID="lblPerson" AssociatedControlID="ddlPerson"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Person</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <!-- Fill this control from code-behind -->
                            <asp:DropDownList runat="server" ID="ddlPerson" CssClass="form-control"></asp:DropDownList>
                            <asp:TextBox runat="server" ID="txtPersonPayroll" CssClass="form-control" Text="(see splits)" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-md-1 col-xs-3">
                            <asp:Button ID="btnNewPerson" runat="server" Text="New" CssClass="btn btn-default col-xs-12" Visible="true"
                                Enabled="true" OnClick="btnNewPerson_Click" CausesValidation="false" ToolTip="Add a new person to this project. Save changes before pressing this button." />
                        </div>
                        <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator ID="rfvPerson" runat="server" InitialValue="" ControlToValidate="ddlPerson" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please select a Person from the list" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- URL -->
            <asp:Panel ID="pnlURL" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtURL"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">URL</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtURL" CssClass="form-control" />
                        </div>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtURL" SetFocusOnError="true"
                            CssClass="text-danger col-xs-12" ErrorMessage="URL format is incorrect"
                            ValidationExpression="^\s*((?:https?://)?(?:[\w-]+\.)+[\w-]+)(/[\w ./?%&=-]*)?\s*$" />
                    </div>
                </div>
            </asp:Panel>

            <!-- Project Class -->
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

            <!-- GL Code -->
            <asp:Panel ID="pnlGLCode" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="ddlGLCode"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Expense Account</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:DropDownList runat="server" ID="ddlGLCode" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="col-md-1 col-xs-3">
                            <asp:Button ID="btnSplitGL" runat="server" Text="Split" CssClass="btn btn-default col-xs-12" Visible="false"
                                Enabled="true" OnClick="btnSplitGL_Click" CausesValidation="false" ToolTip="Divide the request into multiple Expense Accounts" />
                        </div>
                        <div class="col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator ID="rfvGLCode" runat="server" InitialValue="" ControlToValidate="ddlGLCode" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please select a General Ledger Code from the list"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                </div>
            </asp:Panel>

<%--We use two gridviews to split Expenses: gvSplitGL to split Expense Accounts for Contractor Invoices, Reimbursements, and Vendor Invoices;
    and gvSplitPayroll for Payroll. They share an ItemType, so they're often loaded and unloaded by the same code. But their appearance is distinct enough
    to justify having separate gridviews.--%>

            <asp:Panel ID="pnlSplitGL" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-offset-0 col-xs-12">

                            <div class="col-xs-12">
                                <asp:GridView ID="gvSplitGL" runat="server"
                                    CssClass="table table-striped table-hover"
                                    ItemType="Portal11.Models.rowGLCodeSplit"
                                    AutoGenerateColumns="false"
                                    AllowPaging="false"
                                    OnRowDataBound="gvSplitGL_RowDataBound">

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
                                        <asp:TemplateField HeaderText="Expense Account">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlSplitGLCode" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dollar Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSplitAmount" runat="server" CssClass="form-control" Text='<%# Bind("Amount") %>'
                                                    Style="text-align: right" OnTextChanged="txtSplitGLAmount_TextChanged" AutoPostBack="true" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSplitNote" runat="server" CssClass="form-control" Text='<%# Bind("Note") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Row Actions">
                                            <ItemTemplate>
                                                <asp:Button ID="btnSplitGLAdd" runat="server" Text="Add" CssClass="btn btn-default"
                                                    OnClick="btnSplitGLAdd_Click" CausesValidation="false" />
                                                <asp:Button ID="btnSplitGLRemove" runat="server" Text="Rem" CssClass="btn btn-default"
                                                    OnClick="btnSplitGLRemove_Click" CausesValidation="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="text-danger col-xs-offset-1 col-xs-11">
                            <asp:Literal ID="litSplitGLError" runat="server" Visible="false"
                                Text="Each split expense row must have a selected Expense Account and a valid Dollar Amount" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Expense Split for Payroll Split -->
            <asp:Panel ID="pnlSplitPayroll" runat="server" Visible="true">
                <div class="form-group">
                    <div class="row">
                        <div class="col-xs-offset-0 col-xs-12">

                            <div class="col-xs-12">
                                <asp:GridView ID="gvSplitPayroll" runat="server"
                                    CssClass="table table-striped table-hover"
                                    ItemType="Portal11.Models.rowGLCodeSplit"
                                    AutoGenerateColumns="false"
                                    AllowPaging="false"
                                    OnRowDataBound="gvSplitPayroll_RowDataBound">

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
                                                <asp:Label ID="txtSelectedPersonID" runat="server" Text='<%# Bind("SelectedPersonID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Project Class">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlSplitProjectClass" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Employee">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlSplitPerson" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hourly Rate" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <div style="width:100px; overflow: hidden; white-space:nowrap; text-overflow:ellipsis">
                                                <asp:TextBox ID="txtSplitHourlyRate" runat="server" CssClass="form-control" Text='<%# Bind("HourlyRate") %>'
                                                    Style="text-align: right" OnTextChanged="txtSplitPayrollAmount_TextChanged" AutoPostBack="true" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Hours Paid" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <div style="width:100px; overflow: hidden; white-space:nowrap; text-overflow:ellipsis">
                                                <asp:TextBox ID="txtSplitHoursPaid" runat="server" CssClass="form-control" Text='<%# Bind("HoursPaid") %>'
                                                    Style="text-align: right" OnTextChanged="txtSplitPayrollAmount_TextChanged" AutoPostBack="true" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Gross Pay" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <div style="width:120px; overflow: hidden; white-space:nowrap; text-overflow:ellipsis">
                                                <asp:TextBox ID="txtSplitAmount" runat="server" CssClass="form-control" Text='<%# Bind("Amount") %>'
                                                    Style="text-align: right" Enabled="false" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Right">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtSplitNote" runat="server" CssClass="form-control" Text='<%# Bind("Note") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Row Actions">
                                            <ItemTemplate>
                                                <div style="width:130px;">
                                                <asp:Button ID="btnSplitPayrollAdd" runat="server" Text="Add" CssClass="btn btn-default"
                                                    OnClick="btnSplitPayrollAdd_Click" CausesValidation="false" />
                                                <asp:Button ID="btnSplitPayrollRemove" runat="server" Text="Rem" CssClass="btn btn-default"
                                                    OnClick="btnSplitPayrollRemove_Click" CausesValidation="false" />
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="text-danger col-xs-offset-1 col-xs-11">
                            <asp:Literal ID="litSplitPayrollError" runat="server" Visible="false"
                                Text="Each split expense row must have a selected Employee, a non-zero Hourly Rate, and a non-zero Hours Paid" />
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- PaymentMethod -->
            <asp:Panel ID="pnlPaymentMethod" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="rdoPaymentMethod"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Payment Method</asp:Label>
                        <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="radio">
                                <asp:RadioButtonList ID="rdoPaymentMethod" runat="server" AutoPostBack="true"
                                    Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth" 
                                    OnSelectedIndexChanged="rdoPaymentMethod_SelectedIndexChanged">
                                    <asp:ListItem Text="Check" Value="Check" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="CultureTrust Credit Card" Value="CreditCard" Enabled="false"></asp:ListItem>
                                    <asp:ListItem Text="EFT/Direct Deposit" Value="EFT"></asp:ListItem>
                                    <asp:ListItem Text="Request Invoice" Value="Invoice"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <%--Delivery Instructions - A big panel with three smaller panels inside: DeliveryModeReg for most, DeliveryModePO for PO, DeliveryModeRush for all.--%>

            <asp:Panel ID="pnlDeliveryInstructions" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="cblDeliveryModeRush"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Instructions</asp:Label>
                        <div class="panel panel-default col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">

                            <%--A panel of radio buttons for Delivery Mode--%>

                            <asp:Panel ID="pnlDeliveryModeReg" runat="server" Visible="false">
                                <div class="radio">
                                    <asp:RadioButtonList ID="rdoDeliveryModeReg" runat="server" AutoPostBack="true"
                                        Style="margin-left: 20px;" CssClass="rdoColWidth" Visible="true"
                                        OnSelectedIndexChanged="rdoDeliveryModeReg_SelectedIndexChanged">
                                        <asp:ListItem Text="Hold for pickup." Value="Pickup" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Mail to payee." Value="MailPayee"></asp:ListItem>
                                        <asp:ListItem Text="Mail to the below address." Value="MailAddress"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div> <%--End of first radio--%>
                            </asp:Panel> <%--End of pnlDeliveryModeReg--%>

                            <%--A panel of radio buttons for PO Delivery Mode--%>

                            <asp:Panel ID="pnlDeliveryModePO" runat="server" Visible="false">
                                <div class="radio">
                                    <asp:RadioButtonList ID="rdoDeliveryModePO" runat="server" AutoPostBack="true"
                                        Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth" Visible="true"
                                        OnSelectedIndexChanged="rdoDeliveryModePO_SelectedIndexChanged">
                                        <asp:ListItem Text="Have the vendor hold this item. I will pick it up." Value="Pickup" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Have the item delivered to CultureWorks." Value="DeliverCW"></asp:ListItem>
                                        <asp:ListItem Text="Have the item delivered to the below address." Value="DeliverAddress"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </div> <%--End of second radio--%>
                            </asp:Panel> <%--End of pnlDeliveryModePO--%>

                            <%--A panel of check boxes (only one) for Rush. Everybody uses it, so always on.--%>

                            <asp:Panel ID="pnlDeliveryModeRush" runat="server" Visible="true">
                                <div class="checkbox text-danger">
                                    <asp:CheckBoxList ID="cblDeliveryModeRush" runat="server" AutoPostBack="true" 
                                        Style="margin-left: 20px; margin-bottom: 10px" Visible="true"
                                        OnSelectedIndexChanged="cblDeliveryModeRush_SelectedIndexChanged">
                                        <asp:ListItem Text="Rush" Value="Rush" data-toggle="tooltip"
                                            title="Please be aware that by checking this box, CultureWorks staff may, at their discretion, charge you a rush payment fee as stated in the User Manual on the CultureTrust website."></asp:ListItem>
                                    </asp:CheckBoxList>
                                </div> <%--End of check box list--%>
                            </asp:Panel> <%--End of pnlDeliveryModeRush--%>
                        </div> <%--End of panel--%>
                    </div> <%--End of Row--%>
                </div> <%--End of form-group--%>
            </asp:Panel> <%--End of outer panel--%>

<%--            <asp:Panel ID="pnlPODeliveryInstructions" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="rdoPODeliveryMode"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Instructions</asp:Label>
                        <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="radio">
                                <asp:RadioButtonList ID="rdoPODeliveryMode" runat="server" AutoPostBack="true"
                                    Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth" Visible="true"
                                    OnSelectedIndexChanged="rdoPODeliveryMode_SelectedIndexChanged">
                                    <asp:ListItem Text="Have the vendor hold this item. I will pick it up." Value="Pickup" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Have the item delivered to CultureWorks." Value="DeliverCW"></asp:ListItem>
                                    <asp:ListItem Text="Have the item delivered to the below address." Value="DeliverAddress"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlDeliveryInstructions" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="rdoDeliveryMode"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Instructions</asp:Label>
                        <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <div class="radio">
                                <asp:RadioButtonList ID="rdoDeliveryMode" runat="server" AutoPostBack="true"
                                    Style="margin-left: 20px;" CssClass="rdoColWidth" Visible="true"
                                    OnSelectedIndexChanged="rdoDeliveryMode_SelectedIndexChanged">
                                    <asp:ListItem Text="Hold for pickup." Value="Pickup" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Mail to payee." Value="MailPayee"></asp:ListItem>
                                    <asp:ListItem Text="Mail to the below address." Value="MailAddress"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                            <div class="checkbox text-danger">
                                <asp:CheckBoxList ID="cblDeliveryInstructions" runat="server" Style="margin-left: 20px; margin-bottom: 10px">
                                    <asp:ListItem Text="Rush" Value="Rush" data-toggle="tooltip"
                                        title="Please be aware that by checking this box, CultureWorks staff may, at their discretion, charge you a rush payment fee as stated in the User Manual on the CultureTrust website."></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>--%>

            <asp:Panel ID="pnlDeliveryAddress" runat="server" Visible="false">
                <div class="form-group">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="txtDeliveryAddress"
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Address</asp:Label>
                        <div class="col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:TextBox runat="server" ID="txtDeliveryAddress" CssClass="form-control" placeholder="Delivery Address"
                                TextMode="MultiLine" Visible="true" />
                        </div>
                        <div class="col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <asp:RequiredFieldValidator runat="server" ID="rfvDeliveryAddress" ControlToValidate="txtDeliveryAddress" SetFocusOnError="true"
                                CssClass="text-danger col-xs-12" ErrorMessage="Please enter a Delivery Address value." />
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
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtNotes" TextMode="MultiLine" Rows="6" CssClass="form-control has-success" />
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
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtReturnNote" CssClass="form-control" TextMode="MultiLine" Rows="6" Enabled="false" />
                        </div>
                        <div class="col-xs-5">
                            <asp:Button ID="btnReturnNoteClear" runat="server" Text="Clear" CssClass="btn btn-default col-md-2 col-xs-3"
                                Enabled="true" OnClick="btnReturnNoteClear_Click" CausesValidation="false" ToolTip="Clear the text of the Return Note" />
                            <div class="col-xs-12">
                                <br />
                                <asp:RequiredFieldValidator ID="rfvReturnNote" runat="server" InitialValue="" ControlToValidate="txtReturnNote" SetFocusOnError="true"
                                    CssClass="text-danger col-xs-12" ErrorMessage="Please enter a reason for revising the request"></asp:RequiredFieldValidator>
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
                        <div class="col-xs-5">
                            <asp:TextBox runat="server" ID="txtStaffNote" CssClass="form-control" TextMode="MultiLine" Rows="6" />
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
                <div class="row col-xs-12" style="margin-bottom: 10px">

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
                                    <td>There are no History entries for this Expense Request</td>
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
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-warning col-xs-offset-1 col-md-1 col-xs-2" Enabled="true" UseSubmitBehavior="true"
                    OnClick="btnSave_Click" CausesValidation="false" ToolTip="Save this Expense Request and return to the Dashboard" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                    OnClick="btnSubmit_Click" CausesValidation="true" ToolTip="Save this Expense Request, submit it for approval and return to the Dashboard" />
                <asp:Button ID="btnShowHistory" runat="server" Text="History" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                    OnClick="btnShowHistory_Click" CausesValidation="false" ToolTip="List the changes that this Expense Request has gone through" />
            </div>

            <!-- "Scratch" storage used during form processing -->
            <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
            <asp:Literal ID="litSavedDefaultProjectClassID" runat="server" Visible="false" />
            <asp:Literal ID="litSavedEntityEnum" runat="server" Visible="false" />
            <asp:Literal ID="litSavedEntityPersonFlag" runat="server" Visible="false" />
            <asp:Literal ID="litSavedExpID" runat="server" Visible="false" />
            <asp:Literal ID="litSavedExpState" runat="server" Visible="false" />
            <asp:Literal ID="litSavedPersonEnum" runat="server" Visible="false" />
            <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
            <asp:Literal ID="litSavedProjectRole" runat="server" Visible="false" />
            <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
            <asp:Literal ID="litSavedStateEnum" runat="server" Visible="false" />
            <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
            <asp:Literal ID="litSupportingDocMin" runat="server" Visible="false" Text="Minimum"></asp:Literal>

        </asp:Panel>
    </div>

    <%--    This little style helps us hide the File Upload button. And this piece of Javascript makes the click event of the Add button trigger the click event of the File Upload control. This is
        because the File Upload control's native button is too ugly, i.e., in a style that's inconsistent with the rest of the pages.--%>

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

    <div id="divModalNew" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title text-success" style="text-align: center">Confirmation</h3>
                </div>
                <div class="modal-body">
                    <h4>This operation will save the current request.</h4>
                    <h4>Are you sure you want to proceed?</h4>
                </div>
                <div class="modal-footer">
                    <div class="row col-xs-12">
                        <asp:Button runat="server" Text="Cancel" CssClass="btn btn-default col-xs-2" data-dismiss="modal" CausesValidation="false"
                            ToolTip="Close this window without saving." />
                        <asp:Button runat="server" Text="No" CssClass="btn btn-default col-xs-offset-1 col-xs-2" data-dismiss="modal" CausesValidation="false"
                            ToolTip="Do not proceed." />
                        <asp:Button runat="server" Text="Yes" CssClass="btn btn-primary col-xs-offset-1 col-xs-2" CausesValidation="false"
                            OnClick="btnModalNewYes_Click" ToolTip="Save the request, then add a new entity or person." />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--Modal dialog box to tollboth the use of the Rush option--%>

    <div id="divModalRush" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title text-warning" style="text-align: center">Confirmation</h3>
                </div>
                <div class="modal-body">
                    <h4>Please be aware that by checking this box, CultureWorks staff may,</h4>
                    <h4>at their discretion, charge you a rush payment fee as stated </h4>
                    <h4>in the User Manual on the CultureTrust website.</h4>
                    <br />
                    <h4>Are you sure you want to proceed?</h4>
                </div>
                <div class="modal-footer">
                    <div class="row col-xs-12">
                        <asp:Button runat="server" Text="Cancel" CssClass="btn btn-default col-xs-2" CausesValidation="false"
                            OnClick="btnModalRushNo_Click" ToolTip="Uncheck the 'Rush' checkbox." />
                        <asp:Button runat="server" Text="No" CssClass="btn btn-default col-xs-offset-1 col-xs-2" CausesValidation="false"
                            OnClick="btnModalRushNo_Click" ToolTip="Uncheck the 'Rush' checkbox." />
                        <asp:Button runat="server" Text="Yes" CssClass="btn btn-primary col-xs-offset-1 col-xs-2"  data-dismiss="modal" CausesValidation="false"
                            ToolTip="Save the request, then add a new entity or person." />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--Modal dialog box to tollboth our path to the Save function--%>

    <div id="divModalSave" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h3 class="modal-title text-warning" style="text-align: center">Confirmation</h3>
                </div>
                <div class="modal-body">
                    <h4>This operation will only Save the current request,</h4>
                    <h4>not Submit it for approval. </h4>
                    <br />
                    <h4>Are you sure you want to proceed?</h4>
                </div>
                <div class="modal-footer">
                    <div class="row col-xs-12">
                        <asp:Button runat="server" Text="Cancel" CssClass="btn btn-danger col-xs-2" data-dismiss="modal" CausesValidation="false"
                            ToolTip="Close this window without saving." />
                        <asp:Button runat="server" Text="Save" CssClass="btn btn-warning col-xs-offset-1 col-xs-2" CausesValidation="false" 
                            OnClick="btnSave1_Click" ToolTip="Save this Expense Request and return to the Dashboard" />
                        <asp:Button runat="server" Text="Submit" CssClass="btn btn-success col-xs-offset-1 col-xs-2" data-dismiss="modal" CausesValidation="true" UseSubmitBehavior="false"
                            OnClick="btnSubmit_Click" ToolTip="Save this Expense Request, submit it for approval and return to the Dashboard" />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
