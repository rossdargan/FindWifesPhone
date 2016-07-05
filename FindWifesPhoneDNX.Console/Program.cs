using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindWifesPhoneDNX.Console
{
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    using FindWifesPhoneDNX.Library;

    using M2Mqtt;
    using M2Mqtt.Messages;

    using Console = System.Console;

    public class Program
    {
        private static FindWifesPhoneDNX.Library.IFindPhoneService _findPhoneService;

        public static void Main(string[] args)
        {

            string brokerIn = Environment.GetEnvironmentVariable("BROKER");

            if (string.IsNullOrWhiteSpace(brokerIn))
            {
                throw new Exception("The broker host name, or ip must be provided. Please set the environmental variable BROKER");
            }

            string clientID = Environment.GetEnvironmentVariable("CLIENT_ID");
            if (string.IsNullOrWhiteSpace(clientID))
            {
                throw new Exception("The client id must be provided. Please set a client ID using the environmental varliable CLIENT_ID ");
            }

            string usernameIn = Environment.GetEnvironmentVariable("MQTT_USERNAME");
            string passwordIn = Environment.GetEnvironmentVariable("MQTT_PASSWORD");
            string topicIn = Environment.GetEnvironmentVariable("MQTT_TOPIC");            


            string appleUsername = Environment.GetEnvironmentVariable("APPLE_USERNAME");
            string applePassword = Environment.GetEnvironmentVariable("APPLE_PASSWORD");
            if (string.IsNullOrWhiteSpace(appleUsername))
            {
                throw new Exception("You must provide your apple username. Please set the username using the environmental variable APPLE_USERNAME");
            }

            if (string.IsNullOrWhiteSpace(applePassword))
            {
                throw new Exception("You must provide your apple password. Please set the username using the environmental variable APPLE_PASSWORD");
            }

            _findPhoneService = new FindPhoneService(clientID, appleUsername, applePassword);
            
            M2Mqtt.MqttClient client = new M2Mqtt.MqttClient(brokerIn);

            Console.WriteLine($"Connecting to MQTT at host {brokerIn}");
            if (!string.IsNullOrWhiteSpace(usernameIn) && !string.IsNullOrWhiteSpace(passwordIn))
            {
                Console.WriteLine("Using the username and password provided");
                client.Connect(clientID, usernameIn, passwordIn);
            }
            else
            {
                Console.WriteLine("No username or password will be used");
                client.Connect(clientID);
            }
            Console.WriteLine("Connected to MQTT");

            string topic = topicIn;
            if (string.IsNullOrWhiteSpace(topicIn))
            {
                topic = "/findphone";
            }

            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            Console.WriteLine($"Subscribing to topic: {topic}");
            client.Subscribe(new[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            


            Console.ReadLine();

            
        }

        private static async void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                Console.WriteLine("Message Received");
                string deviceName = System.Text.Encoding.UTF8.GetString(e.Message);

                if (string.IsNullOrWhiteSpace(deviceName))
                {
                    Console.WriteLine("Message didn't have a device specified... ignoring");
                    return;
                }
                Console.WriteLine($"Attempting to find {deviceName}");
                bool result = await _findPhoneService.FindPhone(deviceName);
                if (result)
                {
                    Console.WriteLine("Request for sound done!");
                }
                else
                {
                    Console.WriteLine("Something went wrong with the search");
                }

            }
            catch (Exception err)
            {
                Console.WriteLine("The following error occured whilst trying to find the device:");
                Console.WriteLine(err.ToString());                
            }
            


        }
    }
}
