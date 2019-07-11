using System;
using System.Text;

namespace ConsumerSpace
{
    public class Consumer
    {
				readonly string terminateHelpText = "Press any key to end";
				public readonly string minimumPrefixText = "Hello my name is,";
				
			public static void Main(string[] args)
			{
				var consumer = new Consumer();
				consumer.Execute();
			}

			private void Execute()
			{ 

				var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };

				try
				{
					using (var connection = factory.CreateConnection())
					{
						using (var channel = connection.CreateModel())
						{
							channel.QueueDeclare(queue: "greetings", durable: false, exclusive: false, autoDelete: false, arguments: null);

							var consumerEvent = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);
							consumerEvent.Received += (model, deliveryEventArguments) =>
							{
								var properties = deliveryEventArguments.BasicProperties;

								if (deliveryEventArguments.RoutingKey == "greetings")
								{
									if (IsValidContentType(properties.ContentType))
									{
										var body = deliveryEventArguments.Body;
										string message = Encoding.UTF8.GetString(body);

										if (IsValidPrefix(message))
										{
											DisplayMessage(message);
										}
									}
								}
							};

							channel.BasicConsume(queue: "greetings", autoAck: true, consumerTag: "", noLocal: false, exclusive: true, arguments: null, consumer: consumerEvent);
							Console.WriteLine(terminateHelpText);
							Console.ReadKey();
						}
					}
				}
				catch (Exception exception)
				{
					if (exception is RabbitMQ.Client.Exceptions.BrokerUnreachableException)
					{ 
						Console.WriteLine("The messaging service is currently unavailable. " + terminateHelpText);
					}
					else
					{
						Console.WriteLine("An error has occurred. " + exception.Message + ". " + terminateHelpText);
					}
					Console.ReadKey();
				}        
			}

			private void DisplayMessage(string message)
			{
				Console.WriteLine("Hello {0}, I am your father!", GetReceivedName(message));
			}

			private string GetReceivedName(string message)
			{
				int startIndex = message.IndexOf(',') + 1;
				return message.Substring(startIndex, message.Length - minimumPrefixText.Length).Trim();
			}

			public bool IsValidPrefix(string message)
			{
				bool isValid = true;
				if (!message.StartsWith(minimumPrefixText))
				{
					isValid = false;
					Console.WriteLine("The received message did not contain the expected prefix");
				}
				return isValid;
			}

			public bool IsValidContentType(string contentType)
			{
				bool isValid = true;
				if (contentType != "text/plain")
				{
					isValid = false;
					Console.WriteLine("The content type is invalid. Only 'text/plain' is supported");
				}
				return isValid;
			}
    }
}
