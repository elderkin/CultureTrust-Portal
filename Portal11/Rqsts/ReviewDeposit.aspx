<%@ Page Title="Review Deposit Notification" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewDeposit.aspx.cs" 
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

    <style>
        .panel.col-md-3 {
            margin-bottom: 0px;
        }
    </style>

    <div class="form-horizontal">

        <!-- Project Name -->
        <asp:Panel ID="pnlProjectName" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Project Name</asp:Label>
                    <div class="col-md-3 col-xs-6">
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
                    <div class="col-md-3 col-xs-6">
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
                    <div class="col-md-3 col-xs-6">
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
                    <div class="col-md-3 col-xs-6">
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
                    <div class="col-md-3 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEstablishedTime" CssClass="form-control has-success text-right" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Description -->
        <asp:Panel ID="pnlDescription" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Description</asp:Label>
                    <div class="col-md-3 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" CssClass="form-control has-success" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Date of Deposit -->
        <asp:Panel ID="pnlDateOfDeposit" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblDateOfDeposit" CssClass="col-sm-2 col-xs-12 control-label">Dollar Amount</asp:Label>
                    <div class="col-md-3 col-xs-6">
                        <asp:TextBox runat="server" ID="txtDateOfDeposit" CssClass="form-control text-right" ReadOnly="true" />
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
                    <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="checkbox" style="padding-top:10pt; padding-bottom:10pt;">
                            <asp:CheckBoxList ID="cblOptions" runat="server" Style="margin-left: 20px" Enabled="false">
                                <asp:ListItem Text="Pledge Payment" Value="PledgePayment" ></asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Source of Funds and Project Class -->
        <asp:Panel ID="pnlSourceOfFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Source of Funds</asp:Label>
                    <div class="panel panel-default col-md-3 col-sm-5 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoSourceOfFunds" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                Enabled="false">
                                <asp:ListItem Text="Not Applicable" Value="NA"></asp:ListItem>
                                <asp:ListItem Text="Entity (EIN)" Value="Entity"></asp:ListItem>
                                <asp:ListItem Text="Individual (SSN)" Value="Individual"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>
            </div>
               

                <!-- Individual Name or Anonymous-->
                <asp:Panel ID="pnlPerson" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" ID="lblPerson" CssClass="col-sm-2 col-xs-12 control-label">Donor or Customer</asp:Label>
                            <div class="col-md-3 col-xs-6">
                                <asp:TextBox runat="server" ID="txtPerson" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlPersonAnonymous" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" ID="Label1" CssClass="col-sm-2 col-xs-12 control-label">Donor or Customer</asp:Label>
                            <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                                <div class="checkbox" style="padding-top:10pt; padding-bottom:10pt;">
                                    <asp:CheckBoxList runat="server" Style="margin-left: 20px">
                                        <asp:ListItem Text="Anonymous" Selected="True" Enabled="false"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Entity Name or Anonymous-->
                <asp:Panel ID="pnlEntity" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" ID="lblEntity" CssClass="col-sm-2 col-xs-12 control-label">Entity</asp:Label>
                            <div class="col-md-3 col-xs-6">
                                <asp:TextBox runat="server" ID="txtEntity" CssClass="form-control" ReadOnly="true" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlEntityAnonymous" runat="server" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Entity</asp:Label>
                            <div class="panel panel-default col-md-3 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                                <div class="checkbox" style="padding-top:10pt; padding-bottom:10pt;">
                                    <asp:CheckBoxList runat="server" Style="margin-left: 20px">
                                        <asp:ListItem Text="Anonymous" Selected="True" Enabled="false"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

        </asp:Panel>

        <!-- Dest of Funds and Project Class -->
        <asp:Panel ID="pnlDestOfFunds" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Destination of Funds</asp:Label>
                    <div class="panel panel-default col-md-3 col-sm-5 col-xs-6">
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
                        <div class="col-md-3 col-xs-6">
                            <asp:TextBox runat="server" ID="txtProjectClass" CssClass="form-control" ReadOnly="true" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </asp:Panel>

        <!-- Dollar Amount -->
        <asp:Panel ID="pnlAmount" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" ID="lblAmount" CssClass="col-sm-2 col-xs-12 control-label">Dollar Amount</asp:Label>
                    <div class="col-md-3 col-xs-6">
                        <asp:TextBox runat="server" ID="txtAmount" CssClass="form-control text-right" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- GL Code -->
        <asp:Panel ID="pnlGLCode" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">General Ledger Code</asp:Label>
                    <div class="col-md-3 col-xs-6">
                        <asp:TextBox runat="server" ID="txtGLCode" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- GL Code Split -->
        <asp:Panel ID="pnlDepositSplit" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Account Splits</asp:Label>
                    <div class="col-sm-10 col-xs-offset-0 col-xs-12">
 
                        <div class="col-xs-12" style="padding-left:0">
                        <asp:GridView ID="gvDepSplit" runat="server"
                            CssClass="table table-striped table-hover"
                            ItemType="Portal11.Models.rowGLCodeSplit"
                            AutoGenerateColumns="false"
                            AllowPaging="false">

                            <SelectedRowStyle CssClass="success" />

                            <EmptyDataTemplate>
                                <table>
                                    <tr>
                                        <td>There are no Splits for this Request</td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Project Class">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitProjectClass" runat="server" CssClass="form-control" 
                                            Text='<%# Bind("SelectedProjectClassID") %>' Enabled="false"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Account">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitGLCode" runat="server" CssClass="form-control" 
                                            Text='<%# Bind("SelectedGLCodeID") %>' Enabled="false"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dollar Amount" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" >
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitAmount" runat="server" CssClass="form-control" 
                                            Text='<%# Bind("Amount") %>' style="text-align:right" Enabled="false"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Note" HeaderStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtSplitNote" runat="server" CssClass="form-control"  
                                            Text='<%# Bind("Note") %>' Enabled="false"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Supporting Docs -->
        <asp:Panel ID="pnlSupporting" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Supporting Docs</asp:Label>
                    <div class="col-md-3 col-sm-5 col-xs-6">
                        <asp:ListBox runat="server" ID="lstSupporting" CssClass="form-control" Rows="2" SelectionMode="Single"
                            OnSelectedIndexChanged="lstSupporting_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-lg-5 col-md-offset-0 col-md-6 col-sm-offset-2 col-xs-offset-1 col-xs-6">
                    <asp:HyperLink ID="btnViewLink" runat="server" CssClass="btn btn-default col-md-2 col-xs-3" Enabled="false" ToolTip="Select a row then click here to view the document"
                        NavigateUrl="overwrite from code behind" Text="View" Target="_blank" />
                    <asp:Button ID="btnZip" runat="server" Text="Zip" CssClass="btn btn-default col-md-2 col-xs-offset-1 col-xs-3" CausesValidation="false" 
                        Enabled="true" OnClick="btnZip_Click" ToolTip="Download a Zip file containing all supporting documents" />
                    </div>
                    <div class="col-xs-6 text-success">
                        <asp:Literal runat="server" ID="litSDSuccess" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Creator Note</asp:Label>
                    <div class="col-xs-5">
                        <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control has-success" TextMode="MultiLine" Rows="6" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Return Note -->
        <asp:Panel ID="pnlReturnNote" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtReturnNote" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Return Note</asp:Label>
                    <div class="col-xs-5">
                        <asp:TextBox runat="server" ID="txtReturnNote" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                    </div>
                    <div class="col-xs-5 ">
                        <asp:Button ID="btnReturnNoteClear" runat="server" Text="Clear" CssClass="btn btn-default col-md-2 col-xs-3"
                            Enabled="true" OnClick="btnReturnNoteClear_Click" CausesValidation="false" ToolTip="Clear the text of the Return Note" />
                        <div class="text-danger col-xs-12">
                            <br />
                            <asp:Literal ID="litReturnNoteError" runat="server" Visible="false" />
                        </div>
                    </div>
<%--                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtReturnNote"
                        CssClass="text-danger" ErrorMessage="Please supply a reason why you are returning the request." />--%>
                </div>
            </div>
        </asp:Panel>

        <!-- Staff Note -->
        <asp:Panel ID="pnlStaffNote" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtStaffNote" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">
                        Staff Note<br />(visible only to other staff)</asp:Label>
                    <div class="col-xs-5">
                        <asp:TextBox runat="server" ID="txtStaffNote" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                    </div>
                    <div class="col-xs-5 ">
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
                            CommandArgument="Last" CausesValidation="false"  Enabled="false"
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
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnApprove_Click" CausesValidation="false" ToolTip="Advance this Deposit Request to the next stage" />
            <asp:Button ID="btnReturn" runat="server" Text="Return" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnReturn_Click" ToolTip="Disapprove this Deposit Request and return it to the Project Director with a Return Note" />
            <asp:Button ID="btnRevise" runat="server" Text="Revise" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnRevise_Click" ToolTip="Edit this Deposit Request to fix problems and return it to the prior reviewer with a Return Note" />
            <asp:Button ID="btnHistory" runat="server" Text="History" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnHistory_Click" CausesValidation="false" ToolTip="View the history of this Deposit Request" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDepID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedDepType" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectName" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectCode" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedState" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserRole" runat="server" Visible="false" />

    </div>


</asp:Content>
