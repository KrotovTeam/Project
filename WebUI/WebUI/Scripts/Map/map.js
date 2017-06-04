// Точки
var points = [];
// Карта
var map;
var rectangle;

var init = function() {
    ymaps.ready(function () {
        map = new ymaps.Map("map", {
            center: [44.547521, 33.705542],
            zoom: 10
        });
        map.events.add('click', function (e) {
            if (points.length < 2) {
                var coords = e.get('coords');
                var placemark = new ymaps.Placemark([coords[0], coords[1]], {
                    preset: 'islands#icon',
                    iconColor: '#0095b6'
                }, {
                    draggable: true
                });

                placemark.events.add('click', function () {
                    map.geoObjects.remove(placemark);
                    var index = points.findIndex(obj => obj.geometry._coordinates[0] === placemark.geometry._coordinates[0] &&
                        obj.geometry._coordinates[1] === placemark.geometry._coordinates[1]);
                    points.splice(index, 1);
                    if (points.length === 1) {
                        $("#map").trigger("removeRectangle");
                    }
                });

                placemark.events.add("dragend", function () {
                    map.geoObjects.remove(rectangle);
                    $("#map").trigger("addRectangle");
                });

                map.geoObjects.add(placemark);
                points.push(placemark);
                if (points.length === 2) {
                    $("#map").trigger("addRectangle");
                }
            }
        });
    });

    $("#map").on("addRectangle", function () {
        rectangle = new ymaps.Rectangle([points[0].geometry._coordinates, points[1].geometry._coordinates],
        {
            strokeColor: '#0000FF',
            strokeWidth: 2
        });
        map.geoObjects.add(rectangle);
    });

    $("#map").on("removeRectangle", function () {
        map.geoObjects.remove(rectangle);
    });
};