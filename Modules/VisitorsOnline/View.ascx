<%@ Control Language="C#" AutoEventWireup="true" Inherits="HCC.VisitorsOnline.View" CodeBehind="View.ascx.cs" %>

<p align="center">
    <asp:Label ID="lblMessage" runat="server" />
</p>

<div id="map" style="width: 100%; height: <%= GetMapHeight() %>px; margin: 0 auto 0 auto;"></div>

<script type="text/javascript">
    window.onload = function () {
        var locations = [<%= GetLocations() %>];

        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 2,
            center: new google.maps.LatLng(<%= GetMapCenter() %>),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });

        var infowindow = new google.maps.InfoWindow();

        var marker, i;

        for (i = 0; i < locations.length; i++) {
            marker = new google.maps.Marker({
                position: new google.maps.LatLng(locations[i][1], locations[i][2]),
                icon: 'http://maps.google.com/mapfiles/ms/icons/' + locations[i][0] + '.png',
                map: map
            });
        }
    }
</script>

<script async defer src="https://maps.googleapis.com/maps/api/js?key=<%= GetGoogleAPIKey() %>&callback=initMap" type="text/javascript"></script>


