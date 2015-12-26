(function(){
    var injectParams = ['calendarService','$rootScope'];

    var scheduleController = function (calendarService, $rootScope) {
        var vm = this;
        $rootScope.title = 'Schedule';
        
        vm.list = calendarService.getCalendars();
        
    };

    scheduleController.$inject = injectParams;
    angular.module('ScheduleApp').controller('ScheduleController', scheduleController);
}());