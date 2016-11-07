<%@ Page Title="Review Expense Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewExpense.aspx.cs" 
    Inherits="Portal11.Rqsts.ReviewExpense" MaintainScrollPositionOnPostback="true" %>
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

        <!-- Project Name -->
        <asp:Panel ID="pnlProjectName" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Project Name</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtProjectName" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Expense Type -->
        <asp:Panel ID="pnlType" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Expense Type</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtTypeDescription" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Current Status -->
        <asp:Panel ID="pnlState" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Current Status</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtStateDescription" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Established by -->
        <asp:Panel ID="pnlEstablishedBy" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Established by</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEstablishedBy" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Established on -->
        <asp:Panel ID="pnlEstablishedTime" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Established on</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEstablishedTime" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Description -->
        <asp:Panel ID="pnlDescription" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Description</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Beginning/Ending Date -->
        <asp:Panel ID="pnlBeginningEnding" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Pay Period - Beginning</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtBeginningDate" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Pay Period - Ending</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEndingDate" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Date Needed -->
        <asp:Panel ID="pnlDateNeeded" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Date Needed</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDateNeeded" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Goods to Purchase-->
        <asp:Panel ID="pnlGoods" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Description of Goods</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtGoodsDescription" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">SKU or Model Number</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtGoodsSKU" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Quantity</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtGoodsQuantity" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Cost Per Unit</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtGoodsCostPerUnit" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Cards -->
        <asp:Panel ID="pnlCards" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Number of Cards</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtNumberOfCards" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Cash Value of Each Card</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtEachCard" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Date of Invoice -->
        <asp:Panel ID="pnlDateOfInvoice" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Date of Invoice</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtDateOfInvoice" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Invoice Number</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Dollar Amount -->
        <asp:Panel ID="pnlAmount" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblAmount" CssClass="col-sm-2 col-xs-12 control-label">Dollar Amount</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Payment Method -->
        <asp:Panel ID="pnlPaymentMethod" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Payment Method</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtPaymentMethod" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Contract Questions -->
        <asp:Panel ID="pnlContractQuestions" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Existing Contract?</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtExistingContract" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Contract Reason</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtContractReason" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Fulfillment Questions -->
        <asp:Panel ID="pnlPOFulfillmentInstructions" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Fulfillment Instructions</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtPOVendorMode" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Entity Name -->
        <asp:Panel ID="pnlEntity" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label ID="lblEntity" runat="server" CssClass="col-sm-2 col-xs-12 control-label">Vendor</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEntity" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Person Name -->
        <asp:Panel ID="pnlPerson" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblPerson" CssClass="col-sm-2 col-xs-12 control-label">Person</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtPerson" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- URL -->
        <asp:Panel ID="pnlURL" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">URL</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtURL" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- GL Code -->
        <asp:Panel ID="pnlGLCode" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">General Ledger Code</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtGLCode" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Source of Funds and Project Class -->
        <asp:Panel ID="pnlFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Source of Funds</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoSourceOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                Enabled="false">
                                <asp:ListItem Text="Not Applicable" Value="NA"></asp:ListItem>
                                <asp:ListItem Text="Unrestricted (Project Activities)" Value="Unrestricted"></asp:ListItem>
                                <asp:ListItem Text="Restricted (Designated Revenue)" Value="Restricted"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlProjectClass" runat="server">
                    <div class="row">
                        <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Project Class</asp:Label>
                        <div class="col-lg-4 col-md-4 col-xs-6">
                            <asp:TextBox runat="server" ID="txtProjectClass" CssClass="form-control" ReadOnly="true" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>
        <! -- End of pnlFunds -->

        <!-- Delivery Instructions - shared by PO and everybody else -->
        <asp:Panel ID="pnlDeliveryInstructions" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Delivery Instructions</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtDeliveryInstructions" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:panel>

        <asp:Panel ID="pnlPODeliveryInstructions" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Delivery Instructions</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtPODeliveryInstructions" runat="server" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlDeliveryAddress" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Delivery Address</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtDeliveryAddress" runat="server" CssClass="form-control has-success" TextMode="MultiLine" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Supporting Docs -->
        <asp:Panel ID="pnlSupporting" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Supporting Docs</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:ListBox runat="server" ID="lstSupporting" CssClass="form-control" Rows="2" SelectionMode="Single"
                            OnSelectedIndexChanged="lstSupporting_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-default col-xs-1"
                        Enabled="false" OnClick="btnView_Click" CausesValidation="false" ToolTip="Download the selected Supporting Document" />
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Notes</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control has-success" TextMode="MultiLine" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Return Note -->
        <asp:Panel ID="pnlReturnNote" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtReturnNote" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Return Note</asp:Label>
                    <div class="col-lg-3 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtReturnNote" CssClass="form-control" TextMode="MultiLine" />
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
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnApprove_Click" ToolTip="Advance this Expense Request to the next stage" />
            <asp:Button ID="btnReturn" runat="server" Text="Return" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnReturn_Click" ToolTip="Disapprove this Expense Request and return it to the Project Director with a Review Note" />
            <asp:Button ID="btnHistory" runat="server" Text="Show Hist" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnHistory_Click" ToolTip="View the history of this Expense Request" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedExpID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedState" runat="server" Visible="false" />

    </div>

</asp:Content>
