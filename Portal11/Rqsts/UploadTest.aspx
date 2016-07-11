<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UploadTest.aspx.cs" Inherits="Portal11.Rqsts.UploadTest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>

    <div class="form-horizontal">
        <!--        <h4>Edit a Request</h4>
-->
        <hr />
        <p class="text-danger">
            <asp:Literal runat="server" ID="litDangerMessage" />
        </p>
        <p class="text-success">
            <asp:Literal runat="server" ID="litSuccessMessage" />
        </p>
        <div class="row">
            <asp:FileUpload ID="fupTest" runat="server" ClientIDMode="Static" onchange="this.form.submit()" CssClass="hidden"/>
<div class="btn btn-default col-lg-1 col-xs-2" id="uploadTrigger">Browse</div> 

            <asp:Button ID="btnUpload" runat="server" Text="View" CssClass="btn btn-default col-lg-offset-1 col-lg-1 col-xs-2" OnClick="btnUpload_Click" />
        </div>
        <p>
        </p>

    </div>

<style>
    .hidden {
    display:none;
}

</style>

<script type="text/javascript">
    $("#uploadTrigger").click(function () {
        $("#fupTest").click();
    });

</script>
</asp:Content>
