define(['jquery', 'toastr', 'moment', 'text!templates/thisweek.html', 'text!templates/aoTemplate.html', 'text!templates/firstf.html', 'mustache', 'underscore', 'calendarService'],
    function ($, toastr, moment, itemTemplate, aoTemplate, firstTemplate, mustache, _, calSvc) {

        var current = [],
            allEvents = [],
            thisweek = [],
            list = [],
            hasDisplayed = false,
            dayOfWeek = [{ 'val': 0, "day": 'Monday' }, { 'val': 1, "day": 'Tuesday' }, { 'val': 2, "day": 'Wednesday' }, { 'val': 3, "day": 'Thursday' }, { 'val': 4, "day": 'Friday' }, { 'val': 5, "day": 'Saturday' }, { 'val': 6, "day": 'Sunday' }];

        function initialize(options) {

            calSvc.initialize({
                calSvcUrl: options.calSvcUrl,
                calListUrl: options.calListUrl
            });

            getEvents(false);
            //default is current tab
            $(".nav-tabs").bind('click', function (e) {
                if (e.target.text === 'All') {
                    getEvents(true);
                } else if (e.target.text === "This Week") {
                    getEvents(false);
                } else {
                    displayFirstF();
                }
            });

            //$("#current").click();

            $("#workout").change(function () {
                var selected = $(this).find("option:selected").val();
                if (selected === "All") {
                    $("div.ao").show();
                } else {
                    $("div.ao").hide();
                    $("div.ao[data-name='" + selected + "']").show();
                }
            });
        }

        function isBetween(date) {
            var d = moment();
            var startOf = moment().startOf('week');
            var endOf = moment().endOf('week');

            return date.isBetween(startOf, endOf);
        }

        function displayEvents(all) {
            var sorted = _.sortBy(all ? allEvents : current, 'summary');
            var holder = $("#itemHolder");
            $.each(sorted, function (j, event) {
                event.items = _.sortBy(event.items, 'Start.Date');
                $.each(event.items, function (i, item) {
                    var theD = item.Start.Date === null ? moment(item.Start.DateTime) : moment(item.Start.Date);
                    if (isBetween(theD)) {
                        var curItem = {
                            name: event.name,
                            description: item.Summary,
                            date: theD.format("MM/DD/YYYY"),
                            day: theD.format("dddd"),
                            dateraw: theD,
                            location: event.location
                        };
                        thisweek.push(curItem);
                    }
                    item.displayDate = theD.format("MM/DD/YYYY");
                });
            });
            if (all) {
                var allHtml = mustache.to_html(aoTemplate, sorted);
                holder.html(allHtml);
            } else {
                thisWeek();
            }
        }

        function thisWeek() {
            var sorted = _.sortBy(thisweek, function (item) { return item.dateraw.date(); });
            var days = _.groupBy(sorted, 'day');
            var mapped = _.map(days, function (item) {
                var dayText = _.findWhere(dayOfWeek, { day: item[0].day });
                
                return {
                    day: item.length > 0 ? item[0].day : '',
                    date: item.length > 0 ? item[0].date : '',
                    sort: dayText ? dayText.val : 10,
                    items: item
                };
            });
            
            var html = mustache.to_html(itemTemplate, _.sortBy(mapped, 'sort'));
            $("#currentWeekItems").html(html);
        }

        function logger(message) {
            if (console && console.log)
                console.log(message);
        }

        function displayFirstF() {
            
            if (hasDisplayed == false && list && list.Items && list.Items.length > 0) {
                $("#loading-firstf").hide();
                $.each(list.Items, function (i, item) {
                    try {
                        var json = JSON.parse(item.Description);
                        item.SiteQ = json.SiteQ;
                        item.Meets = json.Meets;
                        item.LocationHint = json.LocationHint;
                        item.DisplayLocation = json.DisplayLocation;
                        item.Time = json.Time;
                        item.SignupLink = json.SignupLink;
                    } catch (e) {
                        item.SiteQ = item.Description;
                        item.Meets = item.Description;
                        item.LocationHint = null;
                        item.DisplayLocation = item.Location || "";
                        item.Time = "";
                        item.SignupLink = "";
                    }
                });
                var sorted = _(list.Items)
                    .chain()
                    .sortBy(function (item) {
                        return item.Summary;
                    })
                    .sortBy(function (item) {
                        var data = _.findWhere(dayOfWeek, { day: item.Meets });
                        if (data) {
                            return data.val;
                        }
                        return "";
                    })
                    .value();
                var html = mustache.to_html(firstTemplate, sorted);
                $("#firstF").append(html);
                hasDisplayed = true;
            }
        }

        function getEvents(all) {
            $("#loading-" + (all ? "all" : "current")).show();
            var deferred = [];

            calSvc.getList(function (data) {
                list = data;
                
                $.each(list.Items, function (i, cal) {
                    var d = $.Deferred();
                    deferred.push(d);
                    listUpcomingEvents(cal, all, list, function () {
                        d.resolve();
                    });
                });

                $.when.apply(this, deferred).done(function () {
                    displayEvents(all);
                    logger('all done');
                    $("#loading-" + (all ? "all" : "current")).hide();
                    //$("#current").addClass("active");
                });
            }, function () {
                console.log('unable to get list');
            });



        }

        function listUpcomingEvents(calendar, all, list, callback) {

            var location = _.findWhere(list.Items, { Id: calendar.Id });

            allEvents = [];
            current = [];
            thisweek = [];
            calSvc.getEvents(calendar.Id, all, function (resp) {
                logger('success get: ' + calendar.name);
                var item = {
                    id: calendar.Id,
                    summary: resp.Summary,
                    name: calendar.Summary,
                    items: resp.Items,
                    location: location.Location || ""
                };
                if (all) {
                    allEvents.push(item);
                } else {
                    current.push(item);
                }
                callback();
            }, function () {
                callback();
            });
        }

        return {
            initialize: initialize
        };
    });

