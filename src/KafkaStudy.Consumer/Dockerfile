#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/KafkaStudy.Consumer/KafkaStudy.Consumer.csproj", "src/KafkaStudy.Consumer/"]
RUN dotnet restore "src/KafkaStudy.Consumer/KafkaStudy.Consumer.csproj"
COPY . .
WORKDIR "/src/src/KafkaStudy.Consumer"
RUN dotnet build "KafkaStudy.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KafkaStudy.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KafkaStudy.Consumer.dll"]