define(['jquery', 'toastr', 'moment', 'text!templates/currentweekitem.html', 'mustache', 'underscore'],
    function ($, toastr, moment, itemTemplate, mustache, _) {
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
                }
            ],
            token = "",
            client_id = "",
            current = [],
            allEvents = [];

        function initialize(options) {

            client_id = options.client_id;
            checkAuth();
            $("#authorize-button").on('click', function () {
                gapi.auth.authorize({
                    client_id: client_id,
                    scope: scopes,
                    immediate: false
                },
                    handleAuthResult);
                return false;
            });

            $("#logout-button").on('click', function () {
                gapi.auth.signOut();
                $.ajax({
                    url: 'https://accounts.google.com/o/oauth2/revoke?token=' + token,
                    type: "get",
                    dataType: 'jsonp'
                }).success(function () {
                    document.location.reload();
                });

            });
        }

        function isBetween(date) {
            var d = moment();
            var startOf = moment().startOf('isoweek');
            var endOf = moment().endOf('isoweek');

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

        function displayEvents() {
            var sorted = _.sortBy(allEvents, 'name');
            $.each(sorted, function (j, event) {
                var holder = $("#itemHolder");
                holder.append("<div><p class='lead'>" + event.summary + "</p></div>");
                var html = "<ul class='list-group'>";
                var curItem = {
                    name: event.name,
                    items: []
                };
                $.each(event.items, function (i, item) {
                    var theD = item.start.date == null ? moment(item.start.dateTime) : moment(item.start.date);
                    if (isBetween(theD)) {
                        thisWeek({
                            name: event.name,
                            description: item.summary,
                            date: theD.format("MM/DD/YYYY")
                        });
                    }
                    var displayDate = moment(((item.start.date != null ? item.start.date : item.start.dateTime)));
                    html += '<li class="list-group-item">' + displayDate.format("MM/DD/YYYY") + ' - ' + item.summary + '</li>';
                });
                current.push(curItem);
                html += "</ul>";
                holder.append(html);
            });

        }

        function thisWeek(data) {

            var html = mustache.to_html(itemTemplate, data);
            $("#currentWeekItems").append(html);

        }

        function logger(message) {
            if (console && console.log)
                console.log(message);
        }
        if (jQuery.when.all === undefined) {
            jQuery.when.all = function (deferreds) {
                var deferred = new jQuery.Deferred();
                $.when.apply(jQuery, deferreds).then(
                    function () {
                        deferred.resolve(Array.prototype.slice.call(arguments));
                    },
                    function () {
                        deferred.fail(Array.prototype.slice.call(arguments));
                    });

                return deferred;
            }
        }

        function getAllEvents() {
            var deferred = [];
            $.each(calendars, function (i, cal) {
                var d = $.Deferred();
                deferred.push(d);
                listUpcomingEvents(cal, function () {
                    d.resolve();
                });
            });
            
            $.when.apply(this, deferred).done(function() {
                displayEvents();
                logger('all done');
            });
        }

        function listUpcomingEvents(calendar, callback) {

            var request = gapi.client.calendar.events.list({
                'calendarId': calendar.id,
                'timeMin': (moment().startOf('isoweek')).toISOString(),
                'showDeleted': false,
                'singleEvents': true,
                'maxResults': 10,
                'orderBy': 'startTime'
            });

            request.execute(function (resp) {
                logger('get: ' + calendar.name);
                allEvents.push({
                    summary: resp.summary,
                    name: calendar.name,
                    items: resp.items
                });
                callback();
            });
        }


        return {
            initialize: initialize
        };
    });

