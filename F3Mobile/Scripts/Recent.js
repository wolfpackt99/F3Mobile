define(['jquery', 'text!templates/recentTemplate.html', 'mustache', 'underscore'],
    function($, recentTemplate, mustache, _) {
        "use strict";
        var settings = {
            url: ''
        }

        function init(options) {
            $.extend(settings, options);
            getRecent();
        }

        function getRecent() {
            $.get(settings.url)
                .success(function(data) {
                    var html = mustache.to_html(recentTemplate, data);
                    $("#recent").append(html);
                })
                .error(function() {});
        }

        return {
            init:init
        };
    });