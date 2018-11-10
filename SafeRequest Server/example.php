<?php
    
    include 'safe_request.php';
    define('ENCRYPTION_KEY', 'your encryption key here');
    $sr = new SafeRequest(ENCRYPTION_KEY);

    // automatically access decrypted post data (returns "some_value")
    $sr->output(true, $_POST['some_key']);

    // returning normal encrypted strings (get requests)
    $sr->output(true, 'your message here');

?>