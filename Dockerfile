FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Elara/Elara.sln Elara/
COPY Elara/Elara.API/Elara.API.csproj Elara/Elara.API/
COPY Elara/Elara.Application/Elara.Application.csproj Elara/Elara.Application/
COPY Elara/Elara.Domain/Elara.Domain.csproj Elara/Elara.Domain/
COPY Elara/Elara.Infrastructure/Elara.Infrastructure.csproj Elara/Elara.Infrastructure/
COPY Elara/Elara.Persistence/Elara.Persistence.csproj Elara/Elara.Persistence/

RUN dotnet restore Elara/Elara.sln

COPY Elara/ Elara/

RUN dotnet publish Elara/Elara.API/Elara.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Elara.API.dll"]
