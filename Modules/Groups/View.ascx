<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.Groups.View" CodeBehind="View.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<p align="center">
    <dnn:ActionLink id="cmdManage" runat="server" Title="Manage Your Group" ControlKey="Edit" Security="View" />
    <asp:Label id="lblLogin" runat="server" text="<br />Please Login To Manage Your Group" />
</p>

<div id="map" style="width: 100%; height: <%= GetMapHeight() %>px; margin: 0 auto 0 auto;"></div>

<script type="text/javascript">
    window.onload = function () {
        var locations = [<%= GetLocations() %>];

        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 4,
            center: new google.maps.LatLng(<%= GetMapCenter() %>),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });

        var infowindow = new google.maps.InfoWindow();

        var marker, i;

        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                map: map
            });

            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infowindow.setContent(locations[i][0]);
                    infowindow.open(map, marker);
                }
            })(marker, i));
        }
    }
</script>

<script async defer src="https://maps.googleapis.com/maps/api/js?key=<%= GetGoogleAPIKey() %>&callback=initMap" type="text/javascript"></script>


