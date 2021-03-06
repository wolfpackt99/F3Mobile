(function () {
    var underscore = angular.module('underscore', []);
    underscore.factory('_', function () {
        return window._; //Underscore must already be loaded on the page
    });
    var app = angular.module('ScheduleApp', [
    'ngRoute',
    'ngResource',
    'firebase',
    'underscore',
    'angularMoment',
    'ngCookies'
    ]);

    app.config(function ($routeProvider) {

        $routeProvider.when('/', {
            controller: 'WeekController',
            templateUrl: '/scripts/templates/week.html',
            reloadOnSearch: false,
            controllerAs: 'vm',
            activeTab: 'week'
        });

        $routeProvider.when('/schedule', { 
            controller: 'ScheduleController',
            templateUrl: '/scripts/templates/schedule.html',
            reloadOnSearch: false,
            controllerAs: 'vm',
            activeTab: 'schedule'
        });

        $routeProvider.when('/week', {
            controller: 'WeekController',
            templateUrl: '/scripts/templates/week.html',
            reloadOnSearch: false,
            controllerAs: 'vm',
            activeTab: 'week'
        });

        $routeProvider.when('/stats', {
            controller: 'LeaderboardController',
            templateUrl: '/scripts/templates/leaderboard.html',
            reloadOnSearch: false,
            controllerAs: 'vm',
            activeTab: 'stats'
        });

        $routeProvider.when('/map', {
            controller: 'MapController',
            templateUrl: '/scripts/templates/map.html',
            reloadOnSearch: false,
            controllerAs: 'vm',
            activeTab: 'map'
        });
    });



}())
