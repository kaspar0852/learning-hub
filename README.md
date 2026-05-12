# Global English Marketplace

An international English language learning platform that connects teachers with students worldwide, featuring automatic localization, currency conversion, and personalized teacher discovery.

## 🌍 Features

- **Automatic Location Detection**: Detects user's location and currency using IP-based geolocation
- **Dynamic Currency Conversion**: Displays teacher rates in user's local currency (USD, EUR, GBP, JPY, etc.)
- **Teacher Discovery**: Search and filter teachers by expertise level (Beginner/Advanced)
- **Favorites System**: Save favorite teachers using DynamoDB NoSQL storage
- **Responsive Design**: Modern, mobile-first UI with Tailwind CSS
- **Real-time Updates**: Live price conversion and availability status

## 🏗️ Architecture

### Frontend (React + TypeScript)
- **React 19** with TypeScript for type safety
- **Tailwind CSS** for modern, responsive styling
- **React Query** for data fetching and caching
- **Zustand** for lightweight state management
- **React Router** for navigation

### Backend (.NET Core)
- **ASP.NET Core 9** Web API
- **PostgreSQL** for structured teacher data
- **DynamoDB** for user favorites (NoSQL)
- **External APIs**: ipapi.co (geolocation), ExchangeRate-API (currency conversion)

## 🚀 Quick Start

### Prerequisites

**Required:**
- Docker & Docker Compose installed
- ExchangeRate-API key (free at [exchangerate-api.com](https://www.exchangerate-api.com))

**Optional:**
- Make (for convenience commands - see Option 2)

---

### Option 1: Docker Compose (Recommended) 🐳
#### Manual Docker Commands

If you prefer manual control over Docker:

```bash
# Start all services
docker-compose -f docker-compose.dev.yml up --build

# Run in background
docker-compose -f docker-compose.dev.yml up -d

# Stop services
docker-compose -f docker-compose.dev.yml down

# View logs
docker-compose -f docker-compose.dev.yml logs

# Clean up everything
docker-compose -f docker-compose.dev.yml down -v
```

---

### Option 2: Using Make (Convenience Commands) 🔧

If you have `make` installed, you can use these convenient commands:

#### Installing Make (if not available)

**macOS:**
```bash
# Install Xcode Command Line Tools (includes make)
xcode-select --install

# Or install via Homebrew
brew install make
```

**Ubuntu/Debian:**
```bash
sudo apt update
sudo apt install build-essential
```

**Windows:**
```bash
# Install via Chocolatey
choco install make

# Or via WSL (Recommended for development)
wsl --install
# Then follow Ubuntu instructions inside WSL
```

#### Make Commands

```bash
# One-time setup
make setup

# Start development environment
make dev

# View logs
make logs

# Check status
make status

# Stop services
make stop

# Clean up
make clean
```

That's it! 🎉 The application will be available at:
- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5209  
- **Swagger Docs**: http://localhost:5209/swagger

#### Useful Commands
```bash
make logs        # View service logs
make status      # Check service status
make stop        # Stop all services
make clean       # Clean up containers
make shell-backend # Access backend container
make shell-db     # Access database shell
```

### Option 2: Manual Setup

#### Prerequisites
- Node.js 18+ 
- .NET 9 SDK
- PostgreSQL 14+
- AWS Account (for DynamoDB)
- API Keys (see Configuration section)

#### 1. Clone Repository
```bash
git clone https://github.com/kaspar0852/learning-hub/edit/main
cd learning
```

#### 2. Backend Setup

##### Install Dependencies
```bash
cd backend/src/Marketplace.Api
dotnet restore
```

##### Database Setup
```bash
# Create PostgreSQL database
createdb marketplace

# Run migrations
dotnet ef database update
```

##### Configuration
Create `appsettings.Development.json`:
```json
{
  "GeoIp": {
    "BaseUrl": "http://ip-api.com",
    "FallbackCurrencyCode": "USD",
    "CacheMinutes": 60
  },
  "ExchangeRate": {
    "BaseUrl": "https://v6.exchangerate-api.com",
    "ApiKey": "your-api-key-here",
    "BaseCurrency": "USD"
  },
  "DynamoDb": {
    "TableName": "user-favorites",
    "Region": "us-east-1"
  }
}
```

##### Run Backend
```bash
dotnet run
```

Backend will start on `http://localhost:5209`

#### 3. Frontend Setup

##### Install Dependencies
```bash
cd ../learning-platform-frontned
npm install
```

##### Configuration
Create `.env` file:
```
VITE_API_BASE_URL=http://localhost:5209
VITE_ENABLE_LOCATION_DETECTION=true
VITE_ENABLE_PRICE_CONVERSION=true
VITE_FEATURE_FAVORITES=true
VITE_FEATURE_SEARCH=true
VITE_DEFAULT_CURRENCY=USD
```

##### Run Frontend
```bash
npm run dev
```

Frontend will start on `http://localhost:5173`

## 🐳 Docker Setup (Recommended)

### Services Included
- **PostgreSQL Database**: Port 5433 (external) / 5432 (internal)
- **DynamoDB Local**: Port 8000 (for favorites storage)
- **Backend API**: Port 5209
- **Frontend Web**: Port 5173

### Service Dependencies
- **Backend** waits for PostgreSQL (healthy) and DynamoDB (started)
- **Frontend** runs independently (no hard dependencies)
- **Health checks** ensure proper startup order

### Individual Services
```bash
# Backend
cd backend
docker build -t marketplace-api .
docker run -p 5209:80 marketplace-api

# Frontend
cd learning-platform-frontned
docker build -t marketplace-web .
docker run -p 5173:80 marketplace-web

# PostgreSQL
docker run -d --name postgres -p 5433:5432 -e POSTGRES_DB=marketplace -e POSTGRES_USER=marketplace -e POSTGRES_PASSWORD=marketplace123 postgres:15-alpine

# DynamoDB Local
docker run -d --name dynamodb -p 8000:8000 amazon/dynamodb-local:latest -jar DynamoDBLocal.jar -sharedDb
```

## 🔧 API Keys Setup

### 1. ExchangeRate-API
1. Sign up at [exchangerate-api.com](https://www.exchangerate-api.com)
2. Get your free API key
3. Add to `appsettings.Development.json`

### 2. AWS DynamoDB
1. Create AWS account
2. Create DynamoDB table named `user-favorites`
3. Configure IAM credentials
4. Add region and table name to configuration

### 3. IP-API (Free Tier)
- No API key required for basic usage
- Limited to 45 requests per minute
- Used for geolocation detection

## 📊 Database Schema

### PostgreSQL (Teachers)
```sql
CREATE TABLE teachers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    level INTEGER NOT NULL, -- 1=Beginner, 2=Advanced
    base_price_usd DECIMAL(10,2) NOT NULL
);
```

### DynamoDB (Favorites)
```
Table: user-favorites
Primary Key: session_id (String), teacher_id (String)
Attributes: created_at_utc (String)
```

## 🧪 Testing

### Frontend Tests
```bash
cd learning-platform-frontned
npm test
```

### Backend Tests
```bash
cd backend
dotnet test
```

### Manual Testing Scenarios
1. **Location Detection**: Test with VPN/browser location spoofing
2. **Currency Conversion**: Verify prices in different currencies
3. **Favorites**: Add/remove teachers from favorites list
4. **Filtering**: Test level and search filters

## 🌐 External APIs

### ipapi.co
- **Purpose**: IP-based geolocation and currency detection
- **Endpoint**: `http://ip-api.com/json/?fields=country,countryCode,currency`
- **Cost**: Free (45 requests/minute)

### ExchangeRate-API
- **Purpose**: Real-time currency conversion rates
- **Endpoint**: `https://v6.exchangerate-api.com/v6/{API_KEY}/latest/USD`
- **Cost**: Free (2,000 requests/month)

## 🎯 Design Choices & Trade-offs

### PostgreSQL vs DynamoDB
- **PostgreSQL**: Structured teacher data requiring consistency and complex queries
- **DynamoDB**: High-frequency favorites data requiring low latency and scalability

### Caching Strategy
- **Location Data**: Cached for 1 hour to reduce API calls
- **Exchange Rates**: Daily sync to balance freshness and cost

### Frontend Architecture
- **React Query**: Handles server state and caching automatically
- **Zustand**: Lightweight alternative to Redux for client state

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For questions or support:
- Create an issue in the repository
- Email: support@marketplace.com
- Documentation: [Wiki](https://github.com/your-repo/wiki)
