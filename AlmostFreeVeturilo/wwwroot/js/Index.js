var map;

var LOCKED = "locked";
var DONE = "done";

var startLocMarker;
var startStationsMarkers = [];
var chosenStationMarker;
var pathMarkers = [];
var endLocMarker;

var minBikesAtStation = 3;
var speedFactor = 0.3;

var stationMarkers = [];

function initMap() {
    var directionsService = new google.maps.DirectionsService;
    var directionsDisplayStart = new google.maps.DirectionsRenderer({ suppressMarkers: true });
    var directionsDisplayPath = new google.maps.DirectionsRenderer({ suppressMarkers: true });
    var directionsDisplayEnd = new google.maps.DirectionsRenderer({ suppressMarkers: true });

    map = new google.maps.Map(document.getElementById('map'),
        {
            center: { lat: 52.226637920516765, lng: 21.006863639549124 },
            zoom: 15
        });
    activeteStartListening();
    directionsDisplayStart.setMap(map);
    directionsDisplayPath.setMap(map);
    directionsDisplayEnd.setMap(map);


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
    };
    // timeFactorSlider
    var timeFactorSlider = document.getElementById("timeFactorSlider");
    var timeFactorP = document.getElementById("timeFactorP");
    timeFactorSlider.value = speedFactor;
    timeFactorP.innerHTML = "Time factor: " + speedFactor;

    timeFactorSlider.oninput = function () {
        speedFactor = this.value;
        timeFactorP.innerHTML = "Time factor: " + speedFactor;
    };
    //
    var reset = document.getElementById("reset");
    reset.onclick = function () {
        google.maps.event.clearListeners(map, 'click');
        activeteStartListening();

        if (startLocMarker)
            startLocMarker.setMap(null);

        startStationsMarkers.forEach(function (m) { m.setMap(null); });
        startStationsMarkers.length = 0;

        if (chosenStationMarker)
            chosenStationMarker.setMap(null);

        pathMarkers.forEach(function (m) { m.setMap(null); });
        pathMarkers.length = 0;

        if (endLocMarker)
            endLocMarker.setMap(null);

        startPoint.classList.remove(DONE);
        firstStation.classList.remove(DONE);
        destination.classList.remove(DONE);

        firstStation.classList.add(LOCKED);
        destination.classList.add(LOCKED);

        cost.classList.add(LOCKED);
        cost.innerHTML = "💰Estimated cost💰 --zł";

        directionsDisplayStart.setDirections({ routes: [] });
        directionsDisplayPath.setDirections({ routes: [] });
        directionsDisplayEnd.setDirections({ routes: [] });
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
    hideStations.onclick = function () {
        if (hideStations.checked) {
            for (var i = 0; i < stationMarkers.length; i++) {
                stationMarkers[i].setMap(null);
            }
        } else {
            for (var i = 0; i < stationMarkers.length; i++) {
                stationMarkers[i].setMap(map);
            }
        }
    };


    var settingsIcon = document.getElementById("settingsIcon");
    var settingsDiv = document.getElementById("settings");
    settingsDiv.style.display = "none";
    settingsIcon.onclick = function () {
        if (settingsDiv.style.display === "none")
            settingsDiv.style.display = "block";
        else
            settingsDiv.style.display = "none";
    };
    // MARKERS
    function correctBikesCount(bikes) {
        if (bikes === 0) bikes = "O";
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
        console.log('click');
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
                startLocMarker = createMarker(lat, lng, "Start point", "A");

                data.forEach(function (pathPart) {
                    var station = pathPart.station;
                    var marker = createMarker(station.lat, station.lng, station.name, station.bikes);
                    startStationsMarkers.push(marker);

                    marker.addListener('click', function () {
                        startStationsMarkers.forEach(function (m) { m.setMap(null); });
                        startStationsMarkers.length = 0;

                        showChosenStation(startLocMarker, station);
                    });
                });

            }
        });
    }

    function showChosenStation(currLocMarker, station) {
        firstStation.classList.add(DONE);
        destination.classList.remove(LOCKED);

        chosenStationMarker = createMarker(station.lat, station.lng, station.name, station.bikes);
        google.maps.event.addListener(map, 'click', function (e) {
            google.maps.event.clearListeners(map, 'click');
            destination.classList.add(DONE);
            endLocMarker = createMarker(e.latLng.lat(), e.latLng.lng(), "Destination", "B");

            $.ajax({
                url: "http://localhost:50588/api/Path/" + station.uid + "/" + e.latLng.lat() + "/" + e.latLng.lng() + "/" + minBikesAtStation + "/" + 1/speedFactor,
                dataType: "json",
                success: function (data) {
                    pathMarkers.push(chosenStationMarker);
                    data.stations.forEach(function (station, index) {
                        if (index !== 0)
                            pathMarkers.push(createMarker(station.lat, station.lng, station.name, station.bikes));
                    });
                    cost.classList.remove(LOCKED);
                    cost.innerHTML = "💰Estimated cost💰 " + data.cost + "zł";

                    drawRoute();

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

    function drawRoute() {
        drawStartWalk();
        drawBikePath();
        drawEndWalk();
    }

    function drawStartWalk() {
        var destinationRoute = pathMarkers.length > 0 ? markerToLocation(pathMarkers[0]) : markerToLocation(endLocMarker);
        directionsService.route({
            origin: markerToLocation(startLocMarker),
            destination: destinationRoute,
            travelMode: 'WALKING'
        }, function (response, status) {
            if (status === 'OK')
                directionsDisplayStart.setDirections(response);

        });
    }

    function drawBikePath() {
        if (pathMarkers.length < 2) return;

        var waypts = [];
        for (var i = 1; i < pathMarkers.length - 1; i++) {
            waypts.push({
                location: markerToLocation(pathMarkers[i]),
                stopover: true
            });

        }

        directionsService.route({
            origin: markerToLocation(pathMarkers[0]),
            waypoints: waypts,
            destination: markerToLocation(pathMarkers[pathMarkers.length - 1]),
            travelMode: 'BICYCLING'
        }, function (response, status) {
            if (status === 'OK')
                directionsDisplayPath.setDirections(response);

        });
    }

    function drawEndWalk() {
        var originRoute = pathMarkers.length > 0 ? markerToLocation(pathMarkers[pathMarkers.length - 1]) : markerToLocation(startLocMarker);

        directionsService.route({
            origin: originRoute,
            destination: markerToLocation(endLocMarker),
            travelMode: 'WALKING'
        }, function (response, status) {
            if (status === 'OK')
                directionsDisplayEnd.setDirections(response);

        });
    }

    function markerToLocation(marker) {
        var latLng = marker.position;
        return latLng.lat() + "," + latLng.lng();
    }
}