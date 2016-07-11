<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModalTest.aspx.cs"
    Inherits="Portal11.Staff.ModalTest" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>


    <p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat wisi, condimentum sed, commodo vitae, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. Donec non enim in turpis pulvinar facilisis. Ut felis. Praesent dapibus, neque id cursus faucibus, tortor neque egestas augue, eu vulputate magna eros eu erat. Aliquam erat volutpat. Nam dui mi, tincidunt quis, accumsan porttitor, facilisis luctus, metus</p>

    <p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo.</p>

    <p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo.</p>

    <input type="button" runat="server" id="btnDialog" name="btnDialog" value="Delete" class="btn btn-default" />

    <script>
        $(document).ready(function () {
            $("#dialog").dialog({ autoOpen: false, height: 400, width: 400 });
        });
        $("#btnDialog").click(function () {
            $("#dialog").dialog("open");
        });
    </script>

    <style>.ui-dialog-titlebar-close {visibility: hidden;}</style>

    <div id="dialog" class="modal">
        <div class="modal-dialog">

            <div class="modal-content">
                <div class="modal-header modal-success">
                    <h4 class="modal-title">CultureTrust Portal</h4>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to permanently delete this Request?</p>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnConfirmNo" runat="server" OnClick="btnConfirmNo_Click" Text="No" CssClass="btn btn-default"
                        UseSubmitBehavior="false" data-dismiss="modal" />
                    <asp:Button ID="btnConfirmYes" runat="server" OnClick="btnConfirmYes_Click" Text="Yes" CssClass="btn btn-primary"
                        UseSubmitBehavior="false" data-dismiss="modal" />
                </div>
            </div>
        </div>

    </div>
</asp:Content>
