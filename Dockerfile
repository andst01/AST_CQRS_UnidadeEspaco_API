# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia todos os .csproj para cache (usando globbing para simplificar)
# Isso copia todos os csproj mantendo a estrutura de pastas
COPY *.sln ./
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

# 2. Restaura os pacotes de toda a solução
RUN dotnet restore

# 3. Copia o restante do código fonte
COPY . .

# ---------------------------------------------------------
# ESTÁGIO DE TESTE
# ---------------------------------------------------------
FROM build AS testrunner
WORKDIR /src
# Executa todos os testes da solução. 
# Se qualquer projeto de teste falhar, o build do Docker trava aqui.
RUN dotnet test --configuration Debug --logger:trx --collect:"XPlat Code Coverage"

# ---------------------------------------------------------
# ESTÁGIO DE PUBLICAÇÃO
# ---------------------------------------------------------
FROM build AS publish
WORKDIR /src/src/UnidadeEspacoSrv.Api
RUN dotnet publish "UnidadeEspacoSrv.Api.csproj" -c Debug -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:5091

COPY --from=publish /app/publish .

EXPOSE 5091
ENTRYPOINT ["dotnet", "UnidadeEspacoSrv.Api.dll"]