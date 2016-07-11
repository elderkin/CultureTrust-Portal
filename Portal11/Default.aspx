<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Portal11._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

        <h1 style="text-align: center">CultureTrust Portal</h1>

    <p class="text-success">
        <asp:Literal runat="server" ID="litSuccessMessage" />
    </p>
    <p class="text-danger">
        <asp:Literal runat="server" ID="litDangerMessage" />
    </p>
    <div class="form-horizontal">

        <!-- Place a carousel of images inside the welcome jumbotron. -->

        <div id="WelcomeCarousel" class="carousel slide col-md-offset-4 col-md-4 col-sm-offset-4 col-sm-4 col-xs-offset-4 col-xs-4" data-ride="carousel">
            <ol class="carousel-indicators">
                <li data-target="#WelcomeCarousel" data-slide-to="0" class="active"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="1"></li>
                <li data-target="#WelcomeCarousel" data-slide-to="2"></li>
            </ol>
            <div class="carousel-inner">
                <div class="item active">
                    <img alt="First slide" src="Images/Liz-Brown-BW-285x162.jpg">
                    <div class="carousel-caption">
                        <h3>Just</h3>
                    </div>
                </div>
                <div class="item">
                    <img alt="Second slide" src="Images/Marangeli-photo-BW-285x162.jpg">
                    <div class="carousel-caption">
                        <h3>A</h3>
                    </div>
                </div>
                <div class="item">
                    <img alt="Third slide" src="Images/Thompson-BW.jpg">
                    <div class="carousel-caption">
                        <h3>placeholder</h3>
                    </div>
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
            <p class="lead" style="text-align: center">Welcome! 
                <br />This web site helps Project Directors and Staff make requests of CultureWorks Staff.</p>
        </div>

    <div class="row">
        <div class="col-md-4 col-sm-12 col-xs-12">
            <h2>Getting started</h2>
            <p>
                Click the button below or the Login link at the top right corner of this page to log in. 
                If you do not yet have an account on this system, please contact the CultureWorks staff at X911.
            </p>
            <p>
                <a class="btn btn-default" href="Account/Login.aspx">Login now &raquo;</a>
            </p>
        </div>
        <div class="col-md-4 col-sm-12 col-xs-12">
            <h2>About CultureTrust</h2>
            <p>
                CultureTrust is a move-in-ready charitable home and back office support service for the arts and cultural heritage community in Greater Philadelphia.
            </p>
            <p>
                <a class="btn btn-default" href="http://www.culturetrustphila.org">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4 col-sm-12 col-xs-12">
            <h2>News of the day</h2>
            <p>
                It rained hard last night.
            </p>
            <p>
                <a class="btn btn-default" href="http://www.weather.com/weather/today/l/Philadelphia+PA+USPA1276:1:US">Learn more &raquo;</a>
            </p>
        </div>
    </div>
    </div>

</asp:Content>
