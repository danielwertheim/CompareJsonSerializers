using System;
using System.Diagnostics;
using System.Linq;
using CompareJsonSerializers.Model;

namespace CompareJsonSerializers
{
    class Program
    {
        static void Main(string[] args)
        {
            var numberOfCustomers = 100;
            var numberOfItterations = 5;

			Console.WriteLine("Json.Net - Serializing");
			TimeSerializationAction(SerializeUsingJsonNet, numberOfCustomers, numberOfItterations);
			Console.WriteLine("Json.Net - Deserializing");
			TimeDeserializationAction(Newtonsoft.Json.JsonConvert.SerializeObject, DeSerializeUsingJsonNet, numberOfCustomers, numberOfItterations);

			Console.WriteLine("ServiceStack.Text - Serializing");
			TimeSerializationAction(SerializeUsingServiceStackText, numberOfCustomers, numberOfItterations);
			Console.WriteLine("ServiceStack.Text - Deserializing");
			TimeDeserializationAction(ServiceStack.Text.JsonSerializer.SerializeToString, DeserializeUsingServiceStackText, numberOfCustomers, numberOfItterations);

			Console.WriteLine("fastJSon - Serializing");
			TimeSerializationAction(SerializeUsingFastJson, numberOfCustomers, numberOfItterations);
			Console.WriteLine("fastJson - Deserializing");
			TimeDeserializationAction(fastJSON.JSON.Instance.ToJSON, DeSerializeUsingFastJson, numberOfCustomers, numberOfItterations);

            Console.ReadKey();
        }

		private static void SerializeUsingJsonNet(Customer[] customers)
        {
			var json = customers.Select(Newtonsoft.Json.JsonConvert.SerializeObject).ToArray();
        }

		private static void DeSerializeUsingJsonNet(string[] json)
        {
			var customers = json.Select(Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>).ToArray();
        }

		private static void SerializeUsingServiceStackText(Customer[] customers)
        {
			var json = customers.Select(ServiceStack.Text.JsonSerializer.SerializeToString).ToArray();
        }

		private static void DeserializeUsingServiceStackText(string[] json)
        {
			var customers = json.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<Customer>).ToArray();
        }

		private static void SerializeUsingFastJson(Customer[] customers)
		{
			var json = customers.Select(fastJSON.JSON.Instance.ToJSON).ToArray();
		}

		private static void DeSerializeUsingFastJson(string[] json)
		{
			var customers = json.Select(fastJSON.JSON.Instance.ToObject<Customer>).ToArray();
		}

		private static void TimeSerializationAction(Action<Customer[]> action, int numOfCustomers, int numOfItterations)
        {
			var customers = CustomerFactory.CreateCustomers(numOfCustomers);
			action(customers);
			GC.Collect();

            var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                stopWatch.Start();
                action(customers);
                stopWatch.Stop();

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }
        }

        private static void TimeDeserializationAction(Func<Customer, string> serializer, Action<string[]> action, int numOfCustomers, int numOfItterations)
        {
			var customerJsons = CustomerFactory.CreateCustomers(numOfCustomers).Select(serializer).ToArray();
        	action(customerJsons);
			GC.Collect();
            
			var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                stopWatch.Start();
                action(customerJsons);
                stopWatch.Stop();

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }
        }
    }
}
