<img height="60" width="60" src="https://www.explore-group.com/storage/images-processed/w-1500_h-auto_m-fit_s-any__600_470085481.jpeg">

# Kafka with dotnet example project

A simple example project to get a kafka consumer/producer environment set up. Complete with docker-compose, infra containers and the works.

## Contents

I have the following applications/containers in this project

- **nu.example.DuplicateMessageFilter**
  - An example for a duplicate message filter. This application caches the user hashes in a redis cache and compares it vs the incoming user.
- **nu.example.FirestoreProducer**
  - A simple producer which listens to the `Users` collection in firestore and produces a message for each document change.
- **nu.example.UserConsumer**
  - Consumes user messages from the topic and writes the results to the console.
- **nu.example.UserProducer**
  - More a PoC than anything else, when it starts it'll produce a single message to the `users` topic.
- **nu.example.Shared**
  - Library project containing models, settings and dependencies.

## Requirements

- [Dotnet 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/)
- [Docker-compose](https://docs.docker.com/compose/install/)

## Usage

1. Build (`dotnet build`) the solution.
2. Execute `docker-compose up -d --build`

## GUIs

I've included a couple of guis to visualize the data you're working with:

- [AKHQ](https://github.com/tchiotludo/akhq)
  - [`localhost:2000`](localhost:2000)
- [Firebase emulator](https://firebase.google.com/docs/emulator-suite)
  - [`localhost:2001`](localhost:2001)
- [Redisinsight](https://redis.io/docs/stack/insight/)
  - [`localhost:2002`](localhost:2002)

### Cache setup

A few small steps are needed to setup redisinsights:

1. Navigate to `localhost:2002`
2. Click on `I already have a database` then click on `Connect to a Redis Database`
3. Fill in the following values:

- Host: `cache`
- Port: `6379`
- Name: `Cache`
- Password: `SUPER_SECRET_PASS`

## View topic contents

I've included a AKHQ container to be able to view topics. Open `localhost:1337` in your browser to view.

Open live tail and view the `users` topic. All messages are produced/consumed from there.
