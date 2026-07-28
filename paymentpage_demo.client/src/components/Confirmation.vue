//!--Copyright (c) Robert A. Howell  2026
<script setup>
import {ref, onMounted } from 'vue';
import QueryParams from '../Classes/queryparameters.js';

let isLoading = ref(true);
let transactionSucceeded = false;

onMounted( () => {
  const secrets = {
    cryptography: {
      import: (keyBase64) => {
        return globalThis.crypto.subtle.importKey(
          "pkcs8",
          keyBase64,
          {
            name: "RSA-OAEP",
            hash: "SHA-256"
          }, true, ["decrypt"]);
      },
      decryptText: async (decryptionKey, cipherBase64) => {

        const textToDecrypt = Uint8Array.fromBase64(cipherBase64);

        let plaintext = await globalThis.crypto.subtle.decrypt({ name: "RSA-OAEP" }, decryptionKey, textToDecrypt.buffer);

        const decoder = new TextDecoder();
        return decoder.decode(plaintext);
      }
    },
    bus: {
      decrypt: async (key, cipherBase64) => {
        if(
          key == null || cipherBase64 == null
        ) return;

        // Get search params from URL
        const DecryptedSearchParams = new QueryParams(window.location.search);
        const guid = DecryptedSearchParams.getParameterValue('transactionguid');

        if(guid == '') {
          console.error('Guid was not provided.');
          window.location.href="notfound";
        }

        try {
          return await fetch(`/checkout?transactionguid=${guid}`,{ method: "POST", body: cipherBase64 })
            .then(res => res.json())
            .then(data => {
              if(data.error) {
                window.location.href="notfound";

                return;
              }
              if(!data.plaintext)
                return Error("Decryption failed: Plaintext is null or undefined.");

              return data.plaintext.toString();
            });
        }
        catch {
          console.error("Failed to fetch transaction details");
        }
      },
    },
  };

  ( async () => {

    const keys = [
      ["encrypted-name-ciphertext","encrypted-name","decrypted-name"],
      ["encrypted-number-ciphertext","encrypted-number","decrypted-number"],
      ["encrypted-code-ciphertext","encrypted-code","decrypted-code"],
      ["encrypted-month-ciphertext","encrypted-month","decrypted-month"],
    ];

    const PIFS = document.getElementById("PIFS");

    try {
      for (let key of keys) {

        const cipherBase64 = window.sessionStorage.getItem(key[0]);
        if (!cipherBase64) throw new Error("Ciphertext not found");

        let decryptedtext = await secrets.bus.decrypt(key[1], cipherBase64);

        const elem = PIFS.querySelector(`span [data-id="${key[2]}"]`);
        elem.textContent = decryptedtext;

      }
      transactionSucceeded = true;
    }
    catch {
      transactionSucceeded = false;
    }
    finally {
      isLoading.value = false;
    }
  })();
})

</script>

<template>
  <section>
    <p v-if="isLoading">Checking transaction....</p>
    <p v-else-if="transactionSucceeded">The payment transaction has succeeded.</p>
    <p v-else-if="!transactionSucceeded">Confirmation failure.</p>
    <br>
    <hr>
    <br>
    <h2>Output (for demonstration):</h2>
    <br>
    <output
      id="PIFS"
      name="Summary"
      for="PIF"
      style="display: inline-block; border: 1px solid green;">
      <span>Name(text): <strong data-id="decrypted-name"></strong></span><br>
      <span>Card Number(text): <strong data-id="decrypted-number"></strong></span><br>
      <span>CVV Number(text): <strong data-id="decrypted-code"></strong></span><br>
      <span>Expiration Month(text): <strong data-id="decrypted-month"></strong></span>
    </output>
    <br>
    <br>
    <hr>
  </section>
</template>

<style scoped>

</style>
