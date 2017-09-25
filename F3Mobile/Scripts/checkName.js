define(['jquery', 'text!templates/similar.html', 'mustache', 'underscore'],
    function ($, similarTemplate, mustache, _) {
        "use strict";
        var settings = {
            url: ''
        }

        function init(options) {
            $.extend(settings, options);
            $("#nameChecker").click(function () {
                var btn = $(this);
                query(btn);
            });
        }

        function query(btn) {
            $.get(settings.url, { name: $('#F3Name').val() })
                .success(function (data) {
                    var parent = btn.parent(".form-group");
                    parent.removeClass("has-success has-warning");
                    if (data.length === 0) {
                        parent.addClass("has-success");
                    }
                    else {
                        parent.addClass("has-warning");
                    }
                    var html = mustache.to_html(similarTemplate, { users: data, isEmpty: data.length === 0 });
                    $("#name-response").empty();
                    $("#name-response").append(html);
                })
                .error(function () { });
        }

        return {
            init: init
        };
    });