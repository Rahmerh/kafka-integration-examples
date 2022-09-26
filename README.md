<img height="60" width="60" src="https://www.explore-group.com/storage/images-processed/w-1500_h-auto_m-fit_s-any__600_470085481.jpeg">

# Kafka integration examples

This repository functions as a bunch of example integrations you can have with kafka. Written in C# and provided with docker-compose file to easily set up all projects locally, the complete project contains infra- and project containers, GUIs to view data, setup instructions, the works.

These projects are meant as examples to custom built integrations instead of the [out of the box](https://docs.confluent.io/kafka-connectors/self-managed/kafka_connectors.html) ones. Those also work fine, but if you need some business logic you need a custom built one. This repository serves as a bunch of examples to help you set up quickly.

## Contents

### Projects

The following projects (with description) are currently in this repo. Everything should be started automatically if you follow the steps in the 'Usage' section.

- **nu.example.DuplicateMessageFilter**
  - An example for a duplicate message filter. This application caches the user hashes in a redis cache and compares it vs the incoming user. If the incoming message is the same, it won't produce the user message to the output topic.
- **nu.example.FirestoreProducer**
  - A simple producer which listens to the `Users` collection in firestore and produces a message for each document change.
- **nu.example.FirestoreConsumer**
  - Consumes bank account messages from the `bank-accounts` topic and writes them to the `BankAccounts` firestore collection.
- **nu.example.ConsoleProducer**
  - A simple console producer which will produce a static user or bankaccount message. Can be used to test several consumer/producers. This project is not included in the docker-compose file but can be run locally. Simply execute `dotnet run` in the project folder and choose an option.
- **nu.example.Shared**
  - Library project containing models, settings and shared dependencies.

### Kafka infra

I have the following containers/kafka services included in the docker-compose file:

- 1 Zookeeper instance
- 1 Broker instance
- 1 Connect instance
- 1 Schema registry instance (For automatic (de)serialization)

### Other infra

The following containers are automatically created to be used in the integration projects.

- 1 Redis cache + 1 redisinsight instance
- 1 Firesbase emulator (Only the firestore emulator is set up)
- 1 AKHQ instance

## Requirements

- [Dotnet 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Docker](https://www.docker.com/)
- [docker-compose](https://docs.docker.com/compose/install/)

## Usage

1. Build (`dotnet build`) the solution.
2. Execute `docker-compose up -d --build`

## GUIs

I've included a couple of guis to visualize the data you're working with:

- [AKHQ](https://github.com/tchiotludo/akhq)
  - [`localhost:2000`](http://localhost:2000)
- [Firebase emulator](https://firebase.google.com/docs/emulator-suite)
  - [`localhost:2001`](http://localhost:2001)
- [Redisinsight](https://redis.io/docs/stack/insight/)
  - [`localhost:2002`](http://localhost:2002)

### Kaskade

I personally recommend [kaskade](https://github.com/sauljabin/kaskade) as an awesome TUI to view your kafka topics. Configuration file is included. Install the tool (see link for instructions) and run this command in the root of this repo: `kaskade kaskade.yml`

### Redisinsight setup

A few small steps are needed to setup redisinsights:

1. Navigate to `localhost:2002`
2. Click on `I already have a database` then click on `Connect to a Redis Database`
3. Fill in the following values:

- Host: `cache`
- Port: `6379`
- Name: `Cache`
- Password: `SUPER_SECRET_PASS`

## Stability

I'm currently the only one working on this repository and am constantly pushing to the main branch. This means it's possible I accidentally push breaking changes. Please make sure to pull frequently if you're using my repo as an example. I cannot guarantee any stability in this project.

## Todo

- [x] Proper (de)serialization of messages to and from topics.
- [ ] Fix configuration warnings that occur during startup.
- [ ] Elastic integration project.
- [ ] HTTP integration projects. (Integration which makes and receives simple http requests.)
- [ ] K8s setup, load testing & helm charts.

More to come..
