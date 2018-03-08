<%@ Page Title="New Portal User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" 
    Inherits="Portal11.Account.Register" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">

        <asp:Panel runat="server" ID="pnlFranchise" Visible="false">
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtFranchiseKey" CssClass="col-md-2 col-sm-2 control-label">Franchise</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="txtFranchiseKey" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFranchiseKey" SetFocusOnError="true"
                    CssClass="text-danger" ErrorMessage="The Franchise Key field is required." />
            </div>
        </div>
        </asp:Panel>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Email</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" SetFocusOnError="true"
                    CssClass="text-danger" ErrorMessage="The Email field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Password</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" SetFocusOnError="true"
                    CssClass="text-danger" ErrorMessage="The password field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Confirm password</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword" SetFocusOnError="true"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="FullName" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="true">Name</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="FullName" CssClass="form-control" />
            </div>
            <div class="col-md-7">
                <asp:RequiredFieldValidator runat="server" ControlToValidate="FullName" SetFocusOnError="true"
                    CssClass="text-danger" Display="Dynamic" ErrorMessage="The name field is required." />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Address" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="false">Address</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" TextMode="MultiLine" ID="Address" CssClass="form-control" />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="PhoneNumber" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="false">Phone Number</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="PhoneNumber" CssClass="form-control " />
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="txtGridViewRows" CssClass="col-md-2 col-sm-2 control-label" Font-Bold="false">Rows in Each Grid</asp:Label>
            <div class="col-md-3 col-sm-6 col-xs-12">
                <asp:TextBox runat="server" ID="txtGridViewRows" CssClass="form-control " ToolTip="On every web page of the Portal, include a maximum of this many rows in each table (or grid)." />
            </div>
           <div class="col-md-7">
                <asp:RangeValidator runat="server" Type="Integer" 
                MinimumValue="0" MaximumValue="100" ControlToValidate="txtGridViewRows" 
                CssClass="text-danger" Display="Dynamic" ErrorMessage="Value must be a whole number between 0 and 100" />
           </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="cblOptions" CssClass="col-md-2 col-sm-2 col-xs-12 control-label" Font-Bold="false">Options</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-6">
                <div class="well">
                    <div class="checkbox" style="padding-top: 0">
                        <asp:CheckBoxList ID="cblOptions" runat="server" Style="margin-left: 20px">
                            <asp:ListItem Text="System Administrator" Value="Administrator" data-toggle="tooltip"
                                title="Create, edit and delete systems structures and user accounts"></asp:ListItem>
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="rblRole" CssClass="col-md-2 col-sm-2 col-xs-12 control-label" Font-Bold="false">CT Staff Role</asp:Label>
            <div class="col-md-3 col-sm-5 col-xs-6">
                <div class="well">
                    <div class="checkbox" style="padding-top: 0">
                        <asp:RadioButtonList ID="rblRole" runat="server" >
                            <asp:ListItem Text="Not a Staff Member" Value="" Selected="True" data-toggle="tooltip"
                                title="The User is a Project Director or Project Staff, not a CW staff member"></asp:ListItem>
                            <asp:ListItem Text="Auditor" Value="Auditor" data-toggle="tooltip"
                                title="Examines, but does not change, Requests"></asp:ListItem>
                            <asp:ListItem Text="Community Director" Value="CommunityDirector" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                            <asp:ListItem Text="Finance Director" Value="FinanceDirector" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                            <asp:ListItem Text="InternalCoordinator" Value="InternalCoordinator" data-toggle="tooltip"
                                title="Creates Requests"></asp:ListItem>
                            <asp:ListItem Text="President" Value="President" data-toggle="tooltip"
                                title="Reviews and Approves Requests"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="row">
               <div class="col-md-offset-2 col-md-1 col-xs-2">
                    <asp:Button runat="server" OnClick="CancelUser_Click" Text="Cancel" CssClass="btn btn-default" CausesValidation="false" />
                </div>

                <div class="col-md-offset-1 col-md-1 col-xs-offset-1 col-xs-2">
                    <asp:Button runat="server" OnClick="CreateUser_Click" Text="Register" CssClass="btn btn-primary" />
                </div>

            </div>
        </div>
    </div>
</asp:Content>
