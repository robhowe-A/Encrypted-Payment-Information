<!--
Copyright (c) 2026 Robert A. Howell
Author: Robert A. Howell
Description: Card payment encryption and decryption demonstrate methods in application-layer encryption architecture. This demonstration uses an HTML form, JavaScript event handling, and browser cryptography.
  Warning: This is not a production implementation. It is not recommended to develop your own payment scheme environment without a good security reason to do so.
Created_Date: 2026-01-07
Edited: 2026-02-17
-->

# Payment Information Demo  
This folder showcases a demonstration of a card payment used in the browser. A user enters their payment information and returns to see the data decrypted after transport.  
Hosted page: https://encryptedpaymentinformation-demo.rhdeveloping.com/form.html  

### Important note  
Note: After submission, you should see a 404. This is because of the POST method submitted where the browser expected a GET. This is normal. Click on the URL and press enter to resume/complete the demo. The steps in full are:
1. Enter the form information at the provided URL and click to submit
2. Click the URL and press enter to refresh the page

Running the demo demonstrates secure data encryption and decryption.  

Secure cryptograms are transmitted via browser session API. There are no networked servers; therefore, the browser simulates the data transfer in place of a networked server.  

## Details  
1. JavaScript handles form submission, encryption, and decryption
2. RSA-OAEP encryption and decryption
3. See encrypted ciphertext revealed in the gold-bordered output
   >Hint: Cryptograms are produced by submitting the form and consumed after transmitting
4. Form validation filters the cardholder data inputs


## Architecture  
The browser encrypts and decrypts payment information. This architecture demonstrates client cryptography, which can be used in applications like payment processors, federated applications or even service workers.  The browser encrypts and decrypts data in the browser session storage.  

### PCI-DSS compliance:
- Designed for web payments and to meet the SAQ A-EP payment page requirements
- PCI-DSS 4.x
- No PAN storage
- All scripts are implemented with integrity check

### Encrypt function call  
//cyphertext creation  
~~~ Javascript
globalThis.crypto.subtle.encrypt({name: "RSA-OAEP"}, key, plaintext);
~~~

### Session storage  
//window session is the transmitted medium  
~~~ Javascript
window.sessionStorage.setItem(KeyName, PEMKey);
~~~

As shown above, from the code, the session storage holds the decryption key.  

**SECURITY NOTE:**  
-A secret key is not securely generated on a client system in secure environments
-Because there is no server, the private key would be available to intercept with XSS (cross-site-scripting) and the browser debugger

Please note: Transport encryption is different from application layer encryption. Merely because a page is encrypted does not mean your data is being protected and handled securely.  
_____

## A peek into the code  

### Key transport  
//store the key for later decryption  
~~~ JavaScript
const KeyStorage = new Promise((resolve, reject) => {
     return resolve(secrets.cryptography.exportKey(keyPair.privateKey));
 });
~~~

### Example encryption  
//example output shows example ciphertext  
~~~ JavaScript
case "number":
b = PSEL.querySelector('[data-id="validated-number"]').textContent = a.value;

secrets.bus.encrypt(a.value, IMPSEL.querySelector('[data-id="encrypted-number"]'), "encrypted-number");

break;
~~~


