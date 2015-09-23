(function () {
    var injectParams = ["calendarService", "$rootScope", "_"];

    var scheduleController = function (calendarService, $rootScope, _, $moment) {
        var vm = this,
            dayOfWeek = [{ 'val': 0, "day": 'Monday' }, { 'val': 1, "day": 'Tuesday' }, { 'val': 2, "day": 'Wednesday' }, { 'val': 3, "day": 'Thursday' }, { 'val': 4, "day": 'Friday' }, { 'val': 5, "day": 'Saturday' }, { 'val': 6, "day": 'Sunday' }];

        vm.thisweek = [];
        $rootScope.title = "Schedule";

        var x = calendarService.getWeek();

        x.$loaded()
            .then(function (x) {
                displayEvents(x);
            });

        x.$watch(function (event) {
            displayEvents(x);
        });


        function displayEvents(x) {
            angular.forEach(x, function (item, i) {
                item.Day = moment(item.Start).format("dddd");
                item.DayNumber = _.findWhere(dayOfWeek, { day: item.Day }).val;
            });
            var grpd = _.groupBy(x, 'Day');
            var mapped = _.map(grpd, function (item, key) {
                return {
                    Day: key,
                    Items: item,
                    Date: item.length > 0 ? moment(item[0].Start).format('MM/DD/YYYY') : '',
                };
            });
            vm.items = mapped;
        }

    };

    scheduleController.$inject = injectParams;
    angular.module('ScheduleApp').controller('WeekController', scheduleController);
}());