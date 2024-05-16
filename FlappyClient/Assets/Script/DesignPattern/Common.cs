#if UNITY_EDITOR
#define DEBUG
#define ASSERT
#endif
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Common
{

	public static bool ALLOW_DEBUG = true;
	//-----------------------------------
	//--------------------- Log , warning, 

	[Conditional("DEBUG")]
	public static void Log(object message, Object context = null)
	{
		if (!ALLOW_DEBUG) return;
		if (context != null)
		{
			Debug.Log(message + " " + context.name, context);
		}
		else
		{
			Debug.Log(message, context);
		}
	}

	[Conditional("DEBUG")]
	public static void Log(string format, params object[] args)
	{
		if (!ALLOW_DEBUG) return;
		Debug.Log(string.Format(format, args));
	}

	[Conditional("DEBUG")]
	public static void LogWarning(object message, Object context)
	{
		if (!ALLOW_DEBUG) return;
		Debug.LogWarning(message, context);
	}

	[Conditional("DEBUG")]
	public static void LogWarning(Object context, string format, params object[] args)
	{
		if (!ALLOW_DEBUG) return;
		Debug.LogWarning(string.Format(format, args), context);
	}



	[Conditional("DEBUG")]
	public static void Warning(bool condition, object message)
	{
		if (!ALLOW_DEBUG) return;
		if ( ! condition) Debug.LogWarning(message);
	}

	[Conditional("DEBUG")]
	public static void Warning(bool condition, object message, Object context)
	{
		if (!ALLOW_DEBUG) return;
		if ( ! condition) Debug.LogWarning(message, context);
	}

	[Conditional("DEBUG")]
	public static void Warning(bool condition, Object context, string format, params object[] args)
	{
		if (!ALLOW_DEBUG) return;
		if ( ! condition) Debug.LogWarning(string.Format(format, args), context);
	}


	//---------------------------------------------
	//------------- Assert ------------------------

	/// Thown an exception if condition = false
	[Conditional("ASSERT")]
	public static void Assert(bool condition)
	{
		if (!ALLOW_DEBUG) return;
		if (! condition) throw new UnityException();
	}

	/// Thown an exception if condition = false, show message on console's log
	[Conditional("ASSERT")]
	public static void Assert(bool condition, string message)
	{
		if (!ALLOW_DEBUG) return;
		if (! condition) throw new UnityException(message);
	}

	/// Thown an exception if condition = false, show message on console's log
	[Conditional("ASSERT")]
	public static void Assert(bool condition, string format, params object[] args)
	{
		if (!ALLOW_DEBUG) return;
		if (! condition) throw new UnityException(string.Format(format, args));
	}
}

