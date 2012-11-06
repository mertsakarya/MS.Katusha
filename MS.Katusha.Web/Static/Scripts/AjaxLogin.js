// Cache for dialogs
var dialogs = {};
profile = { guid: '00000000-0000-0000-0000-000000000000', gender: 'Unknown', name: '* Anonymous *', age: 0, country: '', city: '', tokBoxSessionId: '', tokBoxTicketId: '' };
var tokBoxApiKey = '21083872';


var onLogin = function(profile) {
    if (window.mixpanel != null && profile != null && profile.guid != null) {
        window.mixpanel.identify(profile.guid);
        window.mixpanel.register(profile);
        window.mixpanel.name_tag(profile.name);
    }

    $(function () {
        initTokBox();
        ping();
        setInterval("ping()", 60000);
        //var chat = $.connection.communication;
        //chat.addMessage = function (message) {
        //    alert(message);
        //     $('#messages').append('<li>' + message + '');
        //};
        //chat.leave = function (id, time) { };
        //chat.joined = function (id, time) { alert(time); };
        //chat.rejoined = function (id, time) { };

        ////$("#broadcast").click(function () { chat.send($('#msg').val()); });
        //$.connection.hub.start();
    });
};

var initTokBox = function () {
    // TB.setLogLevel(TB.DEBUG);
    if (TB.checkSystemRequirements() != TB.HAS_REQUIREMENTS) {
        alert('Minimum System Requirements not met!');
    }
    TB.addEventListener('exception', exceptionHandler);

    profile.tokBoxSession = (TB != null && profile.tokBoxSessionId != '') ? TB.initSession(profile.tokBoxSessionId) : null;
    if (profile.tokBoxSession != null) {
        
        profile.tokBoxSession.addEventListener('sessionConnected', sessionConnectedHandler);
        profile.tokBoxSession.addEventListener('sessionDisconnected', sessionDisconnectedHandler);
        profile.tokBoxSession.addEventListener('connectionCreated', connectionCreatedHandler);
        profile.tokBoxSession.addEventListener('connectionDestroyed', connectionDestroyedHandler);
        profile.tokBoxSession.addEventListener('streamCreated', streamCreatedHandler);
        profile.tokBoxSession.addEventListener('streamDestroyed', streamDestroyedHandler);

        profile.tokBoxSession.connect(tokBoxApiKey, profile.tokBoxTicketId);
    }
};

var sessionConnectedHandler = function(event) {
    var parentDiv = document.getElementById("tokBoxPublisherDiv");
    var replacementDiv = document.createElement("div");
    replacementDiv.id = "opentok_publisher";
    parentDiv.appendChild(replacementDiv);

    var publishProps = { height: 160, width: 200 };
    profile.tokBoxPublisher = TB.initPublisher(tokBoxApiKey, replacementDiv.id, publishProps);
    profile.tokBoxSubscribers = { };
    // Send my stream to the session
    session.publish(profile.tokBoxPublisher);
};

var streamCreatedHandler = function(event) {
    for (var i = 0; i < event.streams.length; i++) {
        addStream(event.streams[i]);
    }
};

var addStream = function(stream) {
    if (stream.connection.connectionId == session.connection.connectionId) {
        return;
    }
    var div = document.createElement('div');
    var divId = stream.streamId;
    div.setAttribute('id', divId);
    document.body.appendChild(div);
    var subscriberProps = { width: 200, height: 160 };
    subscribers[stream.streamId] = session.subscribe(stream, divId, subscriberProps);
};

var showNewConversations = function(unreadCount, count) {
    var action = '/Messages/Received';
    var title = '' + unreadCount + ' / ' + count;
    var text = '<img src="/Images/mail.jpg" /><br/><b>' + unreadCount + '</b> / ' + count;
    var link = '<a href="' + action + '" title="' + title + '" style="background-color:#ffffff">' + text + '</a>';
    $('#newMessages').html(link).show("slow");
};

var showNewVisits = function(datetime, count) {
    var action = '/Visitor/NewVisits/' + datetime;
    var title = '' + count + ' visitors.';
    var text = '<img src="/Images/view.gif" /><br/><b>' + count + ' </b> visitors';
    var link = '<a href="' + action + '" title="' + title + '" style="background-color:#ffffff">' + text + '</a>';
    $('#newVisits').html(link).show("slow");
};

var ping = function() {
    $.ajax({
        url: '/Profiles/Ping',
        success: function(data) {
            if (data) {
                if (data.ConversationUnreadCount && data.ConversationUnreadCount > 0) showNewConversations(data.ConversationUnreadCount, data.ConversationCount);
                if (data.VisitCount && data.VisitCount > 0) showNewVisits(data.VisitTime, data.VisitCount);
            }
        },
        type: 'GET'
    });
};

var readM = function(guid, fromGuid, fromName, toGuid, toName, e) {
    $.post('/Messages/Read', { key: guid }).done(function(json) {
        json = json || { };
        if (json.error != null && json.error == "NeedsPayment") {
            var id = "NeedsPayment" + json.product;
            if (!dialogs[id]) {
                loadAndShowDialog(id, { data: function() { return ""; } }, "/Payments/Needed/" + json.product);
            } else {
                dialogs[id].dialog('open');
            }
        } else {
            $('#M' + guid).html(json.message).slideDown('slow');
            $('#showReply' + guid).slideDown('fast');
            if (ping != null) ping();
        }
    });
    // Prevent the normal behavior since we use a dialog
    if (e) e.preventDefault();
    return false;
};

var sendMessage = function(obj, e) {
    var link = $(obj),
        url = link.attr('href'),
        id = url;
    $("#dialog_content").show();
    $("#dialog_message").hide();

    if (!dialogs[id]) {
        loadAndShowDialog(id, link, url);
    } else {
        dialogs[id].dialog('open');
    }

    // Prevent the normal behavior since we use a dialog
    if (e) e.preventDefault();
    return false;
};


var countryChanged = function (/*val, cityId*/) {
    return;
    //not used for now
    //$('input#' + cityId + 'Key')[0].value = '';
    //$('input#' + cityId)[0].value = '';
    //if (val != '')
    //    $('input#' + cityId).show('highlight');
    //else
    //    $('input#' + cityId).hide();
};

var cf = function(cityArea, from, city) {
    var ca = document.getElementById(cityArea);
    var f = document.getElementById(from);
    if (ca != null) {
        if (f != null && f.value.length > 0) {
            ca.style.display = '';
        } else {
            ca.style.display = "none";
            document.getElementById(city).value = '';
            document.getElementById(city + 'Key').value = '';
        }
    }
};

var getValidationSummaryErrors = function ($form) {
    // We verify if we created it beforehand
    var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
    if (!errorSummary.length) {
        errorSummary = $('<div class="validation-summary-errors"><span>Please correct the errors and try again.</span><ul></ul></div>')
            .prependTo($form);
    }

    return errorSummary;
};

var displayErrors = function (form, errors) {
    var errorSummary = getValidationSummaryErrors(form)
        .removeClass('validation-summary-valid')
        .addClass('validation-summary-errors');

    var items = $.map(errors, function (error) {
        return '<li>' + error + '</li>';
    }).join('');

    errorSummary
        .find('ul')
        .empty()
        .append(items);
};

var resetForm = function ($form) {
    // We reset the form so we make sure unobtrusive errors get cleared out.
    if ($form.length > 0) {
        $form[0].reset();
        getValidationSummaryErrors($form)
            .removeClass('validation-summary-errors')
            .addClass('validation-summary-valid');
    }
};

var formSubmitHandler = function (e) {
    var $form = $(this);
    if (!$form.valid || $form.valid()) {

        $("input[type=submit]").attr("disabled", "disabled");
        $.post($form.attr('action'), $form.serializeArray())
            .done(function (json) {
                $("input[type=submit]").removeAttr("disabled");
                json = json || {};
                if (json.success) {
                    if (window.mixpanel != null) {
                        var mp = getMixPanelObject($form);
                        if (mp != null && mp.command != null && mp.command != '') {
                            switch (mp.command) {
                            case 'login':
                                window.mixpanel.people.set({ $last_login: new Date() });
                                window.mixpanel.track("Login");
                                break;
                            }
                        }
                    }
                    if (json.message) {
                        $("#dialog_content").hide();
                        $("#dialog_message").text(json.message).show();
                    } else {
                        document.location = json.redirect || location.href;
                    }
                } else if (json.errors) {
                    displayErrors($form, json.errors);
                }
            })
            .error(function () {
                displayErrors($form, ['An unknown error happened.']);
            });
    }

    // Prevent the normal behavior since we opened the dialog
    e.preventDefault();
};

var loadAndShowDialog = function (id, link, url) {
    var separator = url.indexOf('?') >= 0 ? '&' : '?';

    // Save an empty jQuery in our cache for now.
    dialogs[id] = $();

    // Load the dialog with the content=1 QueryString in order to get a PartialView
    $.get(url + separator + 'content=1')
        .done(function (content) {
            dialogs[id] = $('<div class="modal-popup" style="overflow: auto; overflow-scrolling: auto"><div id="dialog_message" style="display:none; color:navy;"></div><div id="dialog_content">' + content + '</div></div>')
                .hide() // Hide the dialog for now so we prevent flicker
                .appendTo(document.body)
                .filter('div') // Filter for the div tag only, script tags could surface
                .dialog({ // Create the jQuery UI dialog
                    title: link.data('dialog-title'),
                    modal: true,
                    resizable: true,
                    draggable: true,
                    width: link.data('dialog-width') || 600,
                    beforeClose: function () { resetForm($(this).find('form')); }
                })
                .find('form') // Attach logic on forms
                    .submit(formSubmitHandler)
                .end();
        });
};

var MakeProfilePhoto = function(photoBaseUrl, photoType, key, guid) {
    $.getJSON('/Photos/MakeProfilePhoto/' + key + '/' + guid,
        function() {
            var img = document.getElementById("ProfilePhoto");
            if (img != null) {
                img.style.display = "";
                img.src = photoBaseUrl + "Photos/" + photoType + "-" + guid + ".jpg";
                if (window.mixpanel != null) window.mixpanel.track('Make Profile Photo', { guid: profile.guid, name: profile.name, photo_guid: guid });
            }
        }
    );
};

var DeletePhoto = function(key, guid) {
    $.post('/Photos/DeletePhoto', { key: key, photoGuid: guid }).done(function(json) {
        json = json || { };
        if (window.mixpanel != null) window.mixpanel.track('Delete Photo', { guid: profile.guid, name: profile.name, photo_guid: guid });
        if (json.isProfilePhoto) {
            document.getElementById("ProfilePhoto").style.display = "none";
        }
        document.getElementById("Photo:" + guid).style.display = "none";
    });
};

var getMixPanelObject = function (jqueryElement) {
    if (window.mixpanel == null) return null;
    var mpCommand = jqueryElement.attr("mp-command");
    var mpData = jqueryElement.attr("mp-data");
    var mpEvent = jqueryElement.attr("mp-event");
    var intervene;
    if (mpEvent != null && mpEvent.length > 0) {
        intervene = mpEvent.substr(0, 1) != "!";
        if (!intervene) mpEvent = mpEvent.substr(1);
    } else {
        intervene = false;
    }
    var mpObject = null;
    if (mpData != null && mpData != '') {
        try {
            eval("mp_object=" + mpData); //$.parseJSON(mp_data);
        } catch(e) {
            alert(e);
            mpObject = null;
        }
    }
    return { event: mpEvent, data: mpData, object: mpObject, command:mpCommand, intervene : intervene };
};

$(function () {
    $('.bar').mosaic({ animation: 'slide' });
    if (window.mixpanel != null) {
        $(".mixpanel").each(function (id, a) {
            var link = $(a), url = link.attr('href');
            var mp = getMixPanelObject(link);

            link.click(function (e) {
                if (window.mixpanel != null)
                    window.mixpanel.track(mp.event, mp.object);
                if (mp != null && mp.intervene) {
                    e.preventDefault();
                    if (url != null && url != '' && url != '#')
                        location.href = url;
                }
            });
        });
    }
    var links = ['#loginLink', '#changePasswordLink', '#facebookRegisterLink', '#sendMessageButton' /*, '#registerLink', '#registerLink2'*/];
    $.each(links, function (i, id) {
        $(id).click(function (e) {
            var link = $(this),
                url = link.attr('href');

            if (!dialogs[id]) {
                loadAndShowDialog(id, link, url);
            } else {
                dialogs[id].dialog('open');
            }

            // Prevent the normal behavior since we use a dialog
            e.preventDefault();
        });
    });
});

