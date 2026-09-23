# syntax=docker/dockerfile:1

# ==========================================================
# Этап 1: сборка (официальный SDK-образ)
# ==========================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала копируем только csproj — чтобы слой restore кэшировался,
# пока не меняются зависимости проекта
COPY IsLabApp.csproj ./
RUN dotnet restore "IsLabApp.csproj"

# Теперь копируем остальные исходники и собираем
COPY . .
RUN dotnet publish "IsLabApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================================
# Этап 2: рантайм (официальный ASP.NET runtime-образ)
# ==========================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем только опубликованные артефакты, ничего лишнего
COPY --from=build /app/publish .

# Приложение слушает 8080 внутри контейнера
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IsLabApp.dll"]
