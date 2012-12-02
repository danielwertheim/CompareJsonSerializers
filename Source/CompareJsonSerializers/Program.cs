using System;
using System.Diagnostics;
using System.Linq;
using CompareJsonSerializers.Model;
using CompareJsonSerializers.SimpleJson;
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

        private static IJsonSerializerStrategy _simpleJsonStrategy = new SimpleJsonSerializerStrategy(); //To get support for ENUM
 
        static void Main(string[] args)
        {
            var numberOfCustomers = 1;
            var numberOfItterations = 1000;

            Console.WriteLine("Num of customers: {0}", numberOfCustomers);
            Console.WriteLine("Num of itterations: {0}", numberOfItterations);
            Console.WriteLine();

			_printer = ts =>
			{
			    Console.WriteLine("s = {0}", ts.TotalSeconds);
                Console.WriteLine("ms = {0}", ts.TotalMilliseconds);
                Console.WriteLine();
			};
            /*********************** SimpleJson ***********************/
            Console.WriteLine("SimpleJson - Serializing");
            TimeSerializationAction(SerializeUsingSimpleJson, numberOfCustomers, numberOfItterations);
            Console.WriteLine("SimpleJson - Deserializing");
            TimeDeserializationAction(SimpleJson.SimpleJson.SerializeObject, DeSerializeUsingSimpleJson, numberOfCustomers, numberOfItterations);
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

        private static int SerializeUsingSimpleJson(Customer[] customers)
        {
            return customers.Select(s => SimpleJson.SimpleJson.SerializeObject(s, _simpleJsonStrategy)).ToArray().Length;
        }

        private static int DeSerializeUsingSimpleJson(string[] json)
        {
            return json.Select(s => SimpleJson.SimpleJson.DeserializeObject<Customer>(s, _simpleJsonStrategy)).ToArray().Length;
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
            GC.Collect();
            action(customers); //Burn one
			
            var stopWatch = new Stopwatch();

		    var sum = new TimeSpan(0);
            for (var c = 0; c < numOfItterations; c++)
            {
                stopWatch.Start();
                action(customers);
                stopWatch.Stop();
                sum += stopWatch.Elapsed;
                stopWatch.Reset();
            }

            _printer(sum);
        }

        private static void TimeDeserializationAction(Func<Customer, string> serializer, Func<string[], int> action, int numOfCustomers, int numOfItterations)
        {
			var customerJsons = CustomerFactory.CreateCustomers(numOfCustomers).Select(serializer).ToArray();
            GC.Collect();
            action(customerJsons); //Burn one
            
			var stopWatch = new Stopwatch();

            var sum = new TimeSpan(0);
            for (var c = 0; c < numOfItterations; c++)
            {
                stopWatch.Start();
                action(customerJsons);
                stopWatch.Stop();
                sum += stopWatch.Elapsed;
                stopWatch.Reset();
            }

            _printer(sum);
        }
    }
}
