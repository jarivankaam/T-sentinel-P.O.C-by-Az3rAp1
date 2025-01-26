using SimpleMqtt;

namespace MqqtController
{


    public class MqqtCOntroller
    {

        private string _mqqtToken;
        private SimpleMqttClient? _client;
        private string _topic;
        public string? result;

        public MqqtCOntroller(string token, string topic)
        {
            _mqqtToken = token;
            _topic = topic;

        }

        public void connect()
        {
            _client = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(_mqqtToken);
        }



        public async Task ReceiveMessageAsync(string? topic)
        {
            if (_client != null)
            {
                string selectedTopic = topic != null ? topic : _topic;
                // Subscribe to the OnMessageReceived event
                try
                {
                    // Await the subscription
                    await _client.SubscribeToTopic(selectedTopic);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing to topic: {ex.Message}");
                }

                _client.OnMessageReceived += (sender, args) => { result = args.Message; };
            }
            else
            {
                Console.WriteLine("Client is not initialized. Please connect first.");
            }
        }

        public async Task SendMessage(string message, string? topic)
        {
            string selectedTopic = topic != null ? topic : _topic;
            if (_client == null)
            {
                 Console.WriteLine("Client is not initialized. Please connect first.");
                return;
            }

            try
            {

                await _client.PublishMessage(message, selectedTopic);
                Console.WriteLine($"Message sent to topic '{selectedTopic}': {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send message to topic '{selectedTopic}': {ex.Message}");
            }
        }

    }
}