FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["code_exchanger_back/code_exchanger_back.csproj", "code_exchanger_back/"]
RUN dotnet restore "code_exchanger_back/code_exchanger_back.csproj"
COPY . .
WORKDIR "/src/code_exchanger_back"
RUN dotnet build "code_exchanger_back.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "code_exchanger_back.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "code_exchanger_back.dll"]