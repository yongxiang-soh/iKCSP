/**
 * Fired when document has been loaded successfully.
 */
$(function() {

    // Initiate connection to note notification.
    var signalrConnection = initiateHubConnection(hubList.noteNotification);

    // Handle events when note is sent from domain.
    signalrConnection.client.receiveNoteInformation = function(screenName, message) {
       // Not belong to AW-Material.
        if (screenName != 'aw-product')
            return;
        var text = $('#EditLog').val() + '\n';
        text += message ;
        $('#EditLog').val(text);
    }

    // Start signalr communication
    signalrStartHubConnection();

});