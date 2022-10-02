FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

RUN apt-get update -y
RUN apt-get install -y librdkafka-dev

WORKDIR /bin
COPY ./bin/Debug/net6.0/* ./

EXPOSE 80

ENTRYPOINT ["dotnet", "/bin/nu.example.HttpProducer.dll"]
