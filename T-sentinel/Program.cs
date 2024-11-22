using System.Device.Gpio;
using Avans.StatisticalRobot;
using System;
using System.Threading;

Console.WriteLine("Tzorg-robot initialized");

while(true) {
    ConvenienceFunctions.move(90, "left");
    Thread.Sleep(1000);
    ConvenienceFunctions.move(90, "forward");
    Thread.Sleep(1000);
    ConvenienceFunctions.move(90, "right");
    Thread.Sleep(1000);
    ConvenienceFunctions.move(90, "backward");
    Thread.Sleep(1000);
}


/// <summary>
/// Convenience functions for the robot
/// </summary>
static class ConvenienceFunctions
{
    /// <summary>
    /// Move the robot in a certain direction with a certain speed
    /// </summary>
    /// <param name="speed"> dictates the speed </param>
    /// <param name="direction">dictates the direction</param>
    public static void move(short speed, string direction){
        if(direction == "forward"){
            Robot.Motors(speed, speed);
        }
        else if(direction == "backward"){
            speed = (short)(speed * -1);
            Robot.Motors(speed, speed);     
        }
        else if(direction == "left"){
            Robot.Motors(speed, (short)(speed * -1));
        }
        else if(direction == "right"){
            Robot.Motors((short)(speed * -1), speed);
        }
        else if(direction == "stop"){
            Robot.Motors(0, 0);
        }
    }
}