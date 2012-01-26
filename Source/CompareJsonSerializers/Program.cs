using System;
using System.Diagnostics;
using System.Linq;
using CompareJsonSerializers.Model;

namespace CompareJsonSerializers
{
    class Program
    {
    	private static Action<TimeSpan> _printer;
 
        static void Main(string[] args)
        {
            var numberOfCustomers = 10000;
            var numberOfItterations = 5;

			//_printer = ts => Console.WriteLine("TotalMilliseconds = {0}", ts.TotalMilliseconds);
			_printer = ts => Console.WriteLine("TotalSeconds = {0}", ts.TotalSeconds);
			/*********************** Json.Net ***********************/
			Console.WriteLine("Json.Net - Serializing");
			TimeSerializationAction(SerializeUsingJsonNet, numberOfCustomers, numberOfItterations);
			Console.WriteLine("Json.Net - Deserializing");
			TimeDeserializationAction(Newtonsoft.Json.JsonConvert.SerializeObject, DeSerializeUsingJsonNet, numberOfCustomers, numberOfItterations);
			/*********************** ServiceStack ***********************/
			Console.WriteLine("ServiceStack.Text - Serializing");
			TimeSerializationAction(SerializeUsingServiceStackText, numberOfCustomers, numberOfItterations);
			Console.WriteLine("ServiceStack.Text - Deserializing");
			TimeDeserializationAction(ServiceStack.Text.JsonSerializer.SerializeToString, DeserializeUsingServiceStackText, numberOfCustomers, numberOfItterations);
			/*********************** fastJSON ***********************/
			Console.WriteLine("fastJSon - Serializing");
			TimeSerializationAction(SerializeUsingFastJson, numberOfCustomers, numberOfItterations);
			Console.WriteLine("fastJson - Deserializing");
			TimeDeserializationAction(fastJSON.JSON.Instance.ToJSON, DeSerializeUsingFastJson, numberOfCustomers, numberOfItterations);

            Console.ReadKey();
        }

		private static int SerializeUsingJsonNet(Customer[] customers)
        {
			return customers.Select(Newtonsoft.Json.JsonConvert.SerializeObject).ToArray().Length;
        }

		private static int DeSerializeUsingJsonNet(string[] json)
        {
			return json.Select(Newtonsoft.Json.JsonConvert.DeserializeObject<Customer>).ToArray().Length;
        }

		private static int SerializeUsingServiceStackText(Customer[] customers)
        {
			return customers.Select(ServiceStack.Text.JsonSerializer.SerializeToString).ToArray().Length;
        }

		private static int DeserializeUsingServiceStackText(string[] json)
        {
			return json.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<Customer>).ToArray().Length;
        }

		private static int SerializeUsingFastJson(Customer[] customers)
		{
			return customers.Select(fastJSON.JSON.Instance.ToJSON).ToArray().Length;
		}

		private static int DeSerializeUsingFastJson(string[] json)
		{
			return json.Select(fastJSON.JSON.Instance.ToObject<Customer>).ToArray().Length;
		}

		private static void TimeSerializationAction(Func<Customer[], int> action, int numOfCustomers, int numOfItterations)
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

            	_printer(stopWatch.Elapsed);

                stopWatch.Reset();
            }
        }

        private static void TimeDeserializationAction(Func<Customer, string> serializer, Func<string[], int> action, int numOfCustomers, int numOfItterations)
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

				_printer(stopWatch.Elapsed);

                stopWatch.Reset();
            }
        }
    }
}
