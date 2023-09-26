using AutoMapper;
using Consumer.Api.KafkaConsumerConfig;
using Consumer.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();

builder.Services.Configure<ConsumerConfigOptions>(builder.Configuration.GetSection("Kafka.ConsumerConfig"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet(
    "/config",
    Results<NotFound, Ok<ConsumerConfigOptions>> (
        IOptions<ConsumerConfigOptions> consumerConfigOptions) =>
    {
        return TypedResults.Ok(consumerConfigOptions.Value);
    })
    .WithOpenApi()
    .WithSummary("What Kafka Config is setup.")
    .WithDescription("Review the Kafka Configuration setup for this Consumer Client.");



app.MapGet(
    "/messages",
    Results<NotFound, Ok<IEnumerable<MessageToShowModel>>> (
        IOptions<ConsumerConfigOptions> consumerConfigOptions,
        IMapper mapper,
        ILogger<MessageToShowModel> logger) =>
    {
        var consumerConfig = mapper.Map<Confluent.Kafka.ConsumerConfig>(consumerConfigOptions.Value);

        var topicName = consumerConfigOptions.Value.TopicName;

        logger.LogInformation($"About to Consume Messages from Kafka - BootstrapServers = '{consumerConfig.BootstrapServers}'" +
            $" - Topic = '{topicName}'");

        // Default is to read from Earliest
        consumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;

        using (var consumer = new Confluent.Kafka.ConsumerBuilder<String, String>(consumerConfig).Build())
        {
            logger.LogInformation($"Subscribing to Topic : '{topicName}'...");

            consumer.Subscribe(topicName);

            logger.LogInformation($"Subscribed Ok");

            var consumedMessages = new List<MessageToShowModel>();

            // We are only reading the messages up to now
            var continueConsuming = true;
            var messageCount = 0;

            var stopwatch = new Stopwatch();

            while(continueConsuming)
            {
                stopwatch.Start();

                // Consume - Wait until no more messages...
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                if (consumeResult == null && messageCount == 0)
                {
                    continueConsuming = false;

                    logger.LogInformation("No Messages to Consume");

                    return TypedResults.NotFound();
                }
                else if (consumeResult == null)
                {
                    stopwatch.Stop();

                    continueConsuming = false;
                }
                else
                {
                    var consumedMessage = mapper.Map<MessageToShowModel>(consumeResult);

                    consumedMessages.Add(consumedMessage);

                    messageCount++;

                    logger.LogInformation($"Consumed Message - Key = '{consumedMessage.Key}'" +
                        $" - Value = '{consumedMessage.Value}'" +
                        $" - Partition = '{consumedMessage.Partition}'" +
                        $" - Offset = '{consumedMessage.Offset}'");
                }
            }

            logger.LogInformation($"Host : '{Environment.MachineName}'" +
                $" - Consumed '{consumedMessages.Count}' messages in" +
                $" '{stopwatch.Elapsed.Seconds}' Second(s)");

            stopwatch.Reset();

            consumer.Close();

            return TypedResults.Ok(consumedMessages.AsEnumerable());
        }
    })
    .WithOpenApi()
    .WithSummary("'Consume' Messages from Kafka.")
    .WithDescription("Read all Messages in a Topic on Kafka, Construct a ConsumerClient (using Config), 'Subscribe' to a Topic and 'Consume' the Message from Kafka");

app.Run();
