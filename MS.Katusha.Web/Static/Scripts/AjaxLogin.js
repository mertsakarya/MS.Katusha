// Cache for dialogs
var dialogs = {};
profile = {guid:'00000000-0000-0000-0000-000000000000', gender:'Unknown', name:'* Anonymous *', age:0, country:'', country:''};

var readM = function(guid, fromGuid, fromName, toGuid, toName, e) {
    $.post('/Messages/Read', { key: guid }).done(function(json) {
        json = json || { };
        if (json.error != null && json.error == "NeedsPayment") {
            var id = "NeedsPayment" + json.product;
            if (!dialogs[id]) {
                loadAndShowDialog(id, { data: function(x) { return ""; } }, "/Payments/Needed/" + json.product);
            } else {
                dialogs[id].dialog('open');
            }
        } else {
            $('#M' + guid).text(json.message).slideDown('slow');
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


var countryChanged = function (val, cityId) {
    $('input#' + cityId + 'Key')[0].value = '';
    $('input#' + cityId)[0].value = '';
    if (val != '')
        $('input#' + cityId).show('highlight');
    else
        $('input#' + cityId).hide();
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
}

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

    var ul = errorSummary
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

    // We check if jQuery.validator exists on the form
    if (!$form.valid || $form.valid()) {
        $.post($form.attr('action'), $form.serializeArray())
            .done(function (json) {
                json = json || {};

                // In case of success, we redirect to the provided URL or the same page.
                if (json.success) {
                    if (json.message) {
                        $("#dialog_content").hide();
                        $("#dialog_message").text(json.message).show();
                    } else {
                        location = json.redirect || location.href;
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

var MakeProfilePhoto = function(key, guid) {
    $.getJSON('/Photos/MakeProfilePhoto/' + key + '/' + guid,
        function () {
            var img = document.getElementById("ProfilePhoto");
            if (img != null) {
                img.style.display = "";
                img.src = "@(Html.GetPhotoBaseUrl())Photos/@(photoType)-" + guid + ".jpg";
                mixpanel.track('Make Profile Photo', { guid: profile.guid, name: profile.name, photo_guid: guid });
            }
        }
    );
}

var DeletePhoto = function(key, guid) {
    $.post('/Photos/DeletePhoto', { key: key, photoGuid: guid }).done(function (json) {
        json = json || {};
        mixpanel.track('Delete Photo', { guid: profile.guid, name: profile.name, photo_guid: guid });
        if (json.isProfilePhoto) { document.getElementById("ProfilePhoto").style.display = "none"; }
        document.getElementById("Photo:" + guid).style.display = "none";
    });
}


$(function () {
    $('.bar').mosaic({ animation: 'slide' });
    if (mixpanel != null) {
        $(".mixpanel").each(function (id, a) {
            var link = $(a), url = link.attr('href');
            var mp_data = link.attr("mp-data");
            var mp_event = link.attr("mp-event");
            var intervene = mp_event.substr(0, 1) != "!";
            if (!intervene) mp_event = mp_event.substr(1);
            var mp_object = null;
            if (mp_data != null && mp_data != '') {
                try {
                    eval("mp_object="+mp_data); //$.parseJSON(mp_data);
                } catch (e) {
                    alert(e);
                    mp_object = null;
                }
            }

            link.click(function (e) {
                mixpanel.track(mp_event, mp_object);
                if (intervene) {
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

