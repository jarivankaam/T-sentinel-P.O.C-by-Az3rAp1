using SimpleMqtt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MqqtCOntroller
{
    private string _mqqtToken;
    private SimpleMqttClient? _client;
    private string _topic;

    // Queue for storing received messages
    private Queue<string> _messageQueue = new Queue<string>();

    // Event for notifying when a new message is received
    public event EventHandler<string>? MessageReceived;

    public MqqtCOntroller(string token, string topic)
    {
        _mqqtToken = token;
        _topic = topic;
    }

    public void Connect()
    {
        _client = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ(_mqqtToken);

        if (_client != null)
        {
            _client.OnMessageReceived += OnMessageReceived;
            Console.WriteLine("MQTT client connected.");
        }
        else
        {
            Console.WriteLine("Failed to initialize the MQTT client.");
        }
    }

    public async Task ReceiveMessageAsync(string? topic)
    {
        if (_client != null)
        {
            string selectedTopic = topic ?? _topic;
            try
            {
                await _client.SubscribeToTopic(selectedTopic);
                Console.WriteLine($"Subscribed to topic: {selectedTopic}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing to topic '{selectedTopic}': {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("MQTT client is not initialized. Please connect first.");
        }
    }

    public void OnMessageReceived(object? sender, SimpleMqttMessage? args)
    {
        try
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(args.Message);
            }

            Console.WriteLine($"Message received on topic '{args.Topic}': {args.Message}");

            // Raise the MessageReceived event
            MessageReceived?.Invoke(this, args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing received message: {ex.Message}");
        }
    }

    public void Disconnect()
    {
        if (_client != null)
        {
            _client.OnMessageReceived -= OnMessageReceived;
            _client.Dispose();
            Console.WriteLine("MQTT client disconnected.");
        }
    }

    public async Task SendMessage(string message, string? topic)
    {
        string selectedTopic = topic ?? _topic;
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

    public string? GetNextMessage()
    {
        lock (_messageQueue)
        {
            if (_messageQueue.Count > 0)
            {
                return _messageQueue.Dequeue();
            }
        }
        return null;
    }
}
