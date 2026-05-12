#!/bin/bash

# Simple Docker Compose Starter - No make required
# Alternative to make commands for quick setup

set -e

echo "🚀 Global English Marketplace - Quick Docker Start"
echo "=================================================="

# Check if .env exists and is configured
if [ ! -f .env ]; then
    echo "❌ .env file not found. Creating from template..."
    cp .env.example .env
    echo ""
    echo "⚠️  IMPORTANT: Please edit .env file with your API keys:"
    echo "   - EXCHANGE_RATE_API_KEY (Required)"
    echo "   - AWS_ACCESS_KEY_ID (Optional - for DynamoDB)"
    echo "   - AWS_SECRET_ACCESS_KEY (Optional - for DynamoDB)"
    echo ""
    echo "📝 Edit the .env file, then run this script again."
    exit 1
fi

# Check if required API key is set
source .env
if [ "$EXCHANGE_RATE_API_KEY" = "your-exchangerate-api-key-here" ]; then
    echo "❌ EXCHANGE_RATE_API_KEY is not set in .env file"
    echo "Please edit .env file with your actual ExchangeRate-API key"
    exit 1
fi

echo "✅ Environment configured"

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose -f docker-compose.dev.yml down --remove-orphans 2>/dev/null || true

# Build and start services
echo "🔨 Building and starting services..."
echo "This may take a few minutes on first run..."

docker-compose -f docker-compose.dev.yml up --build -d

# Wait for services to be ready
echo "⏳ Waiting for services to start..."
sleep 30

# Check if services are running
echo "🔍 Checking service status..."

# Check PostgreSQL
if docker-compose -f docker-compose.dev.yml ps postgres | grep -q "Up"; then
    echo "✅ PostgreSQL is running"
else
    echo "❌ PostgreSQL failed to start"
    docker-compose -f docker-compose.dev.yml logs postgres
fi

# Check Backend
if docker-compose -f docker-compose.dev.yml ps backend | grep -q "Up"; then
    echo "✅ Backend API is running on http://localhost:5209"
else
    echo "❌ Backend API failed to start"
    docker-compose -f docker-compose.dev.yml logs backend
fi

# Check Frontend
if docker-compose -f docker-compose.dev.yml ps frontend | grep -q "Up"; then
    echo "✅ Frontend is running on http://localhost:5173"
else
    echo "❌ Frontend failed to start"
    docker-compose -f docker-compose.dev.yml logs frontend
fi

echo ""
echo "🎉 Setup complete!"
echo "=================================================="
echo "📱 Frontend: http://localhost:5173"
echo "🔧 Backend API: http://localhost:5209"
echo "📊 Swagger Docs: http://localhost:5209/swagger"
echo "🗄️ Database: PostgreSQL on localhost:5432"
echo ""
echo "🔧 Useful commands:"
echo "  View logs:     docker-compose -f docker-compose.dev.yml logs"
echo "  Stop services: docker-compose -f docker-compose.dev.yml down"
echo "  Restart:       docker-compose -f docker-compose.dev.yml restart"
echo "  Remove all:    docker-compose -f docker-compose.dev.yml down -v"
