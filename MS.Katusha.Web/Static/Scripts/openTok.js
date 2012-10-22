var apiKey = "21083872"; // Replace with your API key. See http://www.tokbox.com/
var sessionId = "{$queryStringSessionId}"; // Replace with your own session ID. See http://www.tokbox.com/opentok/api/tools/generator
var token = ''; // Replace with a generated token. See http://www.tokbox.com/opentok/api/tools/generator
		
var session;
var publisher;
var subscribers = {};
var VIDEO_WIDTH = 320;
var VIDEO_HEIGHT = 240;

TB.addEventListener("exception", exceptionHandler);
		
// Un-comment the following to set automatic logging:
// TB.setLogLevel(TB.DEBUG);

if (TB.checkSystemRequirements() != TB.HAS_REQUIREMENTS) {
    alert("You don't have the minimum requirements to run this application."
          + "Please upgrade to the latest version of Flash.");
} else {
    session = TB.initSession(sessionId);	// Initialize session

    // Add event listeners to the session
    session.addEventListener('sessionConnected', sessionConnectedHandler);
    session.addEventListener('sessionDisconnected', sessionDisconnectedHandler);
    session.addEventListener('connectionCreated', connectionCreatedHandler);
    session.addEventListener('connectionDestroyed', connectionDestroyedHandler);
    session.addEventListener('streamCreated', streamCreatedHandler);
    session.addEventListener('streamDestroyed', streamDestroyedHandler);
}

function connect() {
    session.connect(apiKey, token);
}

function disconnect() {
    session.disconnect();
    hide('disconnectLink');
    hide('publishLink');
    hide('unpublishLink');
}

function startPublishing() {
    if (!publisher) {
        var parentDiv = document.getElementById("myCamera");
        var publisherDiv = document.createElement('div'); // Create a div for the publisher to replace
        publisherDiv.setAttribute('id', 'opentok_publisher');
        parentDiv.appendChild(publisherDiv);
        var publisherProps = {width: VIDEO_WIDTH, height: VIDEO_HEIGHT};
        publisher = TB.initPublisher(apiKey, publisherDiv.id, publisherProps);  // Pass the replacement div id and properties
        session.publish(publisher);
        show('unpublishLink');
        hide('publishLink');
    }
}

function stopPublishing() {
    if (publisher) {
        session.unpublish(publisher);
    }
    publisher = null;

    show('publishLink');
    hide('unpublishLink');
}

function sessionConnectedHandler(event) {
    for (var i = 0; i < event.streams.length; i++) {
        addStream(event.streams[i]);
    }
    show('disconnectLink');
    show('publishLink');
    hide('connectLink');
}

function streamCreatedHandler(event) {
    for (var i = 0; i < event.streams.length; i++) {
        addStream(event.streams[i]);
    }
}

function streamDestroyedHandler(event) {
    // This signals that a stream was destroyed. Any Subscribers will automatically be removed.
    // This default behaviour can be prevented using event.preventDefault()
}

function sessionDisconnectedHandler(event) {
    // This signals that the user was disconnected from the Session. Any subscribers and publishers
    // will automatically be removed. This default behaviour can be prevented using event.preventDefault()
    publisher = null;

    show('connectLink');
    hide('disconnectLink');
    hide('publishLink');
    hide('unpublishLink');
}

function connectionDestroyedHandler(event) {
    // This signals that connections were destroyed
}

function connectionCreatedHandler(event) {
    // This signals new connections have been created.
}

/*
If you un-comment the call to TB.addEventListener("exception", exceptionHandler) above, OpenTok calls the
exceptionHandler() method when exception events occur. You can modify this method to further process exception events.
If you un-comment the call to TB.setLogLevel(), above, OpenTok automatically displays exception event messages.
*/
function exceptionHandler(event) {
    if (event.code == 1013) {
        document.body.innerHTML = "This page is trying to connect a third client to an OpenTok peer-to-peer session. "
            + "Only two clients can connect to peer-to-peer sessions.";
    } else {
        alert("Exception: " + event.code + "::" + event.message);
    }
}

//--------------------------------------
//  HELPER METHODS
//--------------------------------------

function addStream(stream) {
    // Check if this is the stream that I am publishing, and if so do not publish.
    if (stream.connection.connectionId == session.connection.connectionId) {
        return;
    }
    var subscriberDiv = document.createElement('div'); // Create a div for the subscriber to replace
    subscriberDiv.setAttribute('id', stream.streamId); // Give the replacement div the id of the stream as its id.
    document.getElementById("subscribers").appendChild(subscriberDiv);
    var subscriberProps = {width: VIDEO_WIDTH, height: VIDEO_HEIGHT};
    subscribers[stream.streamId] = session.subscribe(stream, subscriberDiv.id, subscriberProps);
}

function show(id) {
    document.getElementById(id).style.display = 'block';
}

function hide(id) {
    document.getElementById(id).style.display = 'none';
}