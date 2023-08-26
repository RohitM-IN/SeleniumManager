﻿using System.Diagnostics;
using System.Reflection;

namespace SeleniumManager.ConsoleApp
{
    internal class Program
    {
        static Process p;
        static void Main(string[] args)
        {
            string jarName = "selenium-server-4.11.0.jar"; // Name of your JAR file
            string jarPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), jarName); // Path to your JAR file
            string arguments = $@" -jar {jarPath} standalone";// --password password --username admin";// --selenium-manager true --log-level FINE --log ./trace.log";// --config D:\dev\C#\SeleniumManager\SeleniumManager.ConsoleApp\myconfig.toml";//--driver-implementation \"Chrome\"";
            ProcessStartInfo psi = new ProcessStartInfo("java", arguments);
            psi.CreateNoWindow = false; // Hide the console window
            psi.UseShellExecute = false; // Do not use the operating system shell to start the process
            p = new Process();
            p.StartInfo = psi;
            p.Start();
        }
    }
}