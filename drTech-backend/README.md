# DrTech Backend API

A comprehensive healthcare management system backend supporting multiple database providers (PostgreSQL, MongoDB, Neo4j).

## 🚀 Quick Start

### Prerequisites

Before running this project, ensure you have the following installed:

- **.NET 8.0 SDK** or later
- **Database** (choose one):
  - **PostgreSQL** (recommended for development)
  - **MongoDB** 
  - **Neo4j**

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd drTech-backend
```

### 2. Environment Setup

1. Copy the environment template:
```bash
cp env.example .env
```

2. Edit `.env` file with your configuration:
```bash
# Choose your database provider
DATABASE_PROVIDER=PostgreSQL  # Options: PostgreSQL, MongoDB, Neo4j

# Configure your chosen database (see sections below)
```

### 3. Database Setup

#### Option A: PostgreSQL (Recommended for Development)

1. **Install PostgreSQL:**
   - Windows: Download from [postgresql.org](https://www.postgresql.org/download/windows/)
   - macOS: `brew install postgresql`
   - Linux: `sudo apt-get install postgresql postgresql-contrib`

2. **Create Database:**
```sql
CREATE DATABASE drtech;
CREATE USER drtech_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE drtech TO drtech_user;
```

3. **Update .env:**
```env
DATABASE_PROVIDER=PostgreSQL
POSTGRES_CONNECTION_STRING=Host=localhost;Database=drtech;Username=drtech_user;Password=your_password
```

#### Option B: MongoDB

1. **Install MongoDB:**
   - Windows: Download from [mongodb.com](https://www.mongodb.com/try/download/community)
   - macOS: `brew install mongodb-community`
   - Linux: Follow [MongoDB installation guide](https://docs.mongodb.com/manual/installation/)

2. **Start MongoDB:**
```bash
# Windows
net start MongoDB

# macOS/Linux
brew services start mongodb-community
# or
sudo systemctl start mongod
```

3. **Update .env:**
```env
DATABASE_PROVIDER=MongoDB
MONGO_CONNECTION_STRING=mongodb://localhost:27017
MONGO_DATABASE=dr-tech
```

#### Option C: Neo4j

1. **Install Neo4j:**
   - Download from [neo4j.com](https://neo4j.com/download/)
   - Or use Docker: `docker run -p 7474:7474 -p 7687:7687 neo4j:latest`

2. **Start Neo4j:**
   - Desktop: Launch Neo4j Desktop
   - Docker: Container should start automatically
   - Command line: `neo4j start`

3. **Update .env:**
```env
DATABASE_PROVIDER=Neo4j
NEO4J_URI=neo4j://localhost:7687
NEO4J_USER=neo4j
NEO4J_PASSWORD=your_password
NEO4J_DATABASE=neo4j
```

### 4. Google OAuth Setup (Optional)

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Update `.env`:
```env
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
```

### 5. Run the Application

```bash
# Install dependencies
dotnet restore

# Run the application
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5036
- **Swagger UI**: http://localhost:5036/swagger

## 📊 Database Switching

This project supports switching between databases without code changes. Simply update the `DATABASE_PROVIDER` in your `.env` file:

```env
# For PostgreSQL
DATABASE_PROVIDER=PostgreSQL

# For MongoDB  
DATABASE_PROVIDER=MongoDB

# For Neo4j
DATABASE_PROVIDER=Neo4j
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `DATABASE_PROVIDER` | Database to use | `PostgreSQL` | No |
| `POSTGRES_CONNECTION_STRING` | PostgreSQL connection | - | If using PostgreSQL |
| `MONGO_CONNECTION_STRING` | MongoDB connection | `mongodb://localhost:27017` | If using MongoDB |
| `MONGO_DATABASE` | MongoDB database name | `dr-tech` | If using MongoDB |
| `NEO4J_URI` | Neo4j connection URI | `neo4j://localhost:7687` | If using Neo4j |
| `NEO4J_USER` | Neo4j username | `neo4j` | If using Neo4j |
| `NEO4J_PASSWORD` | Neo4j password | - | If using Neo4j |
| `JWT_KEY` | JWT signing key | - | Yes |
| `JWT_ISSUER` | JWT issuer | `drtech` | No |
| `JWT_AUDIENCE` | JWT audience | `drtech-clients` | No |
| `GOOGLE_CLIENT_ID` | Google OAuth client ID | - | For Google auth |
| `GOOGLE_CLIENT_SECRET` | Google OAuth client secret | - | For Google auth |

### Production Deployment

For production, use environment variables instead of `.env` file:

```bash
export DATABASE_PROVIDER=PostgreSQL
export POSTGRES_CONNECTION_STRING="Host=prod-server;Database=drtech;Username=prod_user;Password=secure_password"
export JWT_KEY="your-super-secure-jwt-key-here"
```

## 🏥 API Endpoints

### Core Entities
- **Hospitals**: `/api/Hospitals`
- **Departments**: `/api/Departments` 
- **Doctors**: `/api/Doctors`
- **Patients**: `/api/Patients`
- **Services**: `/api/Services`
- **Reservations**: `/api/Reservations`

### Business Logic
- **Insurance Agencies**: `/api/Agencies`
- **Contracts**: `/api/Contracts`
- **Price List**: `/api/PriceList`
- **Payments**: `/api/Payments`

### Authentication
- **Register**: `POST /api/Auth/register`
- **Login**: `POST /api/Auth/login`
- **Refresh Token**: `POST /api/Auth/refresh`
- **Google OAuth**: `POST /api/Auth/google`

## 🛠️ Development

### Project Structure
```
drTech-backend/
├── Controllers/          # API Controllers
├── Domain/              # Business entities
├── Infrastructure/      # Database, auth, etc.
├── Middleware/          # Custom middleware
└── Program.cs           # Application entry point
```

### Adding New Entities

1. Create entity in `Domain/Entities/`
2. Add to database service registration in `Infrastructure/DependencyInjection.cs`
3. Create controller in `Controllers/`
4. Add Neo4j label mapping in `Infrastructure/Abstractions/DatabaseService.cs`

### Database Migrations (PostgreSQL)

```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check database is running
   - Verify connection string in `.env`
   - Ensure database exists

2. **Neo4j Encryption Error**
   - For hosted Neo4j, use `neo4j+s://` in URI
   - For local Neo4j, use `neo4j://` or `bolt://`

3. **JWT Errors**
   - Ensure `JWT_KEY` is at least 32 characters
   - Check JWT configuration in `.env`

4. **Port Already in Use**
   - Change port in `launchSettings.json`
   - Or kill process using port 5036

### Logs

Check application logs for detailed error information:
```bash
dotnet run --verbosity detailed
```

## 📝 License

[Your License Here]

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Make changes
4. Test with all database providers
5. Submit pull request

## 📞 Support

For issues and questions:
- Create GitHub issue
- Check troubleshooting section
- Review logs for error details
