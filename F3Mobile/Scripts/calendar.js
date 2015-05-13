define(['jquery'], function ($) {
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
            }
        ],
        token = "",
        client_id = "";

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

    var allEvents = [];

    function getAllEvents() {
        for (var j = 0; j < calendars.length; j++) {
            var cal = calendars[j];
            listUpcomingEvents(cal);
        }
    }

    function displayEvents(events) {
        var holder = $("#itemHolder");
        holder.append("<div><p>" + events.name + "</p></div>");
        var html = "<ul class='list-group'>";

        $.each(events.items, function (i, item) {
            html += '<li class="list-group-item">' + item.start.date + ' - ' + item.summary + '</li>';
        });
        html += "</ul>";
        holder.append(html);
    }

    function logger(message) {
        toastr.info(message);
    }

    function listUpcomingEvents(calendar) {
        logger('get: ' + calendar.name);
        var request = gapi.client.calendar.events.list({
            'calendarId': calendar.id,
            'timeMin': (new Date()).toISOString(),
            'showDeleted': false,
            'singleEvents': true,
            'maxResults': 10,
            'orderBy': 'startTime'
        });

        request.execute(function (resp) {
            var events = {
                name: calendar.name,
                items: resp.items
            }
            if (events.items.length > 0) {
                allEvents.push(events);
                displayEvents(events);
            }

        });
    }
    

    return {
        initialize: initialize
    };
});