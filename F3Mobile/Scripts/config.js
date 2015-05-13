define(function() {

    function initialize(options) {

        require.config({
            baseUrl: options.baseUrl,
            paths: {
                'jquery': [
                    'https://code.jquery.com/jquery-2.1.3',
                    'jquery-2.1.3'
                ],
                'mustache': [
                    "https://rawgithub.com/janl/mustache.js/master/mustache",
                    "mustache"
                ],
                'bootstrap': [
                    "https://netdna.bootstrapcdn.com/bootstrap/3.3.4/js/bootstrap",
                    "bootstrap"
                ],
                'underscore': "//rawgithub.com/jashkenas/underscore/master/underscore",
                'text': [
                    "https://rawgithub.com/requirejs/text/latest/text",
                    "text"
                ],
                'app': ".."
            },
            shim: {
                'underscore': {
                    exports: '_'
                },

                'bootstrap': {
                    deps: ['jquery'],
                    exports: '$.fn.popover'
                }
            }
        });
    }

    return {
        initialize: initialize
    }
});