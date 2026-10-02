//!--Copyright (c) Robert A. Howell  2026
<script setup>
import { ref, onMounted } from 'vue'

let transactionGuid = ref(null);

const generateServerKey = async () => {
    return await fetch(`/checkout/generate`, { method: 'POST' })
      .then(async data => {
        const dataJson = await data.json();
        transactionGuid.value.value = dataJson.transactionGuid;
        let KeyData = dataJson.PublicKey || dataJson.publicKey;

        if (!KeyData) {
          throw new Error("Public key property missing in server response");
        }
        let test = importServerPublicKey(KeyData);
        return test;
      });
}

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

onMounted( ()=> {
  const secrets = {
    cryptography: {
      new: async () => {
          return globalThis.crypto.subtle.generateKey(
            {
              name: "RSA-OAEP",
              modulusLength: 2048,
              publicExponent: new Uint8Array([1, 0, 1]),
              hash: "SHA-256",
            }, true, ["encrypt", "decrypt"]);
      },
      newCheckout: async (KeyName) => { return generateServerKey(); },
      encryptText: async (key, plaintext) => {
        let ciphertext = await globalThis.crypto.subtle.encrypt({name: "RSA-OAEP"}, key, plaintext);

        return new Uint8Array(ciphertext);
      },
      exportKey: async (key) => {
        return await globalThis.crypto.subtle.exportKey("pkcs8", key);
      }
    },
    bus: {
      encrypt: async (plaintext, ViewExampleElement, KeyName, checkoutText) => {
        if(
          plaintext == null || !plaintext.length > 0
        ) return;

        const a = ViewExampleElement;

        let enc = new TextEncoder();

        let text;

        if(a !== null) {
          //form output demonstration
          text = secrets.cryptography.new();
        }
        else {
          //submit clicked
         text = checkoutText;
        }

        return text
          .then(keyPair => {
            if(a !== null) {
              //browser crypto used for demonstration
              keyPair = keyPair.publicKey;
            }

            return secrets.cryptography.encryptText(keyPair, enc.encode(plaintext))
              .then(data => {
                let dataBase64 = data.toBase64();
                if(a !== null) {
                  a.textContent = dataBase64; return; }

                window.sessionStorage.setItem(`${KeyName}-ciphertext`, dataBase64.toString());
                return dataBase64.toString();
              });
          });
      },
    },
  };

  const SecureForm = {
    Initialize: (OutputElement) => {
      document.forms.PIF.addEventListener("submit", async (e) => {
        e.preventDefault();

        document.forms.PIF.style.display = "none";
        const formdata = new FormData(e.currentTarget);

        const nameVal = formdata.get("cardholder-name");
        const numberVal = formdata.get("credit-number");
        const codeVal = formdata.get("card-verification-number");
        const monthVal = formdata.get("card-expiration-month");

        const chnInput = document.querySelector('input[name="cardholder-name"]');
        const ccnInput = document.querySelector('input[name="credit-number"]');
        const cvvInput = document.querySelector('input[name="card-verification-number"]');
        const cemInput = document.querySelector('input[name="card-expiration-month"]');
        chnInput.remove();
        ccnInput.remove();
        cvvInput.remove();
        cemInput.remove();

        let text = secrets.cryptography.newCheckout();

        const secretNameInput = document.forms.PIF.insertAdjacentElement('afterbegin', document.createElement("input"));
        secretNameInput.name = "cardholder-name";
        secretNameInput.value = await secrets.bus
          .encrypt(nameVal, null, "encrypted-name", text);

        const secretCardNumberInput = document.forms.PIF.insertAdjacentElement("afterbegin", document.createElement("input"));
        secretCardNumberInput.name = "credit-number";
        secretCardNumberInput.value = await secrets.bus
          .encrypt(numberVal, null, "encrypted-number", text);

        const secretCvvInput = document.forms.PIF.insertAdjacentElement("afterbegin", document.createElement("input"));
        secretCvvInput.name = "card-verification-number";
        secretCvvInput.value = await secrets.bus
          .encrypt(codeVal, null, "encrypted-code", text);

        const secretEmInput = document.forms.PIF.insertAdjacentElement("afterbegin", document.createElement("input"));
        secretEmInput.name = "card-expiration-month";
        secretEmInput.value = await secrets.bus
          .encrypt(monthVal, null, "encrypted-month", text);

        // Perform the POST via fetch, then redirect to GET /confirmation
        const data = new FormData(e.target);
        try {
          await fetch(e.target.action, {
            method: 'POST',
            body: data
          }).then(res => {
            if (res.status === 405 || res.status === 404) {
              // Continue the demonstration and refresh the page
              console.info("Expected client 400 response");
              window.location.href = `/confirmation?transactionguid=${transactionGuid.value.value}`;
            } else if (res.ok) { //does not exist
              window.location.href = "/confirmation";
            } else {
              console.error("Unexpected response:", res.status, res.statusText);
            }
          });
        } catch (error) {
          console.info("Error fetching data:", error);
        }
      })
    }
  };

  (() => {
    const IMPSEL = document.forms.PIF.querySelector("#PIFIM");

    SecureForm.Initialize(IMPSEL);

    const PIFELS = document.forms.PIF.querySelectorAll("fieldset input[data-validate]");
    const PSEL = document.forms.PIF.querySelector("#PIFS");

    const updateOutput = (e) => {
      let a,b = Object;

      if(e instanceof InputEvent || e instanceof Event){
        e.preventDefault();
        a = e.target;
      }
      else a = e;

      switch(a.dataset.validate) {
        case "name":
          b = PSEL.querySelector('[data-id="validated-name"]').textContent = a.value;

          secrets.bus.encrypt(a.value, IMPSEL.querySelector('[data-id="encrypted-name"]', null), "encrypted-name");

          break;
        case "number":
          b = PSEL.querySelector('[data-id="validated-number"]').textContent = a.value;

          secrets.bus.encrypt(a.value, IMPSEL.querySelector('[data-id="encrypted-number"]', null), "encrypted-number");

          break;
        case "code":
          b = PSEL.querySelector('[data-id="validated-code"]').textContent = a.value;

          secrets.bus.encrypt(a.value, IMPSEL.querySelector('[data-id="encrypted-code"]', null), "encrypted-code");

          break;
        case "month":
          b = PSEL.querySelector('[data-id="validated-month"]').textContent = a.value;

          secrets.bus.encrypt(a.value, IMPSEL.querySelector('[data-id="encrypted-month"]', null), "encrypted-month");

          break;
      }
      if (b == null) console.error("Element not verified.");
    }

    PIFELS.forEach(i => {
      updateOutput(i);
      i.addEventListener("input", updateOutput);
    });
  })();
})

</script>

<template>
  <p><strong>Important Note:</strong>
    <br>The below form simulates a payment processing flow, encrypting sensitive card information before submission.
  </p>
  <br>
  <p>README: <a href="https://docs.rhdeveloping.com/content/payment-information-demo-readme.html" title="Payment Information Demo — Docs" rel="nofollow noreferrer" target="_blank">Payment Information Demo — Docs</a><br><br></p><hr><br><br>
    <form
      action="confirmation"
      method="post"
      accept-charset="UTF-8"
      name="PIF"
      enctype="multipart/form-data">
      <fieldset>
        <legend>Payment Information</legend>
        <div>
          <label>Cardholder Name:
            <input
              name="cardholder-name"
              data-validate="name"
              type="text"
              spellcheck="false"
              required
              placeholder="Firstname Lastname"
              style="appearance: textfield;"
              autofocus
              autocomplete="cc-name"
              autocorrect="off"
              pattern="^[A-Za-z]{1,64}\s[A-Za-z]{1,64}$"
              title="Accepted Input: First Last [a-z]">
          </label>
        </div>
        <div>Credit Card Information:
          <div id="credit-card-information">
            <div id="credit-card-number">
              <label>
                <input
                  name="credit-number"
                  data-validate="number"
                  type="text"
                  spellcheck="false"
                  required
                  placeholder="####-####-####-####"
                  autocomplete="cc-number"
                  autocorrect="off"
                  pattern="^\d{4}(-)\d{4}\1\d{4}\1\d{4}$"
                  title="Accepted Input: ####-####-####-####">
              </label>
              <label id="card-expiration-month">
                <input
                  name="card-expiration-month"
                  data-validate="month"
                  type="month"
                  spellcheck="false"
                  required
                  placeholder="YYYY-MM"
                  autocomplete="cc-exp"
                  autocorrect="off"
                  pattern="^(202[6-9]|20[3-9]\d{1})(-|/)(0[1-9]|1[012])$"
                  title="Accepted Input: >2026, YYYY-MM">
              </label>
            </div>
          </div>
          <div>
            <label id="card-verification-number">Security Code:
              <input
                name="card-verification-number"
                data-validate="code"
                type="text"
                spellcheck="false"
                required
                placeholder="CVV"
                autocomplete="cc-csc"
                autocorrect="off"
                pattern="^\d{3}$"
                title="Accepted Input: ###">
            </label>
          </div>
        </div>
        <input type="reset">
        <input type="submit">
      </fieldset>
      <input
        id="Transaction-Guid"
        type="hidden"
        spellcheck="false"
        ref='transactionGuid'>
      <input
        id="Origin-Site"
        type="hidden"
        spellcheck="false"
        value="00000000-00000000-00000000-00000000">
      <br>
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
        <span>Name(text): <strong data-id="validated-name"></strong></span><br>
        <span>Card Number(text): <strong data-id="validated-number"></strong></span><br>
        <span>CVV Number(text): <strong data-id="validated-code"></strong></span><br>
        <span>Expiration Month(text): <strong data-id="validated-month"></strong></span>
      </output>
      <br>
      <br>
      <output
        id="PIFIM"
        name="Intermediate"
        for="PIF"
        style="display: inline-block; border: 1px solid goldenrod;">
        <span>Name(encrypted): <strong data-id="encrypted-name"></strong></span><br>
        <span>Card Number(encrypted): <strong data-id="encrypted-number"></strong></span><br>
        <span>CVV Number(encrypted): <strong data-id="encrypted-code"></strong></span><br>
        <span>Expiration Month(encrypted): <strong data-id="encrypted-month"></strong></span>
      </output>
    </form>
    <br><br><hr>

</template>

<style scoped>

</style>
