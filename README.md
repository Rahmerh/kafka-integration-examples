<p><img height="60" width="60" src="https://mpng.subpng.com/20190517/hou/kisspng-apache-kafka-apache-software-foundation-computer-s-connectivity-svg-png-icon-free-download-465-6-5cdf21d9a9fa76.5356632115581270656962.jpg"> <h1>Kafka with dotnet example project</h1></p>

A simple example project to get a kafka consumer/producer environment set up. Complete with docker-compose, infra containers and the works.

## Usage

1. Build (`dotnet build`) the solution.
2. Execute `docker-compose up -d`

The producer will automatically produce 100 messages with 200ms sleeps between each message.

## View topic contents

I've included a AKHQ container to be able to view topics. Open `localhost:1337` in your browser to view.

Open live tail and view the `kafka-csharp-example` topic and restart the producer container. You should see a slew of messages appear on the topic.
