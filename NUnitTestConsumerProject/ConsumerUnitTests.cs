using NUnit.Framework;

namespace ConsumerUnitTests
{
	[TestFixture]
	public class ConsumerTest
	{
		private readonly ConsumerSpace.Consumer consumer;
		[SetUp]
		public void Setup()
		{
		}

		public ConsumerTest()
		{
			consumer = new ConsumerSpace.Consumer();
		}

		[Test]
		public void ReturnFalseGivenPrefixOfRubbish()
		{
			var result = consumer.IsValidPrefix("Rubbish");
			Assert.IsFalse(result, "Rubbish is not a valid prefix");
		}

		public void ReturnTrueGivenValidPrefix()
		{

			var result = consumer.IsValidPrefix(consumer.minimumPrefixText);
			Assert.IsTrue(result);
		}

		[Test]
		public void ReturnTrueGivenContentTypeOfTextPlain()
		{
			var result = consumer.IsValidContentType("text/plain");
			Assert.IsTrue(result);
		}

		[TestCase("rubbish")]
		[TestCase("more rubbish")]
		public void ReturnFalseGivenInvalidContentType(string value)
		{
			var result = consumer.IsValidContentType(value);
			Assert.IsFalse(result, value + " is not a valid content type");
		}
	}
}