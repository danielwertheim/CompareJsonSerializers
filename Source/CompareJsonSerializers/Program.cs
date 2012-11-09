using System;
using System.Diagnostics;
using System.Linq;
using CompareJsonSerializers.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ServiceStack.Text;

namespace CompareJsonSerializers
{
    class Program
    {
    	private static Action<TimeSpan> _printer;

        private static JsonSerializerSettings _jsonNetSettings = new JsonSerializerSettings
        {
            CheckAdditionalContent = false, 
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new DefaultContractResolver(), 
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc, 
            DefaultValueHandling = DefaultValueHandling.Include,
            Formatting = Formatting.None, 
            MissingMemberHandling = MissingMemberHandling.Ignore, 
            NullValueHandling = NullValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None
        };
 
        static void Main(string[] args)
        {
            var numberOfCustomers = 1000;
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

            Console.ReadKey();
        }

		private static int SerializeUsingJsonNet(Customer[] customers)
        {
			return customers.Select(s => JsonConvert.SerializeObject(s, _jsonNetSettings)).ToArray().Length;
        }

		private static int DeSerializeUsingJsonNet(string[] json)
        {
			return json.Select(s => JsonConvert.DeserializeObject<Customer>(s, _jsonNetSettings)).ToArray().Length;
        }

		private static int SerializeUsingServiceStackText(Customer[] customers)
		{
            ConfServiceStack();

			return customers.Select(ServiceStack.Text.JsonSerializer.SerializeToString).ToArray().Length;
        }

		private static int DeserializeUsingServiceStackText(string[] json)
        {
            ConfServiceStack();

			return json.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<Customer>).ToArray().Length;
        }

        private static void ConfServiceStack()
        {
            TypeConfig<Customer>.EnableAnonymousFieldSetters = true;
            JsConfig.DateHandler = JsonDateHandler.ISO8601;
            JsConfig.ExcludeTypeInfo = true;
            JsConfig.IncludeNullValues = true;
            JsConfig.IncludeTypeInfo = false;
            JsConfig.TryToParsePrimitiveTypeValues = true;
            JsConfig<Customer>.ExcludeTypeInfo = true;
            JsConfig<Customer>.IncludeTypeInfo = false;
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
