<%@ Page Title="System Administration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Main.aspx.cs" Inherits="Portal11.Admin.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <hr />

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
<%--    <div class="form-horizontal">--%>
    <div class="row">
        <div class="col-xs-4">

            <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Entity</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditEntity?Command=New"  title="Create and describe a new Entity">New Entity</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectEntity?Command=Edit" title="Choose an existing Entity and change its description">Edit Entity</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignEntitys" 
                                title="Choose an existing Project and assign Entitys to it">Assign Entitys to Project</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

            <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Error Log</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/elmah.axd" title="Open the web page for the ELMAH error log">View Error Log</a>
                        </li>
                    </ul>
                </div>
            </div>
            </div>

        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">GL Code</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGLCode.aspx?Command=New" title="Create and describe a new GL Code">New GL Code</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGLCode.aspx?Command=Edit" title="Choose an existing GL Code and change its description">Edit GL Code</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

    </div>
    <div class="col-xs-4">

<%--        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Grant</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGrant.aspx?Command=New" title="Create and describe a new Grant from an existing Grant Maker">New Grant</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGrant.aspx?Command=Edit" title="Choose an existing Grant and change its description">Edit Grant</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignGrant" title="Choose a Project and change which existing Grants are available to it">Assign Grant to Project</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Grant Maker</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGrantMaker.aspx?Command=New" title="Create and describe a new Grant Maker">New Grant Maker - Future</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditGrantMaker.aspx?Command=New" title="Choose an existing Grant Maker and change its description">Edit Grant Maker - Future</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>--%>

            <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Person</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditPerson.aspx?Command=New" title="Create and describe a new Person">New Person</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectPerson.aspx?Command=Edit" 
                                title="Choose an existing Person and change its description">Edit Person</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignPersons" 
                                title="Choose an existing Project and assign Persons to it">Assign Persons to Project</a>
                        </li>
                    </ul>
                </div>
            </div>
            </div>

        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Project</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditProject.aspx?Command=New" title="Create and describe a new Project" >New Project</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=Edit" title="Choose an existing Project and change its description">Edit Project</a>
                        </li>
<%--                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignGrant" 
                                title="Choose a Project and change which existing Grants are available to it">Assign Grant to Project</a>
                        </li>--%>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignEntitys" 
                                title="Choose an existing Project and assign Entitys to it">Assign Entitys to Project</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectProject.aspx?Command=AssignPersons" 
                                title="Choose an existing Project and assign Persons to it">Assign Persons to Project</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectUser.aspx?Command=Assign" 
                                title="Choose a Portal User and change their role (Project Director or Project Staff) on existing Projects">Assign Portal User to Project</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

    </div>
    <div class="col-xs-4">

        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Portal User</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Account/Register.aspx" title="Create a new User with Email and Password" >New Portal User</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectUser.aspx?Command=Edit" title="Choose an existing User and change its description - except for password">Edit Portal User</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectUser.aspx?Command=ChangePassword" title="Choose an existing User and change its password">Change Portal User Password</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectUser.aspx?Command=Assign" 
                                title="Choose a User and change their role (Project Director or Project Staff) on existing Projects">Assign Portal User to Project</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

<%--        <div class="col-xs-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Vendor (for Expenses)</h3>
                </div>
                <div class="panel-body">
                    <ul class="list-group">
                        <li class="list-group-item">
                            <a runat="server" href="~/Admin/EditVendor?Command=New"  title="Create and describe a new Vendor">New Vendor</a>
                        </li>
                        <li class="list-group-item">
                            <a runat="server" href="~/Select/SelectVendor?Command=Edit" title="Choose an existing Vendor and change its description">Edit Vendor</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>--%>

        </div>

    </div>
</asp:Content>
