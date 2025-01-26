using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avans.StatisticalRobot;

public class Program
{
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Tzorg-robot initialized");

        RobotController tSentinel = new RobotController();

        // Initialize MQTT client with token and topic
        string mqttToken = "hivemq.webclient.1736429079800";
        string defaultTopic = "T-sentinel";

        var mqttController = new MqqtCOntroller(mqttToken, defaultTopic);

        // Connect to the MQTT broker
        mqttController.Connect();

        await mqttController.SendMessage("Online", "Robot");

        // Subscribe to the MQTT topic for controlling the robot

        await mqttController.ReceiveMessageAsync(defaultTopic);

        // Start message processing loop

        Console.WriteLine("Listening for MQTT messages...");
        await ProcessMessagesAsync(mqttController, _cancellationTokenSource.Token, tSentinel);

        Console.WriteLine("Program exiting.");
    }

    private static async Task ProcessMessagesAsync(MqqtCOntroller mqttController, CancellationToken token, RobotController tSentinel)
    {
        Console.WriteLine("ProcessMessagesAsync started.");
        while (!token.IsCancellationRequested)
        {
            // Dequeue the next message from the MQTT controller
            string? message = mqttController.GetNextMessage();
            short speed = 100;

            if (!string.IsNullOrEmpty(message) && message != "Online")
            {
                Console.WriteLine($"Processing received message: {message}");

                var routines = message.Split(",");

                // Repeat driving the route until a new message is received
                while (string.IsNullOrEmpty(mqttController.GetNextMessage()))
                {
                    // Move forward through the route
                    foreach (var item in routines)
                    {
                        tSentinel.Move(speed, item);
                        Console.WriteLine($"Direction: {item}, Speed: {speed}");
                        await Task.Delay(speed * 10); // Simulate robot movement
                    }

                    // Turn around before reversing the route
                    tSentinel.TurnAround();
                    Console.WriteLine("Robot is turning around before reversing the route.");
                    await Task.Delay(1000); // Simulate turn around time

                    // After completing the forward route, reverse the route
                    var reversedRoutines = routines.Reverse();

                    // Backtrack along the reversed route
                    foreach (var item in reversedRoutines)
                    {
                        tSentinel.Move(speed, item);
                        Console.WriteLine($"Reversing Direction: {item}, Speed: {speed}");
                        await Task.Delay(speed * 10); // Simulate robot movement
                    }
                }
            }
            else
            {
                // No message available; wait briefly to reduce CPU usage
                await Task.Delay(100);
            }
        }
    }
}
