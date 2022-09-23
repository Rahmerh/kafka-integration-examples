FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base

RUN apt-get update -y
RUN apt-get install -y librdkafka-dev

WORKDIR /bin
COPY ./bin/Debug/net6.0/* ./

ENTRYPOINT ["dotnet", "/bin/nu.example.DuplicateMessageFilter.dll"]
