FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM base AS final
WORKDIR /app
COPY --from=stp.build /app/STP.Identity .
ENTRYPOINT ["dotnet", "STP.Identity.API.dll"]