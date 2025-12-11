# 🚀 SkyBot Universe - Production Deployment Guide

## Prerequisites

- Linux server (Ubuntu 22.04 LTS recommended)
- Docker & Docker Compose installed
- cTrader account with API access
- Domain name (optional, for API)

---

## 🖥️ Server Setup

### Recommended Providers

| Provider | Instance | vCPU | RAM | Storage | Bots Capacity | Cost/Month |
|----------|----------|------|-----|---------|---------------|------------|
| **Hetzner** | CX22 | 2 | 4GB | 40GB | 20-50 bots | €5.83 (~$6) |
| **Hetzner** | CPX31 | 4 | 8GB | 160GB | 100+ bots | €11.00 (~$12) |
| **DigitalOcean** | Basic | 2 | 4GB | 80GB | 10-20 bots | $12 |

### Initial Server Setup

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo apt install docker-compose-plugin -y

# Create non-root user
sudo adduser skybot
sudo usermod -aG docker skybot
su - skybot
```

---

## 📦 Deploy SkyBot

### 1. Clone Repository

```bash
cd ~
git clone https://github.com/yourusername/SkyBot.git
cd SkyBot
```

### 2. Configure Environment

```bash
# Copy example environment file
cp .env.example .env

# Edit with your credentials
nano .env
```

Required variables:
```bash
CTRADER_CLIENT_ID=your_actual_client_id
CTRADER_SECRET=your_actual_secret
CTRADER_ACCOUNT_ID_1=your_account_id_1
CTRADER_ACCOUNT_ID_2=your_account_id_2
DB_PASSWORD=strong_password_here
```

### 3. Build Images

```bash
# Build bot image
docker build -t skybot-atlas:latest .

# Or use docker-compose
docker-compose build
```

### 4. Start Services

```bash
# Start all services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f atlas-001
```

### 5. Verify Deployment

```bash
# Check bot is running
docker ps

# Check bot logs
docker logs skybot-atlas-001

# Should see:
# 🚀 SkyCoreAtlas v1.0.0 iniciado
#    Motor: SignalEngine
#    Escudo: RiskEngine
#    Reactor: RecoveryEngine
#    Sensores: HorizontalDetector
```

---

## 🔧 Managing Bots

### Start/Stop Bots

```bash
# Stop all bots
docker-compose stop

# Start all bots
docker-compose start

# Restart specific bot
docker-compose restart atlas-001

# Stop and remove everything
docker-compose down
```

### Scale Bots

```bash
# Add more bot instances
# Edit docker-compose.yml and add:
atlas-003:
  build: .
  environment:
    - CTRADER_ACCOUNT_ID=${CTRADER_ACCOUNT_ID_3}
    - BOT_NAME=SkyCoreAtlas-003

# Then restart
docker-compose up -d
```

### View Logs

```bash
# Follow logs for all bots
docker-compose logs -f

# Follow specific bot
docker-compose logs -f atlas-001

# Last 100 lines
docker-compose logs --tail=100 atlas-001

# Filter by error
docker-compose logs | grep ERROR
```

### Update Bots

```bash
# Pull latest code
git pull

# Rebuild images
docker-compose build

# Recreate containers
docker-compose up -d --force-recreate
```

---

## 📊 Monitoring

### Resource Usage

```bash
# Docker stats
docker stats

# Disk usage
docker system df

# Memory per container
docker stats --no-stream --format "table {{.Name}}\t{{.MemUsage}}"
```

### Bot Health

```bash
# Check if bot is responding
docker exec skybot-atlas-001 ps aux

# Check network connectivity
docker exec skybot-atlas-001 ping -c 4 8.8.8.8
```

### Database

```bash
# Access PostgreSQL
docker exec -it skybot-postgres psql -U skybot

# View tables
\dt

# View sessions
SELECT * FROM robot_sessions ORDER BY created_at DESC LIMIT 10;

# View trades
SELECT * FROM trades_log ORDER BY timestamp DESC LIMIT 20;
```

---

## 🔒 Security Best Practices

### 1. Firewall Configuration

```bash
# Allow SSH
sudo ufw allow 22/tcp

# Allow PostgreSQL only from localhost
sudo ufw deny 5432/tcp

# Enable firewall
sudo ufw enable
```

### 2. Secure API Credentials

```bash
# Never commit .env file
echo ".env" >> .gitignore

# Use environment variables
# Avoid hardcoding secrets in code
```

### 3. Regular Backups

```bash
# Backup PostgreSQL
docker exec skybot-postgres pg_dump -U skybot skybot > backup_$(date +%Y%m%d).sql

# Backup with cron (daily at 2 AM)
crontab -e
# Add:
0 2 * * * docker exec skybot-postgres pg_dump -U skybot skybot > ~/backups/skybot_$(date +\%Y\%m\%d).sql
```

### 4. Update Regularly

```bash
# System updates
sudo apt update && sudo apt upgrade -y

# Docker images
docker-compose pull
docker-compose up -d
```

---

## 🐛 Troubleshooting

### Bot Not Starting

```bash
# Check logs
docker-compose logs atlas-001

# Common issues:
# 1. Invalid cTrader credentials
#    → Verify CTRADER_CLIENT_ID and CTRADER_SECRET in .env

# 2. Database not ready
#    → Wait for postgres healthcheck: docker-compose ps

# 3. Network issues
#    → Check internet: docker exec atlas-001 ping 8.8.8.8
```

### Bot Crashes

```bash
# View crash logs
docker-compose logs --tail=200 atlas-001

# Restart bot
docker-compose restart atlas-001

# If persistent, check:
# - Memory limits
# - Disk space
# - cTrader API status
```

### High Memory Usage

```bash
# Check memory
docker stats --no-stream

# Reduce bot count or upgrade server
# Add memory limit to docker-compose.yml:
deploy:
  resources:
    limits:
      memory: 512M
```

### Database Issues

```bash
# Reset database
docker-compose down
docker volume rm skybot_postgres_data
docker-compose up -d

# Restore from backup
cat backup_20250112.sql | docker exec -i skybot-postgres psql -U skybot
```

---

## 📈 Performance Optimization

### 1. Resource Limits

```yaml
# In docker-compose.yml
services:
  atlas-001:
    deploy:
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
        reservations:
          cpus: '0.25'
          memory: 256M
```

### 2. Logging Optimization

```yaml
# Limit log size
logging:
  driver: "json-file"
  options:
    max-size: "10m"
    max-file: "3"
```

### 3. Database Tuning

```bash
# Increase PostgreSQL shared buffers
# Edit postgresql.conf in container or mount custom config
```

---

## 🔄 CI/CD Pipeline (Optional)

### GitHub Actions Example

```yaml
name: Deploy SkyBot

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2

      - name: Deploy to server
        uses: appleboy/ssh-action@master
        with:
          host: ${{ secrets.SERVER_HOST }}
          username: ${{ secrets.SERVER_USER }}
          key: ${{ secrets.SSH_PRIVATE_KEY }}
          script: |
            cd ~/SkyBot
            git pull
            docker-compose build
            docker-compose up -d
```

---

## 📞 Support

If you encounter issues:

1. Check logs: `docker-compose logs -f`
2. Verify environment: `cat .env`
3. Test connectivity: `docker exec atlas-001 ping api.ctrader.com`
4. Review documentation: `README.md`, `docs/architecture.md`

---

## ✅ Post-Deployment Checklist

- [ ] Bots are running: `docker ps`
- [ ] No errors in logs: `docker-compose logs`
- [ ] Database is healthy: `docker exec skybot-postgres pg_isready`
- [ ] cTrader API authenticated successfully
- [ ] Firewall configured
- [ ] Backups scheduled
- [ ] Monitoring in place
- [ ] Environment variables secured
- [ ] Resource limits set
- [ ] Documentation reviewed

---

**Your SkyBot Universe is now running in production! 🚀**
