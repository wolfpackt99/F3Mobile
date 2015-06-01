define(['jquery', 'toastr', 'moment', 'text!templates/currentweekitem.html', 'text!templates/aoTemplate.html', 'mustache', 'underscore', 'calendarService'],
    function ($, toastr, moment, itemTemplate, aoTemplate, mustache, _, calSvc) {

        var scopes = ['https://www.googleapis.com/auth/calendar.readonly'],
            calendars = [
                {
                    'name': 'Bagpipe',
                    id: 'l5jvg6rn8d1bpcqg76l14hoq7o@group.calendar.google.com'
                },
                {
                    'name': 'Stonehenge',
                    id: '1gb26eve59nn0ipbpn33ud9tk8@group.calendar.google.com'
                },
                {
                    'name': 'The Brave',
                    id: 'um2l72805ggd9nqoq411nf6p50@group.calendar.google.com'
                },
                {
                    'name': 'The Maul',
                    id: 'sn0n3fk13inesa7cju33fpejg4@group.calendar.google.com'
                },
                {
                    'name': 'The Foxhole',
                    id: 'foxholef3@gmail.com'
                },
                {
                    'name': 'Rebel Yell',
                    id: 'clkjm5a176r66ng4q5sddreaug@group.calendar.google.com'
                }
            ],
            token = "",
            client_id = "",
            current = [],
            allEvents = [],
            thisweek = [];

        function initialize(options) {

            calSvc.initialize({
                calSvcUrl: options.calSvcUrl
            });

            getEvents(false);
            //default is current tab
            $(".nav-tabs").bind('click', function (e) {
                if (e.target.text === 'All') {
                    getEvents(true);
                } else {
                    getEvents(false);
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

        function checkAuth() {
            logger('checkAuth');
            try {
                gapi.auth.authorize({
                    'client_id': client_id,
                    'scope': scopes,
                    'immediate': true
                }, handleAuthResult);
            } catch (e) {
                logger('auth call failed: ' + e.message);
            }
        }

        function handleAuthResult(authResult) {
            //logger('begin handleAuthResult: ' + authResult);
            var authorizeDiv = document.getElementById('authorize-div');
            if (authResult && !authResult.error) {
                // Hide auth UI, then load Calendar client library.
                authorizeDiv.style.display = 'none';
                token = authResult['access_token'];
                loadCalendarApi();
                showLogout(true);
            } else {
                showLogout(false);
                // Show auth UI, allowing the user to initiate authorization by
                // clicking authorize button.
                authorizeDiv.style.display = 'inline';
            }
        }

        function showLogout(visible) {

            $('#logout-div').toggle(visible);
        }

        function loadCalendarApi() {
            logger('load calendar');
            gapi.client.load('calendar', 'v3', getAllEvents);
        }

        function displayEvents(all) {
            var sorted = _.sortBy(all ? allEvents : current, 'name');
            var holder = $("#itemHolder");
            $.each(sorted, function (j, event) {
                event.items = _.sortBy(event.items, 'Start.Date');
                $.each(event.items, function (i, item) {
                    var theD = item.Start.Date === null ? moment(item.Start.DateTime) : moment(item.Start.Date);
                    if (isBetween(theD)) {
                        var curItem = {
                            name: event.name,
                            description: item.Summary,
                            date: theD.format("MM/DD/YYYY")
                        };
                        thisweek.push(curItem);
                    }
                    item.displayDate = theD.format("MM/DD/YYYY");
                });
            });
            if (all) {
                var allHtml = mustache.to_html(aoTemplate, sorted);
                holder.append(allHtml);
            } else {
                thisWeek();
            }
        }

        function thisWeek() {
            var sorted = _.sortBy(thisweek, 'date');
            var html = mustache.to_html(itemTemplate, sorted);
            $("#currentWeekItems").html(html);
        }

        function logger(message) {
            if (console && console.log)
                console.log(message);
        }

        function getEvents(all) {
            //$("#loading").show();
            var deferred = [];
            
            $.each(calendars, function (i, cal) {
                var d = $.Deferred();
                deferred.push(d);
                listUpcomingEvents(cal, all, function () {
                    d.resolve();
                });
            });

            $.when.apply(this, deferred).done(function () {
                displayEvents(all);
                logger('all done');
                //$("#loading").hide();
                //$("#current").addClass("active");
            });
        }

        function listUpcomingEvents(calendar, all, callback) {

            allEvents = [];
            current = [];
            thisweek = [];
            calSvc.getEvents(calendar.id, all, function (resp) {
                logger('success get: ' + calendar.name);
                item = {
                    id: calendar.id,
                    summary: resp.Summary,
                    name: calendar.name,
                    items: resp.Items
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

