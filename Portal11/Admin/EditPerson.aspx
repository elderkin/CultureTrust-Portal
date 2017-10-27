<%@ Page Title="Edit Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditPerson.aspx.cs" 
    Inherits="Portal11.Admin.EditPerson" MaintainScrollPositionOnPostback="true" %>
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
                    <asp:Label runat="server" AssociatedControlID="txtName" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="true">Name</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                            CssClass="text-danger" ErrorMessage="Please supply a Person Name." />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlInactive" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkInact" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">Person is Inactive</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkInact" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="Turn the Person off, disabling its use. Note: This action is difficult to undo!" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlAddress" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtAddress" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Address</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtAddress" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlPhone" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtPhone" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Phone Number</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" />
                       </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RegularExpressionValidator ID="valPhone" runat="server" CssClass="text-danger" ErrorMessage="Please enter valid Phone Number" 
                            ControlToValidate="txtPhone" ValidationExpression= "^([0-9\(\)\/\+ \-]*)$">
                            </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlEmail" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtEmail" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Email</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" />
                    </div>
                    <div class="col-lg-6 col-md-6 col-xs-6">
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" CssClass="text-danger" ErrorMessage="Please enter valid Email address" 
                            ControlToValidate="txtEmail" ValidationExpression= "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$">
                            </asp:RegularExpressionValidator>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Supporting Docs -->
        <asp:Panel ID="pnlSupporting" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="lstSupporting" 
                        CssClass="col-sm-offset-0 col-sm-2 col-xs-offset-1 col-xs-11 control-label" Font-Bold="false">Supporting Docs</asp:Label>
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

        <!-- W-9 Present --> 
        <asp:Panel ID="pnlW9Present" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="chkW9Present" CssClass="col-sm-2 col-xs-10 control-label" Font-Bold="false">W-9 Form Present</asp:Label>
                    <div class="col-lg-4 col-md-4 col-xs-6">
                        <asp:CheckBox ID="chkW9Present" runat="server" CssClass="checkbox col-offset-xs-1 col-xs-1"
                            ToolTip="A W-9 form is present for this Person" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Notes -->
        <asp:Panel ID="pnlNotes" runat="server">
            <div class="form-group">
                <div class="row">
                    <asp:Label runat="server" AssociatedControlID="txtNotes" CssClass="col-sm-2 col-xs-12 control-label" Font-Bold="false">Notes</asp:Label>
                    <div class="col-lg-3 col-md-4 col-sm-5 col-xs-6">
                        <asp:TextBox runat="server" ID="txtNotes" TextMode="MultiLine" CssClass="form-control has-success"></asp:TextBox>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Button array -->
        <div class="row">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-default col-sm-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Cancel_Click" CausesValidation="false" ToolTip="Return to the Dashboard without saving" />
            <asp:Button ID="btnSave" runat="server" CssClass="btn btn-primary col-xs-offset-1 col-md-1 col-xs-2" Enabled="true"
                OnClick="Save_Click" ToolTip="Save this Person and return to main page" Text="Save" />
        </div>
        </asp:Panel>

        <!-- "Scratch" storage used during form processing -->
        <asp:Literal ID="litSavedCommand" runat="server" Visible="false" />
        <asp:Literal ID="litSavedPersonID" runat="server" Visible="false" />
        <asp:Literal ID="litSavedReturn" runat="server" Visible="false" />
        <asp:Literal ID="litSavedUserID" runat="server" Visible="false" />

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
