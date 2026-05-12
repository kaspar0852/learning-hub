# Global English Marketplace - Makefile
# Convenient commands for development and deployment

.PHONY: help setup dev build clean stop logs status

# Default target
help:
	@echo "🚀 Global English Marketplace Commands:"
	@echo "=================================="
	@echo "setup      - First-time setup (creates .env, installs deps)"
	@echo "dev         - Start development environment"
	@echo "build       - Build all services"
	@echo "stop        - Stop all services"
	@echo "logs        - Show service logs"
	@echo "status      - Check service status"
	@echo "clean       - Clean up containers and images"
	@echo "test        - Run tests"
	@echo "prod        - Run production environment"
	@echo ""
	@echo "Examples:"
	@echo "  make setup    # First-time setup"
	@echo "  make dev      # Start development"
	@echo "  make logs    # View logs"

# First-time setup
setup:
	@echo "🔧 Setting up for first time..."
	@if [ ! -f .env ]; then \
		cp .env.example .env; \
		echo "✅ Created .env file - please edit with your API keys"; \
		echo "📝 Required: EXCHANGE_RATE_API_KEY"; \
		echo "📝 Optional: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY"; \
	else \
		echo "✅ .env file already exists"; \
	fi
	@echo "📦 Installing backend dependencies..."
	@cd backend && dotnet restore
	@echo "📦 Installing frontend dependencies..."
	@cd learning-platform-frontned && npm install
	@echo "✅ Setup complete!"

# Development environment
dev:
	@echo "🚀 Starting development environment..."
	docker-compose -f docker-compose.dev.yml up --build -d
	@echo "✅ Services starting..."
	@sleep 10
	@echo "📱 Frontend: http://localhost:5173"
	@echo "🔧 Backend: http://localhost:5209"
	@echo "📊 Swagger: http://localhost:5209/swagger"

# Production environment
prod:
	@echo "🏭 Starting production environment..."
	docker-compose -f docker-compose.yml up --build -d
	@echo "✅ Production services started..."

# Build all services
build:
	@echo "🔨 Building all services..."
	@echo "Building backend..."
	@cd backend && dotnet build -c Release
	@echo "Building frontend..."
	@cd learning-platform-frontned && npm run build
	@echo "✅ Build complete!"

# Stop services
stop:
	@echo "🛑 Stopping all services..."
	docker-compose -f docker-compose.dev.yml down
	@echo "✅ Services stopped"

# View logs
logs:
	@if [ -z "$(SERVICE)" ]; then \
		echo "📋 Showing all service logs..."; \
		docker-compose -f docker-compose.dev.yml logs; \
	else \
		echo "📋 Showing logs for $(SERVICE)..."; \
		docker-compose -f docker-compose.dev.yml logs $(SERVICE); \
	fi

# Check status
status:
	@echo "🔍 Checking service status..."
	docker-compose -f docker-compose.dev.yml ps

# Clean up
clean:
	@echo "🧹 Cleaning up..."
	docker-compose -f docker-compose.dev.yml down -v
	docker system prune -f
	@echo "✅ Cleanup complete!"

# Run tests
test:
	@echo "🧪 Running tests..."
	@echo "Running backend tests..."
	@cd backend && dotnet test
	@echo "Running frontend tests..."
	@cd learning-platform-frontned && npm test
	@echo "✅ Tests complete!"

# Watch logs (continuous)
watch:
	@echo "👀 Watching logs (Ctrl+C to stop)..."
	docker-compose -f docker-compose.dev.yml logs -f

# Access individual services
shell-backend:
	@echo "🐚 Opening backend shell..."
	docker-compose -f docker-compose.dev.yml exec backend bash

shell-db:
	@echo "🐚 Opening database shell..."
	docker-compose -f docker-compose.dev.yml exec postgres psql -U marketplace -d marketplace

# Database operations
db-migrate:
	@echo "🗄️ Running database migrations..."
	docker-compose -f docker-compose.dev.yml exec backend dotnet ef database update

db-reset:
	@echo "🔄 Resetting database..."
	docker-compose -f docker-compose.dev.yml exec postgres psql -U marketplace -d marketplace -c "DROP SCHEMA public CASCADE; CREATE SCHEMA public;"
	docker-compose -f docker-compose.dev.yml exec backend dotnet ef database update
	@echo "✅ Database reset complete!"
