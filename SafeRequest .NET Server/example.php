<?php
    
    include 'safe_request.php';
    define('ENCRYPTION_KEY', 'your encryption key here');

    // decrypt & access post data (returns "some_value")
    $data = GetPOST(ENCRYPTION_KEY);
    output(true, $data['some_key'], ENCRYPTION_KEY);

    // returning normal encrypted strings (get requests)
    output(true, 'your message here', ENCRYPTION_KEY);

?>