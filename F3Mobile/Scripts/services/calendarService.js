(function () {
    var injectParams = ['$http', '$q', '$firebaseArray', '$firebaseAuth','_'];
    var calendarFactory = function ($http, $q, $firebaseArray, $firebaseAuth, _) {
        var factory = {},
            dayOfWeek = [{ 'val': 0, "day": 'Monday' }, { 'val': 1, "day": 'Tuesday' }, { 'val': 2, "day": 'Wednesday' }, { 'val': 3, "day": 'Thursday' }, { 'val': 4, "day": 'Friday' }, { 'val': 5, "day": 'Saturday' }, { 'val': 6, "day": 'Sunday' }];

        factory.getCalendars = function () {
            var ref = new Firebase("https://f3area51.firebaseio.com/events");
            ref.authWithCustomToken("U9rk647fbP2EkkiK3qz2cqsSCa2TymlZbPXh2gLq", function (error, authData) {
                if (!error) {
                }
            });
            var x = $firebaseArray(ref);
            x.$loaded()
                .then(function (x) {
                    massageData(x);
                })
                .catch(function (error) {

                });

            x.$watch(function (event) {
                massageData(x);
            });

            return x;
        }

        factory.getWeek = function (callback) {
            var ref = new Firebase("https://f3area51.firebaseio.com/thisweek");
            ref.authWithCustomToken("U9rk647fbP2EkkiK3qz2cqsSCa2TymlZbPXh2gLq", function (error, authData) {
                if (!error) {
                }
            });
            var x = $firebaseArray(ref);
            
            return x;
        }

        function massageData(x) {
            angular.forEach(x, function(item, i) {
                try {
                    var json = JSON.parse(item.Description);
                    item.SiteQ = json.SiteQ;
                    item.Meets = json.Meets;
                    item.DayOfWeek = _.findWhere(dayOfWeek, { day: item.Meets }).val;
                    item.LocationHint = json.LocationHint;
                    item.DisplayLocation = json.DisplayLocation;
                    item.Time = json.Time;
                    item.SignupLink = json.SignupLink;
                } catch (e) {
                    item.SiteQ = item.Description;
                    item.Meets = item.Description;
                    item.LocationHint = null;
                    item.DisplayLocation = item.Location || "";
                    item.Time = null;
                    item.SignupLink = null;
                }
            });
            
        }

        function massageThisWeek(x) {
            angular.forEach(x, function(item, i) {
                try {
                    var json = JSON.parse(item.Description);
                    item.SiteQ = json.SiteQ;
                    item.Meets = json.Meets;
                    item.DayOfWeek = _.findWhere(dayOfWeek, { day: item.Meets }).val;
                    item.LocationHint = json.LocationHint;
                    item.DisplayLocation = json.DisplayLocation;
                    item.Time = json.Time;
                    item.SignupLink = json.SignupLink;
                } catch (e) {
                    item.SiteQ = item.Description;
                    item.Meets = item.Description;
                    item.LocationHint = null;
                    item.DisplayLocation = item.Location || "";
                    item.Time = null;
                    item.SignupLink = null;
                }
            });
            
        }

        return factory;
    };

    calendarFactory.$inject = injectParams;

    angular.module('ScheduleApp').factory('calendarService', calendarFactory);

}());