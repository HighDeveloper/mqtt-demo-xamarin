using MqttTest.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MqttTest.ViewModel
{
    public class MqttClientViewModel : INotifyPropertyChanged
    {
        string messageSub;
        string messagePub;
        string topic;
        string clientId;
        string ip;
        int port;
        string username;
        string password;
        bool isConnected;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Item> Messages { get; set; }
        public ICommand PublishMessage { get; set; }
        public ICommand Unsubscribe { get; set; }
        public ICommand Subscribe { get; set; }
        public IMqttClient Client { get; set; }

        public string MessageSub
        {
            set
            {
                if (messageSub != value)
                {
                    messageSub = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MessageSub"));
                    }
                }
            }
            get{ return messageSub; }
        }
        public string MessagePub
        {
            set
            {
                if (messagePub != value)
                {
                    messagePub = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MessagePub"));
                    }
                }
            }
            get { return messagePub; }
        }
        public string Topic
        {
            set
            {
                if (topic != value)
                {
                    topic = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Topic"));
                    }
                }
            }
            get { return topic; }
        }
        public string Username
        {
            set
            {
                if (username != value)
                {
                    username = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                    }
                }
            }
            get { return username; }
        }
        public string Password
        {
            set
            {
                if (password != value)
                {
                    password = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                    }
                }
            }
            get { return password; }
        }
        public bool IsConnected
        {
            set
            {
                isConnected = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsConnected"));
                }
            }
            
            get { return isConnected; }
        }
        public string Ip
        {
            set
            {
                if (ip != value)
                {
                    ip = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Ip"));
                    }
                }
            }
            get { return ip; }
        }
        public int Port
        {
            set
            {
                if (port != value)
                {
                    port = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Port"));
                    }
                }
            }
            get { return port; }
        }
        public string ClientId
        {
            set
            {
                if (clientId != value)
                {
                    clientId = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ClientId"));
                    }
                }
            }
            get { return clientId; }
        }


        public MqttClientViewModel()
        {
            {
                this.MessageSub = string.Empty;
                Messages = new ObservableCollection<Item>();
                PublishMessage = new Command(Publish);
                Unsubscribe = new Command(UnsubscribeTopic);
                Subscribe = new Command(MqttClientConnection);
            }
        }

        /// <summary>
        /// Publish a message to the server specified
        /// </summary>
        public async void Publish()
        {
            try
            {
                //Note that you can set more properties and also not set the port, 
                //in which case the MQTT default will be used
                var configuration = new MqttConfiguration { Port = this.Port };
                //Creation of the MQTT client
                var client = await MqttClient.CreateAsync(Ip, configuration);

                //MQTT connection of the client. You can pass optional args to the 
                //ConnectAsync method and credentials
                await client.ConnectAsync(new MqttClientCredentials(ClientId, Username, Password), null, true);

                //MQTT publish to a topic
                //The message has a topic and the payload in byte[], which you are in charge of serializing from the original format
                //The PublishAsync method has some optional args
                var message = new MqttApplicationMessage(Topic, Encoding.UTF8.GetBytes(MessagePub));

                await client.PublishAsync(message, MqttQualityOfService.AtLeastOnce);

                //Method to unsubscribe a topic or many topics, which means that the message will no longer
                //be received in the MessageStream anymore
                await client.UnsubscribeAsync(Topic);

                //MQTT disconnection. Note that by now each client instance lifetime is from Connection to Disconnection
                //You can't re use an instance or re connect once you disconnected. You will need to create another MqttClient instance
                //This is currently reported as an issue and will be fixed for the next public version
                await client.DisconnectAsync();
            }
            catch ( Exception e )
            {

            }
        }

        public async void MqttClientConnection()
        {
            try
            {
                //Note that you can set more properties and also not set the port, 
                //in which case the MQTT default will be used
                var configuration = new MqttConfiguration { Port = this.Port };
                
                //Creation of the MQTT client
                Client = await MqttClient.CreateAsync(Ip, configuration);
                
                //MQTT connection of the client. You can pass optional args to the 
                //ConnectAsync method and credentials
                await Client.ConnectAsync(new MqttClientCredentials(ClientId, Username, Password), null, true);
                
                //MQTT subscription to a topic. This only performs the protocol subscription,
                //which means that at this point it can start receiving messages from the Broker to that topic
                await Client.SubscribeAsync(Topic, MqttQualityOfService.AtLeastOnce);
                
                //Rx Subscription to receive all the messages for the subscribed topics
                Client.MessageStream.Subscribe(msg =>
                {
                    //All the messages from the Broker to any subscribed topic will get here
                    //The MessageStream is an Rx Observable, so you can filter the messages by topic with Linq to Rx
                    //The message object has Topic and Payload properties. The Payload is a byte[] that you need to deserialize 
                    //depending on the type of the message
                    MessageSub = System.Text.Encoding.UTF8.GetString(msg.Payload);
                    Messages.Add(new Item { Text = MessageSub.ToString() });
                });

                if (Client.IsConnected)
                {
                    IsConnected = true;
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Unsubscribe and disconnect the current client to the topic
        /// </summary>
        public async void UnsubscribeTopic()
        {
            try
            {
                //Unsubscribe to the topic
                await Client.UnsubscribeAsync(Topic);
                //Disconnect the current client
                await Client.DisconnectAsync();

                if (!Client.IsConnected)
                {
                    IsConnected = false;
                }
            }
            catch ( Exception e )
            {

            }
        }
    }
}

