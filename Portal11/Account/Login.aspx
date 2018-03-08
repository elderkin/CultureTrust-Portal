<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" 
    Inherits="Portal11.Account.Login1" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .centered_div {
            width: 600px;
            height: 700px;
            position: absolute;
            top: 50%;
            left: 50%;
            margin-left: -300px;
            margin-top: -200px;
        }
      /*.panel-success .panel-heading {
          background-color:#6F7CFC;
 }*/
    </style>
    <asp:Panel runat="server" BackImageUrl="~/Images/LoginBackground.jpg" Height="100%" Width="100%">
        <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; 
        <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; 
        <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; 
        <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp; <br /> &nbsp;
    </asp:Panel>
    <div class="centered_div panel panel-success">
        <div class="panel-heading">
            <h3 class="panel-title">Welcome to the CultureTrust Portal</h3>
        </div>
        <div class="panel-body col-xs-12">

            <section id="loginForm">
                <div class="form-horizontal">
                    <h3>Please Login</h3>

                    <p class="text-success">
                        <asp:Literal runat="server" ID="litSuccessMessage" />
                    </p>
                    <p class="text-danger">
                        <asp:Literal runat="server" ID="litDangerMessage" />
                    </p>
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-offset-0 col-md-3 col-xs-offset-1 control-label">Email</asp:Label>
                            <div class="col-md-offset-0 col-md-7 col-xs-offset-1 col-xs-11 ">
                                <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                            </div>
                            <div class="col-md-offset-3 col-xs-offset-1 col-xs-11 ">
                                <asp:RequiredFieldValidator 
                                    runat="server" 
                                    ControlToValidate="Email"
                                    CssClass="text-danger" 
                                    ErrorMessage="The email field is required." 
                                    />
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-offset-0 col-md-3 col-xs-offset-1 control-label">Password</asp:Label>
                            <div class="col-md-offset-0 col-md-7 col-xs-offset-1 col-xs-11">
                                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            </div>
                            <div class="col-md-offset-2 col-xs-offset-1 col-xs-11">
                                <asp:RequiredFieldValidator 
                                    runat="server" 
                                    ControlToValidate="Password"
                                    CssClass="text-danger" 
                                    ErrorMessage="The password field is required." 
                                    />
                            </div>
                        </div>
                    </div>


                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-offset-3 col-xs-offset-1 col-xs-2">
                                <asp:Button runat="server" OnClick="LogIn" Text="Log in" CssClass="btn btn-primary" />
                            </div>
                             <div class="col-xs-offset-1 col-xs-3">
                                <asp:Button runat="server" ID="btnNew" OnClick="btnNew_Click" Text="What's New" CssClass="btn btn-default" CausesValidation="false" />
                            </div>
                        </div>
                    </div>

                    <asp:Panel runat="server" ID="pnlRememberMe" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-offset-4 col-xs-offset-1 col-xs-4">
                                <div class="checkbox col-xs-2">
                                    <asp:CheckBox runat="server" ID="RememberMe" />
                                    <asp:Label runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    </asp:Panel>

                   <asp:Panel runat="server" ID="pnlRememberEmail" Visible="true">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-md-offset-3 col-xs-offset-1 col-xs-8">
                                <div class="checkbox col-xs-12">
                                    <asp:CheckBox runat="server" ID="ckRememberEmail" />
                                    <asp:Label runat="server" AssociatedControlID="ckRememberEmail">Remember email?</asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    </asp:Panel>

                    <asp:Panel runat="server" ID="pnlVersion" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <asp:Label runat="server" AssociatedControlID="txtVersion" CssClass="col-md-offset-0 col-md-3 col-xs-offset-1 control-label">Version</asp:Label>
                            <div class="col-md-offset-0 col-md-6 col-xs-offset-1 col-xs-11">
                                <asp:TextBox runat="server" ID="txtVersion" ReadOnly="true" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="row">
                            <asp:Label runat="server" AssociatedControlID="txtBuild" CssClass="col-md-offset-0 col-md-3 col-xs-offset-1 control-label">Build</asp:Label>
                            <div class="col-md-offset-0 col-md-6 col-xs-offset-1 col-xs-11">
                                <asp:TextBox runat="server" ID="txtBuild" ReadOnly="true" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    </asp:Panel>

                    <asp:Panel runat="server" ID="pnlNew" Visible="false">
                    <div class="form-group">
                        <div class="row">
                            <div class="col-xs-offset-1 col-xs-10">
                                <asp:TextBox runat="server" ID="txtNew" TextMode="MultiLine" Rows="5" CssClass="form-control" Wrap="true" />
                            </div>
                        </div>
                    </div>
                    </asp:Panel>

                </div>
                <br />&nbsp;

                <asp:Panel runat="server" ID="pnlFirst">
                <div >
                    <asp:ImageButton ID="imgFirst" runat="server" class="img-responsive center-block" ImageUrl="~/Images/LoginIcon.jpg"
                        Height="100" Width="426" OnClick="imgFirst_Click" CausesValidation="false" />
                </div>
                </asp:Panel>
<%--            <p>
                    <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled">Temporary: Register as a new user</asp:HyperLink>
                </p>--%>
                    <%-- Enable this once you have account confirmation enabled for password reset functionality
                    <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled">Forgot your password?</asp:HyperLink>
                    --%>
            </section>
        </div>
    </div>


</asp:Content>
