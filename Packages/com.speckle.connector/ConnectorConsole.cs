using System;
using UnityEngine;

namespace Speckle.ConnectorUnity
{

	public static class ConnectorConsole

	{
		public const string title = "speckle-connector:";

		public static void Log(string msg)
		{
			Debug.Log(title + msg);
		}
		public static void Exception(Exception exception)
		{
			Debug.LogException(exception);
		}
		public static void Warn(string message)
		{
			Debug.LogWarning(title + message);
		}
	}
}