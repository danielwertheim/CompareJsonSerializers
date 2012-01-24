using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CompareJsonSerializers.Model;

namespace CompareJsonSerializers
{
	public static class f
	{
		public static T d<T>(fastJSON.JSON p, string j, T t)
		{
			return p.ToObject<T>(j);
		}
	}
    class Program
    {
        static void Main(string[] args)
        {
            var numberOfCustomers = 100000;
            var numberOfItterations = 5;

			//Console.WriteLine("Json.Net - Serializing");
			//TimeSerializationAction(SerializeUsingJsonNet, numberOfCustomers, numberOfItterations);
			//Console.WriteLine("Json.Net - Deserializing");
			//TimeDeserializationAction(Newtonsoft.Json.JsonConvert.SerializeObject, DeSerializeUsingJsonNet, numberOfCustomers, numberOfItterations);

			//Console.WriteLine("ServiceStack.Text - Serializing");
			//TimeSerializationAction(SerializeUsingServiceStackText, numberOfCustomers, numberOfItterations);
			//Console.WriteLine("ServiceStack.Text - Deserializing");
			//TimeDeserializationAction(ServiceStack.Text.JsonSerializer.SerializeToString, DeserializeUsingServiceStackText, numberOfCustomers, numberOfItterations);

			//Console.WriteLine("fastJSon - Serializing");
			//TimeSerializationAction(SerializeUsingFastJson, numberOfCustomers, numberOfItterations);
			//Console.WriteLine("fastJson - Deserializing");
			//TimeDeserializationAction(fastJSON.JSON.Instance.ToJSON, DeSerializeUsingFastJson, numberOfCustomers, numberOfItterations);

            Console.ReadKey();
        }

        private static void SerializeUsingJsonNet(IEnumerable<Customer> customers)
        {
            var json = customers.Select(Newtonsoft.Json.JsonConvert.SerializeObject).ToList();
        }

        private static void DeSerializeUsingJsonNet(IEnumerable<string> json)
        {
            var customers = json.Select(Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>).ToList();
        }

        private static void SerializeUsingServiceStackText(IEnumerable<Customer> customers)
        {
            var json = customers.Select(ServiceStack.Text.JsonSerializer.SerializeToString).ToList();
        }

        private static void DeserializeUsingServiceStackText(IEnumerable<string> json)
        {
            var customers = json.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<Customer>).ToList();
        }

		private static void SerializeUsingFastJson(IEnumerable<Customer> customers)
		{
			var json = customers.Select(fastJSON.JSON.Instance.ToJSON).ToList();
		}

		private static void DeSerializeUsingFastJson(IEnumerable<string> json)
		{
			var customers = json.Select(fastJSON.JSON.Instance.ToObject<Customer>).ToList();
		}

        private static void TimeSerializationAction(Action<IList<Customer>> action, int numOfCustomers, int numOfItterations)
        {
            var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                var customers = CustomerFactory.CreateCustomers(numOfCustomers);

                stopWatch.Start();
                action(customers);
                stopWatch.Stop();

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }
        }

        private static void TimeDeserializationAction(Func<Customer, string> serializer, Action<IList<string>> action, int numOfCustomers, int numOfItterations)
        {
            var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                var customerJsons = CustomerFactory.CreateCustomers(numOfCustomers).Select(serializer).ToList();

                stopWatch.Start();
                action(customerJsons);
                stopWatch.Stop();

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }
        }
    }
}
