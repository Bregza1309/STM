function initMap() {
            const lat = parseFloat(document.getElementById("userLatitude").value);
    const long = parseFloat(document.getElementById("userLongitude").value);
            var myLatLng = {lat: lat, lng: long};
            var mapOptions = {
                zoom: 15,
                center: myLatLng
            };
            var map = new google.maps.Map(document.getElementById("map"), mapOptions);
            var marker = new google.maps.Marker({
              position: myLatLng,
              map: map,
              title: "User Location",
              icon: {
                url: 'https://c1.klipartz.com/pngpicture/722/763/sticker-png-circle-silhouette-user-user-profile-user-interface-login-avatar-user-account-skin.png', // Replace with the path to your custom marker image
                scaledSize: new google.maps.Size(40, 40) // Set the desired width and height
            }
          });
          //add a location control
          var geoloccontrol = new klokantech.GeolocationControl(map, 13);
          // Create a traffic layer
          var trafficLayer = new google.maps.TrafficLayer();
          trafficLayer.setMap(map);

          // Create a DirectionsService object
          var directionsService = new google.maps.DirectionsService();

          // Create a DirectionsRenderer object
          var directionsRenderer = new google.maps.DirectionsRenderer();
          directionsRenderer.setMap(map);

          // Determine the user's current location
          if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function(position) {
                var userLatLng = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };

                // Define the directions request
                var request = {
                    origin: myLatLng,
                    destination: userLatLng,
                    travelMode: 'DRIVING', // You can change the mode (DRIVING, WALKING, BICYCLING, TRANSIT)
                    provideRouteAlternatives: true, // Set to true to get multiple route options
                    avoidTolls: false,
                    avoidHighways: false
                };

                // Request directions
                directionsService.route(request, function(response, status) {
                    if (status === 'OK') {
                        directionsRenderer.setDirections(response);
                    } else {
                        console.log('Directions request failed: ' + status);
                    }
                });
            });
        }



            // Resize the map when the window is resized
            window.addEventListener('resize', function() {
                var center = map.getCenter();
                google.maps.event.trigger(map, 'resize');
                map.setCenter(center);
            });
        }