var map;
var currentStartStations = [];
var startMarkers = [];
var i = 0;

function initMap() {
    map = new google.maps.Map(document.getElementById('map'),
        {
            center: { lat: 52.226637920516765, lng: 21.006863639549124 },
            zoom: 15
        });
    activeteStartListening();

    function createMarker(lat, lng, name, iconColor) {
        return marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: name,
            icon: "http://maps.google.com/mapfiles/ms/icons/" + iconColor + "-dot.png"
        });
    }

    function activeteStartListening() {
        map.addListener('click', function fun(e) {
            // map.removeEventListener('click', fun);
            i++;
            if (i === 1)
                showStartStations(e.latLng.lat(), e.latLng.lng());
        });
    }

    function showStartStations(lat, lng) {
        $.ajax({
            url: "http://localhost:50588/api/Path/" + lat + "/" + lng,
            dataType: "json",
            success: function (data) {

                var currentLocationMarker = createMarker(lat, lng, "You are here", "green");
                var startStationsMarkers = [];

                data.forEach(function (station) {
                    var marker = createMarker(station.lat, station.lng, "qwe", "yellow");
                    startStationsMarkers.push(marker);

                    marker.addListener('click', function () {
                        startStationsMarkers.forEach(function (m) { m.setMap(null) });
                        startStationsMarkers.length = 0;

                        showChosenStation(currentLocationMarker, station.uid, station.lat, station.lng);
                    });
                });

            }
        });
    }

    function showChosenStation(currLocMarker, uid, lat, lng) {
        var chosenStationMarker = createMarker(lat, lng, "qwe", "blue");

        map.addListener('click', function (e) {
            var endLocMarker = createMarker(e.latLng.lat(), e.latLng.lng(), "qwe", "red");

            $.ajax({
                url: "http://localhost:50588/api/Path/" + uid + "/" + e.latLng.lat() + "/" + e.latLng.lng(),
                dataType: "json",
                success: function (data) {
                    var pathMarkers = [];
                    pathMarkers.push(chosenStationMarker);
                    data.forEach(function (station, index) {
                        if (index !== 0)
                            pathMarkers.push(createMarker(station.lat, station.lng, "qwe", "blue"));
                    });

                    activeteStartListening();
                    map.addListener('click', function fun(e) {
                        //  removeEventListener('click', fun);
                        pathMarkers.forEach(function (m) { m.setMap(null) });
                        pathMarkers.length = 0;
                        currLocMarker.setMap(null);
                        endLocMarker.setMap(null);

                    });

                }
            });
        });
    }
}