<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Portal11._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        img {
            max-height:100%; min-height: 100%;
            max-width: 100%; min-width: 100%;
        }
    </style>
    <h1 style="text-align: center">Welcome to the CultureTrust Portal</h1>

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">

        <!-- Place a carousel of images inside the welcome jumbotron. -->

        <div id="WelcomeCarousel" class="carousel slide" data-ride="carousel"
            style="width: 600px; height: 400px; margin: 0 auto; padding-left: 0px; padding-right: 0px; ">
            <ol class="carousel-indicators">
                <li data-target="#WelcomeCarousel" data-slide-to="0" class="active"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="1"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="2"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="3"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="4"></li>
            </ol>
            <div class="carousel-inner">
                <div class="item active">
                    <img alt="Slide 1" src="Images/CarouselImage-01.jpg">
                </div>
                <div class="item">
                    <img alt="Slide 2" src="Images/CarouselImage-02.jpg">
                </div>
                <div class="item">
                    <img alt="Slide 3" src="Images/CarouselImage-03.jpg">
                </div>
                <div class="item">
                    <img alt="Slide 4" src="Images/CarouselImage-04.jpg">
                </div>
                <div class="item">
                    <img alt="Slide 5" src="Images/CarouselImage-05.jpg">
                </div>
            </div>
            <a class="left carousel-control" href="#WelcomeCarousel" data-slide="prev">
                <span class="glyphicon glyphicon-chevron-left"></span>
            </a>
            <a class="right carousel-control" href="#WelcomeCarousel" data-slide="next">
                <span class="glyphicon glyphicon-chevron-right"></span>
            </a>
        </div>
        <div class="col-md-12 col-sm-12 col-xs-12">
            <br />
            <p class="lead" style="text-align: center"> 
                <br />This web site helps Project Directors and Staff make requests of CultureWorks Staff.</p>
        </div>

    <div class="row">
        <div class="col-md-4 col-sm-12 col-xs-12">
            <h2>Getting started</h2>
            <p>
                Click the button below to log in. 
                If you do not yet have an account on this system, please contact the CultureWorks staff.
            </p>
            <p>
                <a class="btn btn-default" href="Account/Login.aspx">Login now &raquo;</a>
            </p>
        </div>
        <div class="col-md-4 col-xs-12">
            <h2>About CultureTrust</h2>
            <p>
                CultureTrust is a move-in-ready charitable home and back office support service for the arts and cultural heritage community in Greater Philadelphia.
            </p>
            <p>
                <a class="btn btn-default" href="http://www.culturetrustphila.org" target="_blank">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4 col-xs-12">
            <h2>News of the day</h2>
            <p>
                Click the button below to see a PDF version of the latest Track 2 Member Update.
            </p>
            <p>
                <asp:HyperLink ID="btnViewLink" runat="server" CssClass="btn btn-default col-md-4 col-xs-12" 
                    ToolTip="Click to open a news summary in a new window."
                    NavigateUrl="fill from code behind" Target="_blank">Learn more &raquo;</asp:HyperLink>
            </p>
        </div>
    </div>
    </div>

</asp:Content>
