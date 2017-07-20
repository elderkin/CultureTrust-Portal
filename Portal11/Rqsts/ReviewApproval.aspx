<%@ Page Title="Review Approval Request" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewApproval.aspx.cs" 
    Inherits="Portal11.Rqsts.ReviewApproval" MaintainScrollPositionOnPostback="true" %>
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

        <!-- Approval Type -->
        <asp:Panel ID="pnlType" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Approval Type</asp:Label>
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

        <!-- Review Type -->
        <asp:Panel ID="pnlReviewType" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Review Type</asp:Label>
                    <div class="panel panel-default col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <div class="radio">
                            <asp:RadioButtonList ID="rdoReviewType" runat="server" AutoPostBack="true"
                                Style="margin-left: 20px; margin-bottom: 10px;" CssClass="rdoColWidth"
                                Enabled="true">
                                <%--Code assumes that the following three list items are in this order--%>
                                <asp:ListItem Text="IC Only - Review only by Internal Coordinator" Value="ICOnly" Enabled="false"></asp:ListItem>
                                <asp:ListItem Text="Express - no Financial or Executive Review" Value="Express"></asp:ListItem>
                                <asp:ListItem Text="Full - include Financial and Executive Review" Value="Full"></asp:ListItem>
                            </asp:RadioButtonList>
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
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:ListBox runat="server" ID="lstSupporting" CssClass="form-control" Rows="2" SelectionMode="Single"
                            OnSelectedIndexChanged="lstSupporting_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
<%--                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-default col-xs-1"
                        Enabled="false" OnClick="btnView_Click" CausesValidation="false" ToolTip="Download the selected Supporting Document" />--%>
                    <asp:HyperLink ID="btnViewLink" runat="server" CssClass="btn btn-default col-xs-1" Enabled="false" ToolTip="Select a row then click here to view the document"
                        NavigateUrl="overwrite from code behind" Text="View" Target="_blank" />
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" CssClass="col-sm-2 col-xs-12 control-label">Creator Note</asp:Label>
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
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtReturnNote"
                        CssClass="text-danger" ErrorMessage="Please supply a reason why you are returning the request." />
                </div>
            </div>
        </asp:Panel>

        <!-- Staff Note -->
        <asp:Panel ID="pnlStaffNote" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtStaffNote" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">
                        Staff Note<br />(visible only to other staff)</asp:Label>
                    <div class="col-lg-3 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtStaffNote" CssClass="form-control" TextMode="MultiLine" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- History Grid -->
        <asp:Panel ID="pnlHistory" runat="server">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">

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
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnCancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnApprove_Click" CausesValidation="false" ToolTip="Advance this Approval Request to the next stage" />
            <asp:Button ID="btnReturn" runat="server" Text="Return" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnReturn_Click" ToolTip="Disapprove this Approval Request and return it to the Project Director with a Review Note" />
            <asp:Button ID="btnHistory" runat="server" Text="History" CssClass="btn btn-default col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="btnHistory_Click" CausesValidation="false" ToolTip="View the history of this Approval Request" />
        </div>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedAppID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedProjectID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReviewType" runat="server" Visible="false" />
        <asp:Literal ID="litSavedState" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserRole" runat="server" Visible="false" />

    </div>


</asp:Content>
