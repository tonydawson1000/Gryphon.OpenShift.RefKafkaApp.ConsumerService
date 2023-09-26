# Introduction

Sample HTTP API that Consumes simple Messages from Kafka.

Part of the 'RefKafkaApp' Reference Architecture Project.

Used to demonstrate containerisation of a Kafka Consumer configured and running on OpenShift. 

# Getting Started
1.	Ensure [Podman](https://github.com/containers/podman) is installed
2.	Ensure you have connection details to a Kafka (BootstrapServers, Topic)
3.	Clone the repo

## Setting up a 'Local' Kafka
1.	Ensure [Podman-Compose](https://github.com/containers/podman-compose) is installed
2.	Clone the repo
3.	Start Kafka
4.	From the confluentKafkaAllInOne folder - run `podman-compose up -d`
5.	View the 'Control Centre' Dashboard from -> http://localhost:9021/clusters 

# Build and Test (Podman)
To Build a Container Image using the Containerfile:
- `podman build -t kafka-consumer:0.1 .`

To Run a Container instance:
- `podman run -e "Kafka.ConsumerConfig:TopicName=<ENTER-TOPIC-NAME-HERE>" -e "Kafka.ConsumerConfig:BootstrapServers=<IPS-FOR-BOOTSTRAP-SERVERS>:9092" -e "Kafka.ConsumerConfig:GroupId=<CONSUMER_GROUP_ID>" -p 8082:8080 kafka-consumer:0.1 .`

e.g. 
- `podman run -e "Kafka.ConsumerConfig:TopicName=MyTestTopic" -e "Kafka.ConsumerConfig:BootstrapServers=10.25.30.157:9092" -e "Kafka.ConsumerConfig:GroupId=MyTestConsumerGroupId" -p 8082:8080 kafka-consumer:0.1 .`

To View Container API:
- http://localhost:8082/swagger/

- The **GET** [Config](http://localhost:8082/config) Endpoint returns details of the loaded Kafka Consumer Config

- The **GET** `Messages` Endpoint 'Consumes' *ALL* Messages from the Kafka Topic

# TODO : Setup Kubernetes Manifests for Deployment...