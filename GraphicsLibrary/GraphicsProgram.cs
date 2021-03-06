﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Reflection;
using GraphicsLibrary.Core;

namespace GraphicsLibrary
{
	public class GraphicsProgram:IDisposable
	{
		public bool enableLogging;
		public string logFilename;
		public string[] programArguments;

		public Config config;

		public GraphicsProgram(string[] arguments, bool enableLogging, string logFilename)
		{

			RenderWindow.Instance.program = this;
			this.enableLogging = enableLogging;
			this.logFilename = logFilename;
			programArguments = arguments;

			System.Diagnostics.Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			if(enableLogging)
			{
				try
				{
					StreamWriter streamWriter = new StreamWriter(new FileStream(logFilename, FileMode.OpenOrCreate)) { AutoFlush = true };
					System.Diagnostics.Debug.Listeners.Add(new TextWriterTraceListener(streamWriter));
					
				}
				catch(Exception exception)
				{
					Debug.WriteLine("WARNING: Failed to create log file. Make sure you have admin rights and close all processes using this file: " + exception.Message + " @ " + exception.Source);
				}
			}
			Debug.WriteLine("---------------");
			Debug.WriteLine("Gallium");
			Debug.WriteLine("v{0}", Assembly.GetExecutingAssembly().GetName().Version);
			Debug.WriteLine("---------------");
			Debug.WriteLine("Created by:");
			Debug.WriteLine("Fons van der Plas (fons-), fonsvdplas@gmail.com");
			Debug.WriteLine("---------------");
			Debug.WriteLine("https://github.com/fons-");
			Debug.WriteLine("---------------");
			Debug.WriteLine("Program launched at {0}", DateTime.Now);
			Debug.Write("Received arguments: ");
			int i;
			for(i = 0; i < programArguments.Length - 1; i++)
			{
				Debug.Write(programArguments[i] + ", ");
			}
			if(programArguments.Length == 0)
			{
				Debug.Write("none");
			}
			else
			{
				Debug.Write(programArguments[0]);
			}
			Debug.WriteLine("");
		}

		public virtual void LoadResources()
		{

		}

		public virtual void InitGame()
		{

		}

		public virtual void Update(float timeSinceLastUpdate)
		{

		}

		public virtual void Resize(Rectangle newDimensions)
		{

		}

		public virtual bool ExecuteCommand(string command)
		{
			return false;
		}

		public void Run()
		{
			RenderWindow.Instance.Run();
		}

		public virtual void Dispose()
		{
			RenderWindow.Instance.Dispose();
			//throw new NotImplementedException(); //TODO
		}
	}
}