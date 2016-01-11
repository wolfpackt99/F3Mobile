(function () {
    var injectParams = ['$http', '$q', '$firebaseArray', '$firebaseAuth', "_"];
    var leaderBoardFactory = function ($http, $q, $firebaseArray, $firebaseAuth, _) {
        var factory = {};
        factory.getLeaders = function (successCallback, errCallback) {
            $http({
                method: "GET",
                url: "/leaderboard/_index",
                headers: {
                    'Content-Type': 'json'
                },
            }).then(function (data) {
                successCallback(data.data);
            }, function (err) {
                errCallback(err);
            });
        }

        return factory;
    };

    leaderBoardFactory.$inject = injectParams;

    angular.module('ScheduleApp').factory('leaderboardService', leaderBoardFactory);

}());