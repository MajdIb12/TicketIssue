# Stage 1: Base Runtime Environment (.NET 8/9)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: SDK Image to Build the Code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# نسخ ملفات الـ Projects أولاً لعمل Cache للـ Nuget Packages (تسريع البناء مستقبلاً)
COPY ["TicketIssue.Api/TicketIssue.Api.csproj", "TicketIssue.Api/"]
COPY ["TicketIssue.Domain/TicketIssue.Domain.csproj", "TicketIssue.Domain/"]
COPY ["TicketIssue.Infrastructure/TicketIssue.Infrastructure.csproj", "TicketIssue.Infrastructure/"]
RUN dotnet restore "./TicketIssue.Api/TicketIssue.Api.csproj"

# نسخ باقي ملفات الكود بالكامل وبناء المشروع
COPY . .
WORKDIR "/src/TicketIssue.Api"
RUN dotnet build "./TicketIssue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the Application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TicketIssue.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final Lean Runtime Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketIssue.Api.dll"]