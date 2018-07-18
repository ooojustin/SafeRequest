<?php
    
    include 'safe_request.php';
    define('ENCRYPTION_KEY', 'your encryption key here');
    $sr = new SafeRequest(ENCRYPTION_KEY);

    // decrypt & access post data (returns "some_value")
    $data = $sr->getPOST();
    $sr->output(true, $data['some_key'], ENCRYPTION_KEY);

    // returning normal encrypted strings (get requests)
    $sr->output(true, 'your message here', ENCRYPTION_KEY);

?>