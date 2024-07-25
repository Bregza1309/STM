"use strict";
function initMap() {
    const CONFIGURATION = {
        "ctaTitle": "Checkout",
        "mapOptions": { "center": { "lat": 37.4221, "lng": -122.0841 }, "fullscreenControl": true, "mapTypeControl": false, "streetViewControl": true, "zoom": 18, "zoomControl": true, "maxZoom": 25, "mapId": "" },
        "mapsApiKey": "AIzaSyBpt0zlCjBZKelUBz4ZYhBnZENLnWJQiRI",
        "capabilities": { "addressAutocompleteControl": true, "mapDisplayControl": true, "ctaControl": true }
    };

    

    const getFormInputElement = (component) => document.getElementById(component + '-input');
    const map = new google.maps.Map(document.getElementById('map'), {
        zoom: CONFIGURATION.mapOptions.zoom,
        center: { lat: 37.4221, lng: -122.0841 },
        mapTypeControl: false,
        fullscreenControl: CONFIGURATION.mapOptions.fullscreenControl,
        zoomControl: CONFIGURATION.mapOptions.zoomControl,
        streetViewControl: CONFIGURATION.mapOptions.streetViewControl
    });
    const marker = new google.maps.Marker({ map: map, draggable: true });
    google.maps.event.addListener(marker, 'dragend', function () {
        getFormInputElement('latitude').value = marker.getPosition().lat();
        getFormInputElement('longitude').value = marker.getPosition().lng();
    })
    const autocompleteInput = getFormInputElement('location');
    const autocomplete = new google.maps.places.Autocomplete(autocompleteInput, {
        fields: ["address_components", "geometry", "name"],
        types: ["address"],
    });
    autocomplete.addListener('place_changed', function () {
        marker.setVisible(false);
        const place = autocomplete.getPlace();
        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            window.alert('No details available for input: \'' + place.name + '\'');
            return;
        }
        renderAddress(place);
        fillInAddress(place);
    });

    function fillInAddress(place) {  // optional parameter
        const addressNameFormat = {
            'street_number': 'long_name',
            'route': 'long_name',
            'locality': 'long_name',
            'administrative_area_level_1': 'short_name',
            'country': 'long_name',
            'postal_code': 'short_name',
        };
        const getAddressComp = function (type) {
            for (const component of place.address_components) {
                if (component.types[0] === type) {
                    return component[addressNameFormat[type]];
                }
            }
            return '';
        };
        getFormInputElement('location').value = getAddressComp('street_number') + ' '
            + getAddressComp('route');
    }

    function renderAddress(place) {
        map.setCenter(place.geometry.location);
        marker.setPosition(place.geometry.location);
        marker.setVisible(true);
        getFormInputElement('latitude').value = place.geometry.location.lat();
        getFormInputElement('longitude').value = place.geometry.location.lng();
    }
}