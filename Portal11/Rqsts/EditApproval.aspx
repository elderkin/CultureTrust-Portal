<%@ Page Title="Edit Approval" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditApproval.aspx.cs" 
    Inherits="Portal11.Rqsts.EditApproval" MaintainScrollPositionOnPostback="true" %>
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
        .panel.col-lg-3 {
            margin-bottom: 0px;
        }
    </style>

    <div class="form-horizontal">
        <asp:Panel runat="server" DefaultButton="btnSave">
        <div class="form-group">
            <div class="row">
                <asp:Label runat="server" AssociatedControlID="rdoAppType" 
                    CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Approval Type</asp:Label>
                <div class="panel panel-default col-lg-4 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-11">
                    <div class="radio">
                        <asp:RadioButtonList ID="rdoAppType" runat="server" AutoPostBack="true"
                            Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                            RepeatLayout="Table" RepeatDirection="Vertical" RepeatColumns="2"
                            OnSelectedIndexChanged="rdoAppType_SelectedIndexChanged">
                            <asp:ListItem Text="Contract/Proposal of Work" Value="Contract"></asp:ListItem>
                            <asp:ListItem Text="Grant Proposal/Fundraising Materials" Value="Grant"></asp:ListItem>
                            <asp:ListItem Text="Marketing/Fundraising Campaign Information" Value="Campaign"></asp:ListItem>
                            <asp:ListItem Text="Certificate of Insurance Request" Value="Certificate"></asp:ListItem>
                            <asp:ListItem Text="Financial Report/Analysis Request" Value="Report"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>

        <!-- Current Status -->
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


        <!-- Review Type -->
        <asp:Panel ID="pnlReviewType" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="rdoReviewType" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">Review Type</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoReviewType" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                            
                                OnSelectedIndexChanged="rdoReviewType_SelectedIndexChanged">
                                <%--Code assumes that the following three list items are in this order--%>
                                <asp:ListItem Text="IC Only - Review only by Internal Coordinator" Value="ICOnly" Enabled="false"></asp:ListItem>
                                <asp:ListItem Text="Express - no Financial or Executive Review" Value="Express"></asp:ListItem>
                                <asp:ListItem Text="Full - include Financial and Executive Review" Value="Full" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
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

        <!-- Staff Note -->
        <asp:Panel ID="pnlStaffNote" runat="server" Visible="false">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtStaffNote" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label">
                            Staff Note<br />(visible only to other staff)</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-offset-0 col-xs-offset-1 col-xs-6">
                        <asp:TextBox runat="server" ID="txtStaffNote" CssClass="form-control" TextMode="MultiLine" />
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
                                    <td>There are no History entries for this Approval Request</td>
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
                OnClick="btnSave_Click" ToolTip="Save this Approval Request and return to the Dashboard" Text="Save" CausesValidation="false"/>
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnSubmit_Click" ToolTip="Save this Approval Request, submit it for approval and return to the Dashboard" />
            <asp:Button ID="btnRevise" runat="server" Text="Revise" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnRevise_Click" ToolTip="Make this returned Approval Request editable and begin editing it" Visible="false" />
            <asp:Button ID="btnShowHistory" runat="server" Text="History" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnShowHistory_Click" ToolTip="List the changes that this Approval Request has gone through" Visible="false" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedAppID" runat="server" Visible="false" />
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
