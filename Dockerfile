#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Contacts.Presentation/Fiap.Team10.Contacts.Presentation.csproj", "Contacts.Presentation/"]
COPY ["Contacts.Application/Fiap.Team10.Contacts.Application.csproj", "Contacts.Application/"]
COPY ["Contacts.CrossCutting/Fiap.Team10.Contacts.CrossCutting.csproj", "Contacts.CrossCutting/"]
COPY ["Contacts.Domain/Fiap.Team10.Contacts.Domain.csproj", "Contacts.Domain/"]
COPY ["Contacts.Infrastructure/Fiap.Team10.Contacts.Infrastructure.csproj", "Contacts.Infrastructure/"]
RUN dotnet restore "./Contacts.Presentation/Fiap.Team10.Contacts.Presentation.csproj"
COPY . .
WORKDIR "/src/Contacts.Presentation"
RUN dotnet build "./Fiap.Team10.Contacts.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.Team10.Contacts.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Team10.Contacts.Presentation.dll"]