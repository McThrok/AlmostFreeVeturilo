
var map;
var currentStartStations = [];
var startMarkers = [];
var i = 0;
var LOCKED = "locked";
var DONE = "done";
var pathMarkers = [];
var endLocMarker;
var currentLocationMarker;

var minBikesAtStation = 3;
var timeFactor = 2;

var stationMarkers = [];

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
    var cost = document.getElementById("cost");

    // minBikesAtStationSlider
    var minBikesAtStationSlider = document.getElementById("minBikesAtStationSlider");
    var minBikesAtStationP = document.getElementById("minBikesAtStationP");
    minBikesAtStationSlider.value = minBikesAtStation;
    minBikesAtStationP.innerHTML = "Minimum bicycles at station: " + minBikesAtStation;

    minBikesAtStationSlider.oninput = function () {
        minBikesAtStation = this.value;
        minBikesAtStationP.innerHTML = "Minimum bicycles at station: " + minBikesAtStation;
    }
    // timeFactorSlider
    var timeFactorSlider = document.getElementById("timeFactorSlider");
    var timeFactorP = document.getElementById("timeFactorP");
    timeFactorSlider.value = timeFactor;
    timeFactorP.innerHTML = "Time factor: " + timeFactor;

    timeFactorSlider.oninput = function () {
        timeFactor = this.value;
        timeFactorP.innerHTML = "Time factor: " + timeFactor;
    }
    //
    var reset = document.getElementById("reset");
    reset.onclick = function () {
        activeteStartListening();
        pathMarkers.forEach(function (m) { m.setMap(null); });
        pathMarkers.length = 0;

        endLocMarker.setMap(null);
        currentLocationMarker.setMap(null);

        startPoint.classList.remove(DONE);
        firstStation.classList.remove(DONE);
        destination.classList.remove(DONE);

        firstStation.classList.add(LOCKED);
        destination.classList.add(LOCKED);

        cost.classList.add(LOCKED);
        cost.innerHTML = "💰Estimated cost💰 --zł";
    };

    //TODO uncomment
    $.ajax({
        url: "http://localhost:50588/api/Path/",
        dataType: "json",
        success: function (data) {
            //console.log(data);
            for (var i = 0; i < data.length; i++) {
                var station = data[i];
                //console.log(station.bikes)
                stationMarkers.push(markerForBasicStation(station.lat, station.lng, station.name, station.bikes));
            }
        }
    });

    var hideStations = document.getElementById("hideStations");
    console.log(hideStations);
    hideStations.onclick = function () {
        if (hideStations.checked) {
            for (var i = 0; i < stationMarkers.length; i++) {
                stationMarkers[i].setMap(null);
            }
        }
        else {
            console.log("un checked");
            for (var i = 0; i < stationMarkers.length; i++) {
                stationMarkers[i].setMap(map);
            }
        }
    }

    var settingsIcon = document.getElementById("settingsIcon");
    var settingsDiv = document.getElementById("settings");
    settingsDiv.style.display = "none";
    settingsIcon.onclick = function () {
        if (settingsDiv.style.display === "none")
            settingsDiv.style.display = "block";
        else
            settingsDiv.style.display = "none";
    }
    // MARKERS
    function correctBikesCount(bikes) {
        if (bikes == 0) bikes = "O";
        if (bikes > 10) bikes = 10;

        return bikes;
    }

    function markerForBasicStation(lat, lng, name, bikes) {
        bikes = correctBikesCount(bikes);

        return marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: name,
            icon: "http://maps.google.com/mapfiles/kml/paddle/" + bikes + "-lv.png"
        });
    }
    function createMarker(lat, lng, name, bikes) {
        bikes = correctBikesCount(bikes);

        return marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: name,
            icon: "http://maps.google.com/mapfiles/kml/paddle/" + bikes + ".png"
        });
    }
    //

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
            url: "http://localhost:50588/api/Path/" + lat + "/" + lng + "/" + minBikesAtStation,
            dataType: "json",
            success: function (data) {
                currentLocationMarker = createMarker(lat, lng, "Start point", "A");
                var startStationsMarkers = [];

                data.forEach(function (pathPart) {
                    var station = pathPart.station;
                    var marker = createMarker(station.lat, station.lng, station.name, station.bikes);
                    startStationsMarkers.push(marker);

                    marker.addListener('click', function () {
                        startStationsMarkers.forEach(function (m) { m.setMap(null); });
                        startStationsMarkers.length = 0;

                        showChosenStation(currentLocationMarker, station);
                    });
                });

            }
        });
    }

    function showChosenStation(currLocMarker, station) {
        firstStation.classList.add(DONE);
        destination.classList.remove(LOCKED);

        var chosenStationMarker = createMarker(station.lat, station.lng, station.name, station.bikes);
        google.maps.event.addListener(map, 'click', function (e) {
            google.maps.event.clearListeners(map, 'click');
            destination.classList.add(DONE);
            endLocMarker = createMarker(e.latLng.lat(), e.latLng.lng(), "Destination", "B");

            $.ajax({
                url: "http://localhost:50588/api/Path/" + station.uid + "/" + e.latLng.lat() + "/" + e.latLng.lng() + "/" + minBikesAtStation + "/" + timeFactor,
                dataType: "json",
                success: function (data) {
                    pathMarkers.push(chosenStationMarker);
                    data.stations.forEach(function (station, index) {
                        if (index !== 0)
                            pathMarkers.push(createMarker(station.lat, station.lng, station.name, station.bikes));
                    });
                    cost.classList.remove(LOCKED);
                    cost.innerHTML = "💰Estimated cost💰 " + data.cost + "zł";
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