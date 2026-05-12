#!/bin/bash

# Global English Marketplace - Setup Script
# This script helps new developers get the application running quickly

set -e

echo "🚀 Setting up Global English Marketplace..."
echo "=================================="

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    echo "Visit: https://docs.docker.com/get-docker/"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    echo "Visit: https://docs.docker.com/compose/install/"
    exit 1
fi

echo "✅ Docker and Docker Compose are installed"

# Create environment file if it doesn't exist
if [ ! -f .env ]; then
    echo "📝 Creating .env file from template..."
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

# Load environment variables
source .env

# Check if required API key is set
if [ "$EXCHANGE_RATE_API_KEY" = "your-exchangerate-api-key-here" ]; then
    echo "❌ EXCHANGE_RATE_API_KEY is not set in .env file"
    echo "Please edit .env file with your actual ExchangeRate-API key"
    exit 1
fi

echo "✅ Environment configured"

# Create network if it doesn't exist
if ! docker network inspect marketplace-network &> /dev/null; then
    echo "🌐 Creating Docker network..."
    docker network create marketplace-network
fi

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker-compose down --remove-orphans 2>/dev/null || true

# Build and start services
echo "🔨 Building and starting services..."
echo "This may take a few minutes on first run..."

docker-compose up --build -d

# Wait for services to be ready
echo "⏳ Waiting for services to start..."
sleep 30

# Check if services are running
echo "🔍 Checking service status..."

# Check PostgreSQL
if docker-compose ps postgres | grep -q "Up"; then
    echo "✅ PostgreSQL is running"
else
    echo "❌ PostgreSQL failed to start"
    docker-compose logs postgres
fi

# Check Backend
if docker-compose ps backend | grep -q "Up"; then
    echo "✅ Backend API is running on http://localhost:5209"
else
    echo "❌ Backend API failed to start"
    docker-compose logs backend
fi

# Check Frontend
if docker-compose ps frontend | grep -q "Up"; then
    echo "✅ Frontend is running on http://localhost:5173"
else
    echo "❌ Frontend failed to start"
    docker-compose logs frontend
fi

echo ""
echo "🎉 Setup complete!"
echo "=================================="
echo "📱 Frontend: http://localhost:5173"
echo "🔧 Backend API: http://localhost:5209"
echo "📊 Swagger Docs: http://localhost:5209/swagger"
echo "🗄️ Database: PostgreSQL on localhost:5432"
echo ""
echo "🔧 Useful commands:"
echo "  View logs:     docker-compose logs [service-name]"
echo "  Stop services: docker-compose down"
echo "  Restart:       docker-compose restart [service-name]"
echo "  Remove all:    docker-compose down -v"
