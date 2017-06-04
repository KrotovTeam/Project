var Map = {};

Map.points = [];

Map.getPoints = function() {
    return [{
        Latitude: Map.points[0].geometry._coordinates[0],
        Longitude: Map.points[0].geometry._coordinates[1]
    }, {
        Latitude: Map.points[1].geometry._coordinates[0],
        Longitude: Map.points[1].geometry._coordinates[1]
    }];
};

Map.init = function() {
    Map.map = new ymaps.Map("map", {
        center: [44.547521, 33.705542],
        zoom: 10
    });
    Map.map.events.add('click', function (e) {
        if (Map.points.length < 2) {
            var coords = e.get('coords');
            var placemark = new ymaps.Placemark([coords[0], coords[1]], {
                preset: 'islands#icon',
                iconColor: '#0095b6'
            }, {
                draggable: true
            });

            placemark.events.add('click', function () {
                Map.map.geoObjects.remove(placemark);
                var index = Map.points.findIndex(obj => obj.geometry._coordinates[0] === placemark.geometry._coordinates[0] &&
                    obj.geometry._coordinates[1] === placemark.geometry._coordinates[1]);
                Map.points.splice(index, 1);
                if (Map.points.length === 1) {
                    $("#map").trigger("removeRectangle");
                }
            });

            placemark.events.add("dragend", function () {
                Map.map.geoObjects.remove(Map.rectangle);
                $("#map").trigger("addRectangle");
            });

            Map.map.geoObjects.add(placemark);
            Map.points.push(placemark);
            if (Map.points.length === 2) {
                $("#map").trigger("addRectangle");
            }
        }
    });

    $("#map").on("addRectangle", function () {
        Map.rectangle = new ymaps.Rectangle([Map.points[0].geometry._coordinates, Map.points[1].geometry._coordinates],
        {
            strokeColor: '#0000FF',
            strokeWidth: 2
        });
        Map.map.geoObjects.add(Map.rectangle);
    });

    $("#map").on("removeRectangle", function () {
        Map.map.geoObjects.remove(Map.rectangle);
    });
};
