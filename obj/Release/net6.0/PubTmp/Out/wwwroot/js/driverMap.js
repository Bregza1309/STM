function initMap() {
    // Create a map centered on a default location
    var map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 37.7749, lng: -122.4194 }, // Default center
        zoom: 13 // Default zoom level
    });
    var geoloccontrol = new klokantech.GeolocationControl(map, 18);
    
    // Create a directions service to use the route information
    var directionsService = new google.maps.DirectionsService;
    var directionsDisplay = new google.maps.DirectionsRenderer({
        panel: document.getElementById('directions')
    });

    // Get the user's current location
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var userLocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            map.setCenter(userLocation);
            // Add a marker for the user's location

            // Calculate and display directions from user's location to a nearby location
            calculateAndDisplayRoute(directionsService, directionsDisplay, userLocation);
        });
        const watchId = navigator.geolocation.watchPosition(
            function (position) {
                const latitude = position.coords.latitude;
                const longitude = position.coords.longitude;
                document.getElementById("driverLatitude").value = latitude;
                document.getElementById("driverLongitude").value = longitude;
            },
            function (error) {
                console.error("Error getting location: " + error.message);
            }
        );
    }
    // Create a directions service to use the route information
    var directionsService = new google.maps.DirectionsService;
    var directionsDisplay = new google.maps.DirectionsRenderer({
        panel: document.getElementById('directions')
    });
    directionsDisplay.setMap(map);
    const lat = parseFloat(document.getElementById("studentLatitude").value);
    const long = parseFloat(document.getElementById("studentLongitude").value);
    
    // Define the coordinates of the nearby location (end)
    var nearbyLocation = { lat: lat, lng: long };

    // Add a marker for the nearby location (end)


    // Center the map on the nearby location (end)
    map.setCenter(nearbyLocation);
    // Add a polyline to represent the route line

}

const lat = parseFloat(document.getElementById("studentLatitude").value);
const long = parseFloat(document.getElementById("studentLongitude").value);
function calculateAndDisplayRoute(directionsService,directionsDisplay, userLocation) {
    directionsService.route({
        origin: userLocation,
        destination: { lat: lat , lng: long }, // Change to your nearby location's coordinates
        travelMode: 'DRIVING'
    }, function (response, status) {
        if (status === 'OK') {
            directionsDisplay.setDirections(response);
            var routePath = response.routes[0].overview_path;
            
        } else {
            window.alert('Directions request failed due to ' + status);
        }
    });
}
