using System;
using UnityEngine;

namespace UnityMdDocGenerator {
	public enum LogLevel {
		Message,
		Warning,
		Error
	}

	public abstract class LoggerBase {

		public abstract void Log(LogLevel level, string message);

		public void LogFormat(LogLevel level, string message, params object[] args) {
			Log(level, string.Format(message, args));
		}

		public void Log(string message) {
			Log(LogLevel.Message, message);
		}

		public void LogFormat(string message, params object[] args) {
			Log(string.Format(message, args));
		}
	}

	public class UnityLogger:LoggerBase {
		public override void Log(LogLevel level, string message) {
			var logType = ConvertLogLevel(level);
			Debug.logger.Log(logType, message);
		}

		LogType ConvertLogLevel(LogLevel level) {
			switch(level) {
				case LogLevel.Message: return LogType.Log;
				case LogLevel.Warning: return LogType.Warning;
				case LogLevel.Error: return LogType.Error;
				default: throw new Exception(string.Format("Unknown log level: {0}", level));
			}
		}
	}
}
