(function () {
    var injectParams = ['leaderboardService', 'regionService', '$rootScope', '$location', '$http', '$cookies'];

    var leaderboardController = function (leaderboardService, regionService, $rootScope, $location, $http, $cookies) {
        var vm = this;
        vm.showStrava = false;

        var hasApprovedStrava = $cookies.get("allowStrava");
        if (!hasApprovedStrava) {
            vm.showStrava = true;
        }

        function getAccessToken(code, success) {
            var data = {
                code: $location.search().code
            };
            $http.post("/Leaderboard/SetAuth", data)
                .then(function (resp) {
                    success(resp.data);
                    leaderboardService.getLeaders(function (data) {
                        vm.list = data;
                    }, function (err) {

                    });
                }, function (response) {
                    //failed
                });
        }



        if ($location.search().code) {
            getAccessToken($location.search().code, function (resp) {
                $cookies.put("allowStrava", true);
                vm.showStrava = false;
            });
        }
        $rootScope.title = 'Leaderboard';

        $rootScope.regions = regionService.regions;
        $rootScope.region = $rootScope.regions[0];

        vm.selectedRegion = '';

        $rootScope.setSelected = function () {
            vm.selectedRegion = $rootScope.region.val;
        };

        vm.login = function () {
            document.location.href = "https://www.strava.com/oauth/authorize?client_id=9524" +
                "&response_type=code&" +
                "redirect_uri=https://localhost:44306/schedule%23/stats/&" +
                "scope=view_private&" +
                "state=stats&" +
                "approval_prompt=force";
        }


    };




    leaderboardController.$inject = injectParams;
    angular.module('ScheduleApp').controller('LeaderboardController', leaderboardController);
}());