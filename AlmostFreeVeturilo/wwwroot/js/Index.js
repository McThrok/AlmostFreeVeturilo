var map;
var currentStartStations = [];
var startMarkers = [];
var i = 0;
var LOCKED = "locked";
var DONE = "done";
var pathMarkers = [];
var endLocMarker;
var currentLocationMarker;

var minBikesOnStation = 3;
var timeFactor = 3;

function initMap() {
    map = new google.maps.Map(document.getElementById('map'),
        {
            center: { lat: 52.226637920516765, lng: 21.006863639549124 },
            zoom: 15
        });
    activeteStartListening();


    var startPoint = document.getElementById("startPoint");
    var firstStation = document.getElementById("firstStation");
    var destination = document.getElementById("destination");

    var reset = document.getElementById("reset");
    reset.onclick = function() {
        activeteStartListening();
        pathMarkers.forEach(function(m) { m.setMap(null); });
        pathMarkers.length = 0;

        endLocMarker.setMap(null);
        currentLocationMarker.setMap(null);

        startPoint.classList.remove(DONE);
        firstStation.classList.remove(DONE);
        destination.classList.remove(DONE);

        firstStation.classList.add(LOCKED);
        destination.classList.add(LOCKED);
    };

    function createMarker(lat, lng, name, iconColor) {
        return marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: name,
            icon: "http://maps.google.com/mapfiles/ms/icons/" + iconColor + "-dot.png"
        });
    }

    function activeteStartListening() {
        google.maps.event.addListener(map, 'click', function (e) {
            startPoint.classList.add(DONE);
            firstStation.classList.remove(LOCKED);

            showStartStations(e.latLng.lat(), e.latLng.lng());
            google.maps.event.clearListeners(map, 'click');
        });
    }

    function showStartStations(lat, lng) {
        $.ajax({
            url: "http://localhost:50588/api/Path/" + lat + "/" + lng + "/" + minBikesOnStation,
            dataType: "json",
            success: function (data) {
                currentLocationMarker = createMarker(lat, lng, "Start point", "green");
                var startStationsMarkers = [];

                data.forEach(function (pathPart) {
                    var station = pathPart.station;
                    var marker = createMarker(station.lat, station.lng, "qwe", "yellow");
                    startStationsMarkers.push(marker);

                    marker.addListener('click', function () {
                        startStationsMarkers.forEach(function (m) { m.setMap(null); });
                        startStationsMarkers.length = 0;

                        showChosenStation(currentLocationMarker, station.uid, station.lat, station.lng);
                    });
                });

            }
        });
    }

    function showChosenStation(currLocMarker, uid, lat, lng) {
        firstStation.classList.add(DONE);
        destination.classList.remove(LOCKED);

        var chosenStationMarker = createMarker(lat, lng, "qwe", "blue");
        google.maps.event.addListener(map, 'click', function (e) {
            google.maps.event.clearListeners(map, 'click');
            destination.classList.add(DONE);
            endLocMarker = createMarker(e.latLng.lat(), e.latLng.lng(), "qwe", "red");

            $.ajax({
                url: "http://localhost:50588/api/Path/" + uid + "/" + e.latLng.lat() + "/" + e.latLng.lng() + "/" + minBikesOnStation + "/" + timeFactor,
                dataType: "json",
                success: function (data) {
                    pathMarkers.push(chosenStationMarker);
                    data.stations.forEach(function (station, index) {
                        if (index !== 0)
                            pathMarkers.push(createMarker(station.lat, station.lng, "qwe", "blue"));
                    });

                    //activeteStartListening();
                    //map.addListener('click', function fun(e) {
                    //    //  removeEventListener('click', fun);
                    //    pathMarkers.forEach(function (m) { m.setMap(null) });
                    //    pathMarkers.length = 0;
                    //    currLocMarker.setMap(null);
                    //    endLocMarker.setMap(null);

                    //});

                }
            });
        });
    }
}