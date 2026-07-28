//!--Copyright (c) Robert A. Howell  2026
<script setup>
import { ref, computed } from 'vue';
import PaymentPageForm from './components/PaymentPageForm.vue'
import Confirmation from "./components/Confirmation.vue";
import NotFound from "./components/NotFound.vue";
import PageNotFound from "./components/PageNotFound.vue";

const routes = {
  '/form': PaymentPageForm,
  '/confirmation': Confirmation,
  '/notfound': NotFound,
  '/pagenotfound': PageNotFound
}

const currentPath = ref(window.location.pathname)

window.addEventListener('pageshow', () => {
  currentPath.value = window.location.pathname
})

let params;

const currentView = computed(() => {
  if(currentPath.value === '/'){
    currentPath.value = '/form'
  }
  return routes[currentPath.value] || routes['/pagenotfound'];
})
</script>

<template>
  <main>
    <h1>Payment Information Demo</h1>
    <br>
    <component :is="currentView" />
    <br>
    <h2>Demonstration detail:</h2>
    <details>
      <br>
      <summary>Details</summary>
      <h3>Payment Information Demo:</h3>
      <ul>
        <li>CHD/PII is protected</li>
        <li>Transport layer (TLS) encryption</li>
        <li>Application layer (source code) encryption</li>
      </ul>
      <br>
      <h3>Client:</h3>
      <ul>
        <li>Encrypts cardholder data</li>
        <li>Simulates the data submission to a payment provider</li>
        <li>Transports the user to the confirmation</li>
      </ul>
      <br>
      <h3>Server:</h3>
      <ul>
        <li>Allegorizes a payment provider:
          <ol>
            <li>Operates as a payment processor</li>
            <li>Exists as a simulation</li>
          </ol>
        </li>
        <li>Creates asymmetric encryption keys</li>
        <li>Decrypts the secret data</li>
      </ul>
    </details>
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
