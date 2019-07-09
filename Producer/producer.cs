using System;
using System.Text;

namespace ProducerSpace
{
	public class Producer
	{
		readonly string terminateHelpText = "Press any key to end";

		public static void Main(string[] args)
		{
			var producer = new Producer();
			producer.Execute();
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
						var channelProperties = channel.CreateBasicProperties();
						channelProperties.ContentType = "text/plain";
						channel.QueueDeclare(queue: "greetings", durable: false, exclusive: false, autoDelete: false, arguments: null);

						DisplayInstructions();

						string message = GetInput();
						while (message.Length > 0)
						{
							var body = Encoding.UTF8.GetBytes(message);
							channel.BasicPublish(exchange: "", routingKey: "greetings", mandatory: false, basicProperties: channelProperties, body: body);
							message = GetInput();
						}
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
					Console.WriteLine("An unknown error has occurred. " + exception.Message + ". " + terminateHelpText);
				}
				Console.ReadKey();
			}
		}

		public void DisplayInstructions()
		{
			Console.WriteLine("Please enter your name and press the <Enter> key. Press <Enter> key without a name to end");
		}

		private string GetInput()
		{
			string inputValue = string.Empty;
			inputValue = Console.ReadLine();
			if (inputValue.Length > 0)
			{ 
				inputValue = "Hello my name is, " + inputValue;
			}
			return inputValue;
		}
	}
}
