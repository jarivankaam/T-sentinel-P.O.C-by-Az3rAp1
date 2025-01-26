using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Linq;
using System.Threading;
using System.Timers;
using Avans.StatisticalRobot;
using Timer = System.Timers.Timer;


public class RobotController
{
    private bool _isDriving;
    private short _currentSpeed;
    private string _currentDirection;

    // Constructor
    public RobotController()
    {
        _isDriving = false;
        _currentSpeed = 0;
        _currentDirection = "stop";
        Console.WriteLine("Robot Controller initialized.");
    }

    /// <summary>
    /// Updates the LCD with the specified text and dynamically detected I2C address.
    /// </summary>
    /// <param name="text">Text to display on the LCD.</param>
    public void lcd(string text, byte address)
    {
        LCD16x2 lcd = new LCD16x2(address);
        lcd.SetText(text);
    }

    /// <summary>
    /// Moves the robot in a specific direction at a specific speed, with obstacle detection.
    /// </summary>
    /// <param name="speed">The speed at which to move (0-100).</param>
    /// <param name="direction">The direction to move in ("forward", "backward", "left", "right", "stop").</param>
    /// <param name="ultrasonicPin">The GPIO pin connected to the ultrasonic sensor.</param>
    /// <param name="safeDistance">The safe distance in cm to stop the robot if an obstacle is detected.</param>
    public void Move(short speed, string direction, int ultrasonicPin = -1, int safeDistance = 20)
    {
        if (speed < 0 || speed > 100)
        {
            Console.WriteLine("Speed must be between 0 and 100.");
            return;
        }

        // Check for obstacles if ultrasonicPin is provided
        if (ultrasonicPin >= 0)
        {
            int distance = MeasureDistance(ultrasonicPin);
            if (distance > 0 && distance < safeDistance)
            {
                Console.WriteLine($"Obstacle detected at {distance} cm! Stopping the robot.");
                Stop();
                return;
            }
        }

        _isDriving = direction.ToLower() != "stop";
        _currentSpeed = speed;
        _currentDirection = direction.ToLower();

        switch (_currentDirection)
        {
            case "forward":
                Robot.Motors(speed, speed);
                Thread.Sleep(speed * 100);
                break;

            case "backward":
                Robot.Motors((short)(-speed), (short)(-speed));
                Thread.Sleep(speed * 10);
                break;

            case "left":
                Robot.Motors(speed, 0);
                Thread.Sleep(speed * 100);
                break;

            case "right":
                Robot.Motors(0, speed);
                Thread.Sleep(speed * 100);
                break;

            case "stop":
                Robot.Motors(0, 0);
                Thread.Sleep(speed * 100);
                break;

            default:
                Console.WriteLine($"Unknown direction: {direction}");
                break;
        }

        Thread.Sleep(speed * 10);
        Console.WriteLine($"Moving {direction} at speed {speed}.");
    }


    /// <summary>
    /// Stops the robot immediately.
    /// </summary>
    public void Stop()
    {
        _isDriving = false;
        _currentSpeed = 0;
        _currentDirection = "stop";
        Robot.Motors(0, 0);
        Console.WriteLine("Robot stopped.");
    }

    public void TurnAround()
    {
        Console.WriteLine("Robot is turning around.");
        Move(50, "left");
        Thread.Sleep(2000); // Simulate turn duration
        Stop();
        Console.WriteLine("Turnaround complete.");
    }

    /// <summary>
    /// Retrieves the current status of the robot.
    /// </summary>
    /// <returns>A string describing the robot's current state.</returns>
    public string GetStatus()
    {
        return _isDriving
            ? $"Robot is moving {_currentDirection} at speed {_currentSpeed}."
            : "Robot is stationary.";
    }

    /// <summary>
    /// Uses the ultrasonic sensor to measure distance.
    /// </summary>
    /// <param name="pin">The GPIO pin connected to the ultrasonic sensor.</param>
    /// <returns>The measured distance in cm.</returns>
    public int MeasureDistance(int pin)
    {
        try
        {
            Ultrasonic ultrasonicSensor = new Ultrasonic(pin);
            int distance = ultrasonicSensor.GetUltrasoneDistance();

            Console.WriteLine($"Measured distance: {distance} cm");
            return distance;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error measuring distance: {ex.Message}");
            return -1; // Return -1 in case of an error
        }
    }

    public void CheckAndAvoidObstacle(int ultrasonicPin, int safeDistance)
    {
        int distance = MeasureDistance(ultrasonicPin);

        if (distance > 0 && distance < safeDistance)
        {
            Console.WriteLine("Obstacle detected! Stopping the robot.");
            Stop();
        }
        else
        {
            Console.WriteLine("Path is clear. Continuing operation.");
        }
    }
}


