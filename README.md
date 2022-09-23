<img height="60" width="60" src="https://www.explore-group.com/storage/images-processed/w-1500_h-auto_m-fit_s-any__600_470085481.jpeg">

# Kafka with dotnet example project

A simple example project to get a kafka consumer/producer environment set up. Complete with docker-compose, infra containers and the works.

## Contents

I have the following applications/containers in this project

- **nu.example.FirestoreProducer**
  - A simple producer which listens to the `Users` collection in firestore and produces a message for each document change.
- **nu.example.UserConsumer**
  - Consumes user messages from the topic and writes the results to the console.
- **nu.example.UserProducer**
  - More a PoC than anything else, when it starts it'll produce 100 messages to the topic withh 200ms sleeps in between each message.

## Requirements

- [Dotnet 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/)
- [Docker-compose](https://docs.docker.com/compose/install/)

## Usage

1. Build (`dotnet build`) the solution.
2. Execute `docker-compose up -d --build`

## View topic contents

I've included a AKHQ container to be able to view topics. Open `localhost:1337` in your browser to view.

Open live tail and view the `kafka-csharp-example` topic. All messages are produced/consumed from there.
