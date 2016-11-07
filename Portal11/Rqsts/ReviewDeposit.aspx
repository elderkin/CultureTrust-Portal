<%@ Page Title="Review Deposit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewDeposit.aspx.cs" 
    Inherits="Portal11.Rqsts.ReviewDeposit" MaintainScrollPositionOnPostback="true" %>
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

        <!-- Deposit Type -->
        <asp:Panel ID="pnlType" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Deposit Type</asp:Label>
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

        <!-- Date of Deposit -->
        <asp:Panel ID="pnlDateOfDeposit" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblDateOfDeposit" CssClass="col-sm-2 col-xs-12 control-label">Dollar Amount</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDateOfDeposit" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Options -->
        <asp:Panel ID="pnlOptions" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="cblOptions" CssClass="col-sm-2 col-xs-12 control-label"
                        Font-Bold="false">Options</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="checkbox" style="padding-top:10pt; padding-bottom:10pt">
                            <asp:CheckBoxList ID="cblOptions" runat="server" Style="margin-left: 20px" Enabled="false">
                                <asp:ListItem Text="Pledge Payment" Value="PledgePayment" ></asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
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
        <asp:Panel ID="pnlSourceOfFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Source of Funds</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoSourceOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                Enabled="false">
                                <asp:ListItem Text="Not Applicable" Value="NA"></asp:ListItem>
                                <asp:ListItem Text="Anonymous" Value="Anonymous"></asp:ListItem>
                                <asp:ListItem Text="Entity (EIN)" Value="Entity"></asp:ListItem>
                                <asp:ListItem Text="Individual (SSN)" Value="Individual"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>

                <!-- Individual Name -->
                <asp:Panel ID="pnlPerson" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" ID="lblPerson" CssClass="col-sm-2 col-xs-12 control-label">Donor or Customer</asp:Label>
                            <div class="col-lg-4 col-md-4 col-xs-6">
                                <asp:TextBox runat="server" ID="txtPerson" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Entity Name -->
                <asp:Panel ID="pnlEntity" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" ID="lblEntity" CssClass="col-sm-2 col-xs-12 control-label">Entity</asp:Label>
                            <div class="col-lg-4 col-md-4 col-xs-6">
                                <asp:TextBox runat="server" ID="txtEntity" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

            </div>
        </asp:Panel>
        <! -- End of pnlFunds -->

        <!-- Dest of Funds and Project Class -->
        <asp:Panel ID="pnlDestOfFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Destination of Funds</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoDestOfFunds" runat="server" AutoPostBack="true"
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
                                    <td>There are no History entries for this Deposit Request</td>
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
                OnClick="btnApprove_Click" ToolTip="Advance this Deposit Request to the next stage" />
            <asp:Button ID="btnReturn" runat="server" Text="Return" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnReturn_Click" ToolTip="Disapprove this Deposit Request and return it to the Project Director with a Review Note" />
            <asp:Button ID="btnHistory" runat="server" Text="Show Hist" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnHistory_Click" ToolTip="View the history of this Deposit Request" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDepID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedState" runat="server" Visible="false" />

    </div>


</asp:Content>
