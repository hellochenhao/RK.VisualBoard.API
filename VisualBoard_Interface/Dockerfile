#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN cp /usr/share/zoneinfo/Asia/Shanghai /usr/share/zoneinfo/Asia/Beijing
RUN sed -i 's/TLSv1.2/TLSv1.0/g' /etc/ssl/openssl.cnf
ENV ASPNETCORE_URLS http://*:5003

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["NuGet.Config", "/"]
COPY ["VisualBoard_Interface/VisualBoard_Interface.csproj", "VisualBoard_Interface/"]
COPY ["Models/VisualBoard/VisualBoard.Models.csproj", "Models/VisualBoard/"]
COPY ["BusinessService/VisualBoard.Business.Service/VisualBoard.Business.Service.csproj", "BusinessService/VisualBoard.Business.Service/"]
COPY ["BusinessInterface/VisualBoard.Business.Interface/VisualBoard.Business.Interface.csproj", "BusinessInterface/VisualBoard.Business.Interface/"]
RUN dotnet restore "VisualBoard_Interface/VisualBoard_Interface.csproj"
COPY . .
WORKDIR "/src/VisualBoard_Interface"
RUN dotnet build "VisualBoard_Interface.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VisualBoard_Interface.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VisualBoard_Interface.dll"]