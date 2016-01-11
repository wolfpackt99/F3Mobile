(function(){
    var injectParams = ['leaderboardService','regionService', '$rootScope'];

    var leaderboardController = function (leaderboardService, regionService, $rootScope) {
        var vm = this;
        $rootScope.title = 'Leaderboard';
        
        $rootScope.regions = regionService.regions;
        $rootScope.region = $rootScope.regions[0];

        vm.selectedRegion = '';

        $rootScope.setSelected = function () {
            vm.selectedRegion = $rootScope.region.val;
        };

        leaderboardService.getLeaders(function(data) {
            vm.list = data;
        }, function(err) {
            
        });
        
    };

    leaderboardController.$inject = injectParams;
    angular.module('ScheduleApp').controller('LeaderboardController', leaderboardController);
}());