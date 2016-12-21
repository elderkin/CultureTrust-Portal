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
    </style>

    <div class="form-horizontal">
        <asp:Panel runat="server" DefaultButton="btnSave">
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" AssociatedControlID="rdoExpType" 
                    CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Expense Type</asp:Label>
                <div class="panel panel-default col-lg-4 col-md-5 col-sm-offset-0 col-xs-offset-1 col-xs-11">
                    <div class="radio">
                        <asp:RadioButtonList ID="rdoExpType" runat="server" AutoPostBack="true"
                            Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                            RepeatLayout="Table" RepeatDirection="Vertical" RepeatColumns="2"
                            OnSelectedIndexChanged="rdoExpType_SelectedIndexChanged">
                            <asp:ListItem Text="Paycheck" Value="Paycheck"></asp:ListItem>
                            <asp:ListItem Text="Contractor Invoice" Value="ContractorInvoice"></asp:ListItem>
                            <asp:ListItem Text="Reimbursement" Value="Reimbursement"></asp:ListItem>
                            <asp:ListItem Text="Vendor Invoice" Value="VendorInvoice"></asp:ListItem>
                            <asp:ListItem Text="Purchase Order" Value="PurchaseOrder"></asp:ListItem>
                            <asp:ListItem Text="PEX Card" Value="PEXCard"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlState" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtState" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Current Status</asp:Label>
                    <div class="col-lg-4 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtState" CssClass="form-control has-success" Enabled="false"></asp:TextBox>
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription"
                            CssClass="text-danger" ErrorMessage="Please supply a Description of the new Request." />
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBeginningDate"
                            CssClass="text-danger" ErrorMessage="Please supply a Beginning Date of the new Request." />
                        <asp:RegularExpressionValidator ID="valBeginningDate" runat="server" ControlToValidate="txtBeginningDate"
                            CssClass="text-danger" ErrorMessage="Date format is dd/mm/yyyy"
                            ValidationExpression="^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$">
                        </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtEndingDate" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Pay Period - Ending</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">

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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndingDate"
                            CssClass="text-danger" ErrorMessage="Please supply an Ending Date of the new Request." />
                        <asp:RegularExpressionValidator ID="valEndingDate" runat="server" ControlToValidate="txtEndingDate"
                            CssClass="text-danger" ErrorMessage="Date format is dd/mm/yyyy"
                            ValidationExpression="^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$">
                        </asp:RegularExpressionValidator>
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="input-group">

                            <asp:TextBox runat="server" ID="txtDateNeeded" CssClass="form-control has-success"
                                OnTextChanged="txtDateNeeded_TextChanged" AutoPostBack="true" ></asp:TextBox>
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDateNeeded"
                            CssClass="text-danger" ErrorMessage="Please supply a Date Needed of the new Request." />
                        <asp:RegularExpressionValidator ID="valDateNeeded" runat="server" ControlToValidate="txtDateNeeded"
                            CssClass="text-danger" ErrorMessage="Date format is dd/mm/yyyy"
                            ValidationExpression="^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$">
                        </asp:RegularExpressionValidator>
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox ID="txtGoodsDescription" runat="server" CssClass="form-control has-success" ></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsDescription"
                            CssClass="text-danger" ErrorMessage="Please supply a Description of Goods for the new Request." />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtGoodsSKU" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">SKU or Model Number</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox ID="txtGoodsSKU" runat="server" CssClass="form-control has-success" ></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsSKU"
                            CssClass="text-danger" ErrorMessage="Please supply a SKU or Model Number for the new Request." />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtGoodsQuantity" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Quantity</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox ID="txtGoodsQuantity" runat="server" CssClass="form-control has-success" OnTextChanged="txtGoodsCostPerUnit_TextChanged" AutoPostBack="true" ></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsQuantity"
                            CssClass="text-danger" ErrorMessage="Please supply a Quantity of the new Request." />
                        <asp:RegularExpressionValidator ID="valGoodsQuantity" runat="server" ControlToValidate="txtGoodsQuantity"
                            CssClass="text-danger" ErrorMessage="Numbers only please" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtGoodsCostPerUnit" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Cost Per Unit</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox ID="txtGoodsCostPerUnit" runat="server" CssClass="form-control has-success" OnTextChanged="txtGoodsCostPerUnit_TextChanged" AutoPostBack="true" ></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtGoodsCostPerUnit"
                            CssClass="text-danger" ErrorMessage="Please supply a Cost Per Unit of the new Request." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtGoodsCostPerUnit"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
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
                    <div class="col-lg-4 col-md-4 col-sm-offset-0 col-sm-5 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtNumberOfCards" CssClass="form-control has-success" 
                            OnTextChanged="txtNumberOfCards_TextChanged" AutoPostBack="true" Text="1"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNumberOfCards"
                            CssClass="text-danger" ErrorMessage="Numbers only please." ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
                        <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtNumberOfCards" Type="Integer"
                            CssClass="text-danger" ErrorMessage="Please supply a value between 1 and 500." MinimumValue="1" MaximumValue="500"></asp:RangeValidator>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumberOfCards"
                            CssClass="text-danger" ErrorMessage="Please supply a Number of Cards of the new Request." />
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtEachCard" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Cash Value of Each Card</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEachCard" CssClass="form-control" OnTextChanged="txtNumberOfCards_TextChanged" AutoPostBack="true"/>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEachCard"
                            CssClass="text-danger" ErrorMessage="Please supply a Cash Value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEachCard"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="input-group">
                            <asp:TextBox runat="server" ID="txtDateOfInvoice" CssClass="form-control has-success" 
                                OnTextChanged="txtDateOfInvoice_TextChanged" AutoPostBack="true"></asp:TextBox>
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDateOfInvoice"
                            CssClass="text-danger" ErrorMessage="Please supply a Date of Invoice of the new Request." />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtDateOfInvoice"
                            CssClass="text-danger" ErrorMessage="Date format is dd/mm/yyyy"
                            ValidationExpression="^([0-9]{1,2})[./-]+([0-9]{1,2})[./-]+([0-9]{2}|[0-9]{4})$">
                        </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtInvoiceNumber" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Invoice Number</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control has-success" ></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtInvoiceNumber"
                            CssClass="text-danger" ErrorMessage="Please supply an Invoice Number of the new Request." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Dollar Amount -->
        <asp:Panel ID="pnlAmount" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblAmount" AssociatedControlID="txtAmount" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Dollar Amount</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAmount"
                            CssClass="text-danger" ErrorMessage="Please supply a Dollar Amount value." />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAmount"
                            CssClass="text-danger" ErrorMessage="Currency values only. For example, $123.45"
                            ValidationExpression="^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$" />
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
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoPaymentMethod" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth" OnSelectedIndexChanged="rdoPaymentMethod_SelectedIndexChanged">
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

        <!-- Contract Questions -->
        <asp:Panel ID="pnlContractQuestions" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoContractOnFile" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Contract Questions</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ID="radRfv" ControlToValidate="rdoContractOnFile" 
                            CssClass="text-danger" ErrorMessage="Please select Yes or No"/>
                    </div>
                </div>
                <asp:Panel ID="pnlContractReason" runat="server" Visible="false">
                <div class="row">
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-2 col-xs-offset-1 col-xs-6">
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
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-2 col-xs-offset-1 col-xs-6">
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
                    <div class="col-lg-6 col-md-6 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="rdoContractComing" 
                            CssClass="text-danger" ErrorMessage="Please select Yes or No"/>
                    </div>
                </div>
            </asp:Panel>
            </div>

        </asp:Panel>

        <!-- Fulfillment Questions -->
        <asp:Panel ID="pnlPOFulFillmentInstructions" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoPOVendorMode" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Fulfillment Instructions</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlEntity" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlEntity"
                            CssClass="text-danger" ErrorMessage="Please select a Vendor from the list"></asp:RequiredFieldValidator>
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <!-- Fill this control from code-behind -->
                        <asp:DropDownList runat="server" ID="ddlPerson" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlPerson"
                            CssClass="text-danger" ErrorMessage="Please select a Person from the list"></asp:RequiredFieldValidator>
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
                    <div class="col-lg-4 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtURL" CssClass="form-control" />
                    </div>
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtURL"
                        CssClass="text-danger" ErrorMessage="URL format is incorrect"
                        ValidationExpression="^\s*((?:https?://)?(?:[\w-]+\.)+[\w-]+)(/[\w ./?%&=-]*)?\s*$" />
                </div>
            </div>
        </asp:Panel>

        <!-- GL Code -->
        <asp:Panel ID="pnlGLCode" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="ddlGLCode" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Expense Account</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:DropDownList runat="server" ID="ddlGLCode" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="col-lg-6 col-md-6 col-sm-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" InitialValue="" ControlToValidate="ddlGLCode"
                            CssClass="text-danger" ErrorMessage="Please select a General Ledger Code from the list"></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Source of Funds and Project Class -->
        <asp:Panel ID="pnlSourceOfFunds" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoSourceOfFunds" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Source of Funds</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoSourceOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                OnSelectedIndexChanged="rdoSourceOfFunds_SelectedIndexChanged">
                                <asp:ListItem Text="Unrestricted (Project Activities)" Value="Unrestricted" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Restricted (Designated Revenue)" Value="Restricted"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlProjectClass" runat="server" Visible="false">
                    <div class="row">
                        <asp:Label runat="server" AssociatedControlID="ddlProjectClass" 
                            CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Project Class</asp:Label>
                        <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                            <!-- Fill this control from code-behind -->
                            <asp:DropDownList runat="server" ID="ddlProjectClass" CssClass="form-control"></asp:DropDownList>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>

        <!-- Delivery Instructions - two flavors: PO and everybody else -->

        <asp:Panel ID="pnlPODeliveryInstructions" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoPODeliveryMode" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Instructions</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
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
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoDeliveryMode" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px;" CssClass="rdoColWidth" Visible="true"
                                OnSelectedIndexChanged="rdoDeliveryMode_SelectedIndexChanged">
                                <asp:ListItem Text="Hold for pickup." Value="Pickup" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Mail to payee." Value="MailPayee"></asp:ListItem>
                                <asp:ListItem Text="Mail to the below address." Value="MailAddress"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                        <div class="checkbox text-danger" >
                            <asp:CheckBoxList ID="cblDeliveryInstructions" runat="server" Style="margin-left: 20px; margin-bottom: 10px">
                                <asp:ListItem Text="Rush" Value="Rush" data-toggle="tooltip"
                                    title="Please process this request as quickly as possible"></asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlDeliveryAddress" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtDeliveryAddress" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Delivery Address</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDeliveryAddress" CssClass="form-control" placeholder="Delivery Address"
                            TextMode="MultiLine" Visible="true" />
                    </div>
                    <div class="col-lg-offset-1 col-lg-6 col-md-6 col-sm-4 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDeliveryAddress"
                            CssClass="text-danger" ErrorMessage="Please supply a Delivery Address value." />
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:ListBox runat="server" ID="lstSupporting" CssClass="form-control" Rows="2" SelectionMode="Single"
                            OnSelectedIndexChanged="lstSupporting_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-lg-5 col-md-offset-0 col-md-6 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                        <asp:Panel ID="pnlAdd" runat="server">
                            <asp:FileUpload ID="fupUpload" runat="server" ClientIDMode="Static" onchange="this.form.submit()"
                                CssClass="hidden" />
                            <div id="btnAdd" class="btn btn-default col-md-2 col-xs-3">Add</div>
                        </asp:Panel>
                        <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-default col-md-2 col-xs-offset-1 col-xs-3"
                            Enabled="false" OnClick="btnView_Click" CausesValidation="false" ToolTip="Download the selected Supporting Document" />
                        <asp:Button ID="btnRem" runat="server" Text="Remove" CssClass="btn btn-default col-md-2 col-xs-offset-1 col-xs-3"
                            Enabled="false" OnClick="btnRemove_Click" CausesValidation="false" ToolTip="Remove the selected Supporting Document from the Expense Request" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtNotes" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Notes</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtNotes" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
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
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtReturnNote" CssClass="form-control" TextMode="MultiLine" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- History Grid -->
        <asp:Panel ID="pnlHistory" runat="server">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">

                    <!-- Code assumes that RowID is the first column of this grid -->
                    <asp:GridView ID="EDHistoryView" runat="server"
                        CssClass="table table-striped table-hover"
                        ItemType="Portal11.Models.EDHistoryViewRow"
                        AutoGenerateColumns="false"
                        AllowPaging="true" PageSize="20"
                        OnPageIndexChanging="EDHistoryView_PageIndexChanging">

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
                                    <td>There are no History entries for this Expense Request</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:BoundField DataField="FormerStatus" HeaderText="Former Status" />
                            <asp:BoundField DataField="EstablishedBy" HeaderText="Established By" />
                            <asp:BoundField DataField="UpdatedStatus" HeaderText="Updated Status" />
                            <asp:BoundField DataField="ReasonForChange" HeaderText="Reason For Change" />
                            <asp:BoundField DataField="ReturnNote" HeaderText="Return Note" />
                        </Columns>
                    </asp:GridView>

                </div>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-sm-offset-0 col-xs-offset-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnSave_Click" CausesValidation="false" ToolTip="Save this Expense Request and return to the Dashboard" Text="Save"/>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnSubmit_Click" ToolTip="Save this Expense Request, submit it for approval and return to the Dashboard" />
            <asp:Button ID="btnRevise" runat="server" Text="Revise" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnRevise_Click" ToolTip="Make this returned Expense Request editable and begin editing it" Visible="false" />
            <asp:Button ID="btnShowHistory" runat="server" Text="Show Hist" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnShowHistory_Click" CausesValidation="false" ToolTip="List the changes that this Expense Request has gone through" Visible="false" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedEntityEnum" runat="server" Visible="false" />
        <asp:Literal ID="litSavedExpID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedPersonEnum" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectRole" runat="server" Visible="false" />
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


</asp:Content>
