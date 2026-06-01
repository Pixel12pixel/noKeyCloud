# noKeyCloud

# Deploying noKeyCloud

noKeyCloud is deployed as a fully self-contained Docker stack. All services (Database, Redis, API, and Frontend) run securely inside an isolated Docker network. A bundled Caddy reverse proxy automatically handles routing and HTTPS.

## Prerequisites
* Docker with docker compose installed

## Quick Start Deployment

### 1. Configure the Environment
Copy the example environment file and edit it with your secure credentials:

```bash
cp .env.example .env

```

Open `.env` and configure your setup:

* **`JWT_SECRET`**: You **must** change this to a secure, random string (minimum 32 characters).
* **`DOMAIN_NAME`**:
  * If testing locally, leave it as `localhost`.
  * If deploying to a public server, enter your real domain (e.g., `cloud.example.com`).



### 2. Start the Stack

Run the following command to build the images and start the services in the background:

```bash
docker compose up -d --build

```

---

## HTTPS & SSL Certificates

noKeyCloud enforces HTTPS across all environments.
Depending on your deployment strategy, Caddy will handle SSL in one of two ways:

### Path A: Local Network / LAN Deployment (`DOMAIN_NAME=localhost`)

When set to `localhost` or a local IP, Caddy operates fully offline and issues a **Local Self-Signed Certificate**.

* **The Browser Warning:** Because the Caddy instance lives inside an isolated Docker container, it cannot automatically install its root authority into your host machine's operating system. When you first visit `https://localhost:8743`, your browser will throw a "Not Secure" warning.
* **How to proceed:** Click **Advanced -> Proceed to localhost (unsafe)**. This is entirely safe and normal for local self-hosted applications.

### Path B: Public Production Deployment (`DOMAIN_NAME=yourdomain.com`)

When you provide a public domain, Caddy completely automates the SSL process.

* **Requirements:** Your public domain name must have an A/AAAA record pointing to your server's public IP address, and your firewall/router must forward incoming traffic to the host ports.
* **Automated ACME:** Upon startup, Caddy will contact Let's Encrypt or ZeroSSL, complete an HTTP-01 challenge, obtain a fully trusted, globally recognized SSL certificate, and renew it automatically every 90 days.
* **The Result:** Anyone visiting your domain will see a fully trusted green padlock with zero manual configuration required.

---

## Accessing the Application

* **Local Dev / Test:** Open your browser to `https://localhost:8743` (or `https://<YOUR_SERVER_IP>:8743`).
* **Production:** Open your browser to `https://yourdomain.com:8743`.

*Note: If you are running on a dedicated VPS and want to use the standard web port instead of `8743`, modify the `ports` mapping under the `caddy` service in `docker-compose.yml` to `-"443:443"`.*