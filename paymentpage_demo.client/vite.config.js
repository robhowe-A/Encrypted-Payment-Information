import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import htmlSRI from 'vite-plugin-html-sri';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "paymentpage_demo.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7290';

const devConnectSrc = "'self' https://encryptedpaymentinformation-demo.rhdeveloping.com";

const csp = [
  "upgrade-insecure-requests",
  "default-src 'none'",
  "img-src 'self'",
  "frame-ancestors https://www.roberthowell.dev",
  `connect-src ${devConnectSrc}`,
  "script-src 'self' https://static.cloudflareinsights.com",
  "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com",
  "base-uri 'none'",
  "form-action 'self'"
].join("; ");


export default defineConfig(({ command })=> ({
    plugins: [
      plugin(),
      command === 'build' ? htmlSRI({
        hashAlgorithm: 'sha384',
        crossorigin: 'anonymous',
        enabled: true,
        exclude: ['**/*.map'],
        reportOnly: false,
        integrityPropertyName: 'integrity',
        hashPrefix: 'sha384-',
        external: true
      }) : null,].filter(Boolean),
    base: '/',
    build: {
      EmptyOutDir: true,
      sourcemap: false,
      minify: 'esbuild'
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: parseInt(env.DEV_SERVER_PORT || '60492'),
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
      },
      headers: {
        "Access-Control-Allow-Origin": "https://encryptedpaymentinformation-demo.rhdeveloping.com/",
        "Content-Security-Policy": csp,
        //"Integrity-Policy": "blocked-destinations=(script)",
        "Permissions-Policy": "geolocation=(), microphone=()",
        "Referrer-Policy": "strict-origin-when-cross-origin",
        "Strict-Transport-Security": "max-age=15768000; includeSubDomains",
        "Content-Security-Policy-Report-Only": "default-src 'self'; img-src 'self'; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; report-to local-endpoint",
        "Reporting-Endpoints": 'local-endpoint="https://reporting.rhdeveloping.com/csp-reports"',
        "X-Content-Type-Options": "nosniff"
      },
      proxy: {
        '^/checkout': {
          target,
          secure: false
        }
      },
    }
}));
