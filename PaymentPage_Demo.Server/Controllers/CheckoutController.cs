//!--Copyright (c) Robert A. Howell  2026

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PaymentPage_Demo.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckoutController : ControllerBase
{
    private static System.Collections.Generic.Dictionary<string, RSA> _keys = new();
    
    [HttpPost]
    [Route("/checkout/generate")]
    public IActionResult Index()
    {
        var rsa = RSA.Create(2048);  
        // Export the public key in a format the browser likes (SPKI)
        var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
        var publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);

        // You must store the 'rsa' instance (or its Private Key) 
        Guid guid = Guid.NewGuid();
        _keys.Add(guid.ToString(), rsa);
        // in a cache or session to decrypt the POST later!
    
        return Ok(new { publicKey = publicKeyBase64, transactionGuid = guid.ToString() });
    }

    [HttpPost]
    [Route("/checkout")]
    public async Task<IActionResult> OnPost([FromQuery] string transactionguid)
    {
        if (string.IsNullOrEmpty(transactionguid))
            throw new ArgumentNullException(nameof(transactionguid));
        
        if (!_keys.TryGetValue(transactionguid, out var rsaDecrypt))
            return Accepted( new { error = "Transaction not found." });
        
        rsaDecrypt = _keys[transactionguid];
        using var reader = new StreamReader(Request.Body);
        var cypherText = await reader.ReadToEndAsync();
        
        var plainTextBytes = rsaDecrypt.Decrypt(Convert.FromBase64String(cypherText), RSAEncryptionPadding.OaepSHA256);
        
        return Ok(new { plaintext = System.Text.Encoding.UTF8.GetString(plainTextBytes) });
    }
}