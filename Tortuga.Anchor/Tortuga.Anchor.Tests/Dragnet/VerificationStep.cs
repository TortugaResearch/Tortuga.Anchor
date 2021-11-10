using System.Text;

namespace Tortuga.Dragnet;

public class VerificationStep
{
	public VerificationStep(string? checkType, string message, Severity severity)
	{
		Message = message;
		CheckType = checkType;
		Severity = severity;
		if (severity != Severity.Message)
			StackTrace = Environment.StackTrace;
	}

	public Severity Severity { get; private set; }
	public string? StackTrace { get; private set; }
	public string? CheckType { get; private set; }
	public string Message { get; private set; }

	public override string ToString()
	{
		var result = new StringBuilder();

		if (!string.IsNullOrWhiteSpace(CheckType))
			result.AppendLine(CheckType);
		if (!string.IsNullOrWhiteSpace(Message))
			result.AppendLine(Message);
		if (!string.IsNullOrWhiteSpace(StackTrace))
			result.AppendLine(StackTrace);

		return result.ToString();
	}
}
