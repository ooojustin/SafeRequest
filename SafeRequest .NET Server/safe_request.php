<?php

    function output($status, $message, $encryption_key, $extras = null) {
        $response = array('status' => $status ? true : false, 'message' => $message);
        if ($extras != null)
            array_fuse($response, $extras);
        $data = json_encode($response);
        $enc = new Encryption($encryption_key);
        $data = $enc->EncryptString($data);
        die($data);
    }

    function array_fuse(&$arr1, $arr2) {
        foreach ($arr2 as $key => $value)
            $arr1[$key] = $value;
    }

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

?>