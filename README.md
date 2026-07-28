<!--
Copyright (c) 2026 Robert A. Howell
Author: Robert A. Howell
Description: Card payment encryption and decryption demonstrate methods in application-layer data protection. This demonstration uses a HTML form and simulates posting payment information to a designated endpoint.
  Warning: This is not a production implementation. It is not recommended to develop your own payment scheme environment without a good security reason to do so.
Created_Date: 2026-01-07
Edited: 2026-07-01
-->

# Payment Information Demo (private repository)  
This demonstration reproduces a credit card form and payment transaction on a website, such as those used in e-commerce websites. Following a real payment processing flow, a merchant bank and payment processor environment is simulated by encrypting and transporting sensitive card information securely. A user submits their payment information and is returned to a confirmation page through a mock transaction by a backend server. This website exemplifies security precations taken in sensitive data environments and encrypts data at the application layer. The source code remains closed-source; however, the process provides a means to test security against e-commerce skimming, XSS attacks, CSP reporting, or other cyber defense methods.  
Hosted page: [Payment Information Demo](https://encryptedpaymentinformation-demo.rhdeveloping.com/)  


## Feature Details  
1. Form validation filter data inputs
2. Asymmetric encryption and decryption
3. Save card data in the browser
4. Encrypted ciphertext is revealed in the gold-bordered output
   >Hint: Cryptograms are produced by submitting the form and consumed after transmitting  
5. Success/Failed processing status


### Demonstration Steps  
1. Enter the form information at the provided URL
2. Click submit
3. The confirmation page completes loading and the demo is complete 

Completing these steps runs the provided inputs through secure data encryption, transport, and decryption.  
  > An e-commerce page may provide a third party to submit the information to. This demonstration complies with and can support enterprise credit libraries. 


## Architecture  
This demonstration provides live encryption and decryption in a client-server space. The server creates an encryption key for the client to encrypt the data. Only the server can decrypt the data, simulating the payment processor after a user requested payment is submitted.  


### Flow Design  
The credit card form text is entered by the user. The payment transaction follows with this process:
1. The user clicks the sumbit button
2. An encrypting key is generated and returned by the backend server
3. The application encrypts the cardholder data
4. The cardholder data is session submitted
5. The browser redirects to the confirmation page
6. The backend server decrypts the data
7. The status is returned of the successful payment processing simulation
8. If the transaction was successful, the user will see success

Steps 4 and 6 differ from the real world because this design has no need to store payment data. An important distinction is data processing. A payment system wants data submitted early for a quick transaction not requiring submission of the session information in step 4. Since the backend simulates the payment processor, the decryption would take place by Mastercard or another payment provider.  


### PCI-DSS compliance  
- E-commerce payment design meeting the SAQ A-EP payment page requirements
- PCI-DSS 4.x
- Scripts meet integrity requirments
- Account data is not stored

**SECURITY NOTE:**  
In security-sharing fashion: be aware that transport encryption is different from application layer encryption. Merely because a website is encrypted does not mean your data is being protected and handled securely.  

## Security Headers  
Security headers have been configured for this website in detail. To a developer, it is common to know that a violation of the Content-Security-Policy(CSP) http header outputs an error message in the developer's tools. It is the intention of W3C to take security a step further by offering a reporting mechanism for such violations. Following security practices, a reporing API has been configured and receives these reports.  

A violation error should not occur with the feature enabled. Should one occur, the backend records the violation's details and uses the below format. Tests show each Chrome, Edge, and Firefox use the reporting API in this configuration.  

Note: Cloudflare may be blocked and also show as an error if the browser settings are configured with strict tracking. This is normal behavior and the browsers send a CSP error report for this, too. The expected, allowed (if configured) error:
  ~~~ text
  ERROR:
   Content-Security-Policy: The page’s settings blocked an inline script (script-src-elem) from being executed because it violates the following directive: "script-src 'self' https://static.cloudflareinsights.com '...'". Consider using a hash ('sha256-...') or a nonce.
   Response should include 'x-content-type-options' header.
  WARNING:
   The resource at “https://static.cloudflareinsights.com/beacon.min.js/v4513226cdae34746b4dedf0b4dfa099e1781791509496” was blocked because Enhanced Tracking Protection is enabled.
  ~~~

### CSP Report  
Required security headers are specific to both the receiving API and sending host (browser) to engage the reporting post method. As of current, the reports are logged to a file not exposed for this demonstration. A documented summary of the browser reports as such:

PREREQUISITES: CSP Reporting endpoint configured of the Payment Information Form to receive a browsers' Reporting API packets  
URL: `https://encryptedpaymentinformation-demo.rhdeveloping.com/form`  
DATE: 7-2-2026  
ATTEMPT: CSP Violation  
ACTION: inline event listener injected into requested code to trigger a violation  
METHOD: HTML inspection edit  
PLAINTEXT: `<!-- <div onclick="alert(1)" style="background: blue;">test</div> -->`  
BROWSERS TESTED: Chrome, Firefox, Edge  
LOGGED RESULTS:  
~~~ text
2026-07-02 21:28:49Z | /csp-reports | [{"age":0,"body":{"blockedURL":"inline","disposition":"enforce","documentURL":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","effectiveDirective":"script-src-attr","lineNumber":1,"originalPolicy":"upgrade-insecure-requests; default-src 'none'; img-src 'self'; frame-ancestors https://www.roberthowell.dev; connect-src 'self' https://encryptedpaymentinformation-demo.rhdeveloping.com; script-src 'self' https://static.cloudflareinsights.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; base-uri 'none'; form-action 'self'; report-to local-endpoint","referrer":"","sample":"","sourceFile":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","statusCode":200},"type":"csp-violation","url":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","user_agent":"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36 Edg/149.0.0.0"}]
2026-07-02 21:25:48Z | /csp-reports | [{"age":0,"body":{"blockedURL":"inline","disposition":"enforce","documentURL":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","effectiveDirective":"script-src-attr","lineNumber":1,"originalPolicy":"upgrade-insecure-requests; default-src 'none'; img-src 'self'; frame-ancestors https://www.roberthowell.dev; connect-src 'self' https://encryptedpaymentinformation-demo.rhdeveloping.com; script-src 'self' https://static.cloudflareinsights.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; base-uri 'none'; form-action 'self'; report-to local-endpoint","referrer":"","sample":"","sourceFile":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","statusCode":200},"type":"csp-violation","url":"https://encryptedpaymentinformation-demo.rhdeveloping.com/form","user_agent":"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/149.0.0.0 Safari/537.36"}]
2026-07-02 21:33:19Z | /csp-reports | [
 {
  "age": 4,
  "type": "csp-violation",
  "url": "https://encryptedpaymentinformation-demo.rhdeveloping.com/form",
  "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:152.0) Gecko/20100101 Firefox/152.0",
  "body": {
 "documentURL": "https://encryptedpaymentinformation-demo.rhdeveloping.com/form",
 "blockedURL": "inline",
 "referrer": null,
 "effectiveDirective": "script-src-attr",
 "originalPolicy": "upgrade-insecure-requests; default-src 'none'; img-src 'self'; frame-ancestors https://www.roberthowell.dev; connect-src 'self' https://encryptedpaymentinformation-demo.rhdeveloping.com; script-src 'self' https://static.cloudflareinsights.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; base-uri 'none'; form-action 'self'; report-to local-endpoint",
 "sourceFile": "resource",
 "sample": null,
 "disposition": "enforce",
 "statusCode": 200,
 "lineNumber": 1632,
 "columnNumber": 20
}

 }
]
~~~

ANALYSIS: The returned document URL(s) prove the report generated from the web page origin. The reporting evidence find attempts were made against an illegal script source attribute element. The CSP in place does not allow inline scripts which has resulted in correct, positive reporting from all three browsers.  
ANNOTATIONS: Each test provides a source code line number affected by the violation. The logged evidence is true: Chrome and Edge were inserted at #1 and Firefox at the end #1632.  

_____

## A peek into the code  

### Key transport  
~~~ JavaScript
// Importing the server's key
const importServerPublicKey = (base64Key) => {
  const binaryKey = Uint8Array.from(atob(base64Key), c => c.charCodeAt(0));

  return globalThis.crypto.subtle.importKey(
    "spki",
    binaryKey,
    {
      name: "RSA-OAEP",
      hash: "SHA-256",
    },
    true,
    ["encrypt"]
  );
}
~~~
