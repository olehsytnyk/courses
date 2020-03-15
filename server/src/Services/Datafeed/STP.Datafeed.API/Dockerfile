FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM base AS final
WORKDIR /app
COPY --from=stp.build /app/STP.Datafeed .
ENTRYPOINT ["dotnet", "STP.Datafeed.API.dll"]