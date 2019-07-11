using NUnit.Framework;

namespace Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TestDisplayInstructions()
		{
			var producer = new ProducerSpace.Producer();
			producer.DisplayInstructions();
			Assert.Pass();
		}
	}
}