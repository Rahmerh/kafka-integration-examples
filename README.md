<img height="60" width="60" src="https://www.explore-group.com/storage/images-processed/w-1500_h-auto_m-fit_s-any__600_470085481.jpeg">

# Kafka integration examples

This repository functions as a bunch of example integrations you can have with kafka. Written in C# and provided with docker-compose file to easily set up all projects locally, the complete project contains infra- and project containers, GUIs to view data, setup instructions, the works.

These projects are meant as examples to custom built integrations instead of the [out of the box](https://docs.confluent.io/kafka-connectors/self-managed/kafka_connectors.html) ones. Those also work fine, but if you need some business logic you need a custom built one. This repository serves as a bunch of examples to help you set up quickly.

# Getting started

Please visit the [wiki](https://github.com/Rahmerh/kafka-integration-examples/wiki) for more information & instructions about how to get started.

# Stability

I'm currently the only one working on this repository and am constantly pushing to the main branch. This means it's possible I accidentally push breaking changes. Please make sure to pull frequently if you're using my repo as an example. I cannot guarantee stability in this project.

# Todo

- [x] Proper (de)serialization of messages to and from topics.
- [x] Fix configuration warnings that occur during startup.
  - [ ] Create short wait loop for topic(s) to be created.
- [ ] Elastic integration project.
- [x] HTTP integration projects. (Integration which makes and receives simple http requests.)
- [ ] Kubernetes setup, load testing ([K6](https://k6.io/)) & helm charts.
- [ ] Expand new wiki pages.

More to come..
