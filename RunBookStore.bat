@echo off
echo Starting BookStore application...
dotnet run --project .\backend\DatabaseTask3\BookStore.API.csproj --urls=http://localhost:5000
pause
