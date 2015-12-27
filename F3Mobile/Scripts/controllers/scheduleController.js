(function(){
    var injectParams = ['calendarService','regionService','$rootScope'];

    var scheduleController = function (calendarService, regionService, $rootScope) {
        var vm = this;
        $rootScope.title = 'Schedule';
        
        $rootScope.regions = regionService.regions;
        $rootScope.region = $rootScope.regions[0];

        vm.selectedRegion = '';

        $rootScope.setSelected = function () {
            vm.selectedRegion = $rootScope.region.val;
        };

        vm.list = calendarService.getCalendars();
        
    };

    scheduleController.$inject = injectParams;
    angular.module('ScheduleApp').controller('ScheduleController', scheduleController);
}());