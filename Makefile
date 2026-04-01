.PHONY: build run test migrate migration fmt restore

# Restore dependencies
restore:
	dotnet restore

# Build the solution
build:
	dotnet build

# Run the API
run:
	dotnet run --project src/Api

# Run all tests
test:
	dotnet test

# Create a new migration (usage: make migration name=MigrationName)
migration:
	dotnet ef migrations add $(name) -p src/Infrastructure -s src/Api

# Apply pending migrations
migrate:
	dotnet ef database update -p src/Infrastructure -s src/Api

# Format code
fmt:
	dotnet format
