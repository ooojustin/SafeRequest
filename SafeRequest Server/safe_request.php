<?php

    /*
    
        === SafeRequest ===
        Developed by Justin Garofolo
        Open source .NET/PHP library to allow encrypted json data to be transferred between client and server.
        Github: https://github.com/ooojustin/SafeRequest.NET
        LICENSE: https://github.com/ooojustin/SafeRequest.NET/blob/master/LICENSE.md
        
    */
    
    class SafeRequest {
        
        var $auth;
        var $enc;
        
        function __construct($key) {
            $this->enc = new Encryption($key);
            $_POST = $this->getPOST();
            $this->auth = $this->enc->DecryptString($_POST['authentication_key']);   
        }
        
        // Decrypts POST data from SafeRequest client.
        // Example: $_POST = GetPost('secret_key');
        function getPOST() {
            $data = file_get_contents('php://input');
            $data = $this->enc->DecryptString($data);
            return json_decode($data, true);
        }
        
        // Returns encrypted JSON information to SafeRequest client.
        // Example: output(true, 'my encrypted string here', 'secret_key');
        function output($status, $message, $extras = null) {
            $response = array('status' => $status ? true : false, 'message' => $message);
            if ($extras != null)
                array_fuse($response, $extras);
            $response['authentication_key'] = $this->auth;
            $data = json_encode($response);
            $data = $this->enc->EncryptString($data);
            die($data);
        }

    }

    
    /* Encryption class to safely communicate with SafeRequest client.
       Example:
           $enc = new Encryption('secret_key);
           $encrypted = $enc->EncryptString('secret message');
           $decrypted = $enc->DecryptString($encrypted);
           echo $decrypted; // outputs 'secret message'
    */
    class Encryption {
    
        var $_key;
        var $_iv;
    
        function __construct($key) {
            $this->_key = substr(hash('sha256', $key, true), 0, 32);
            $this->_iv = chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0) . chr(0x0);
        }
    
        function EncryptString($plainText) {
            return base64_encode(openssl_encrypt($plainText, 'aes-256-cbc', $this->_key, OPENSSL_RAW_DATA, $this->_iv));
        }
    
        function DecryptString($cipherText) {
            return openssl_decrypt(base64_decode($cipherText), 'aes-256-cbc', $this->_key, OPENSSL_RAW_DATA, $this->_iv);
        }
    
        function SetIV($iv) {
            $this->_iv = $iv;
        }
    
    }

    // Combines 2 arrays. ($arr2 gets added to the end of $arr1)
    function array_fuse(&$arr1, $arr2) {
        foreach ($arr2 as $key => $value)
            $arr1[$key] = $value;
    }

?>