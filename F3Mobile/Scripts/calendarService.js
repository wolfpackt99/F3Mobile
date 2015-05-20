define(['jquery'], function ($) {
    var settings = {};
    function initialize(options) {
        $.extend(settings, options);
    }
    function getEvents(id, success, error) {
        $.ajax({
            url: settings.calSvcUrl  + "?id=" + id,
            type: 'GET'
        })
        .success(function (data) {
            success(data);
        })
        .error(function (err) {
            error(err);
        });
    };

    return {
        initialize: initialize,
        getEvents: getEvents
    };
});