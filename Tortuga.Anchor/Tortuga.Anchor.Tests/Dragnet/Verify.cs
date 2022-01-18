using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tortuga.Dragnet;

public class Verify : IDisposable
{
	readonly ConcurrentQueue<VerificationStep> m_TestResults = new();

	public bool AreEqual<T>(T? expected, T? actual, string message)
	{
		if (expected == null && actual == null)
			return true;

		if (expected == null)
		{
			Fail($"Expected value was <null> but the actual value was \"{actual}\". {message}");
			return false;
		}

		if (actual == null)
		{
			Fail($"Expected value was \"{expected}\" but the actual value was <null>. {message}");
			return false;
		}

		if (ReferenceEquals(expected, actual))
			return true;

		if (expected is IEquatable<T> equatable && equatable.Equals(actual))
			return true;

		if (object.Equals(expected, actual))
			return true;

		Fail($"Expected value was \"{expected}\" but the actual value was \"{actual}\". {message}");

		return false;
	}

	public bool AreEqual(double expected, double actual, double tolerance, string message)
	{
		var min = expected - tolerance;
		var max = expected + tolerance;
		if (actual < min || actual > max)
		{
			Fail($"Value {actual} is outside the tolerance [{min}, {max}]. {message}");
			return false;
		}
		return true;
	}

	public ArgumentException ArgumentException(string expectedParamName, Action body)
	{
		var ex = Exception<ArgumentException>(body);
		AreEqual(expectedParamName, ex.ParamName, "ArgumentException.ParamName was incorrectly set");
		return ex;
	}

	public ArgumentException ArgumentException(string expectedParamName, Action body, string message)
	{
		var ex = Exception<ArgumentException>(body, message);
		AreEqual(expectedParamName, ex.ParamName, "ArgumentException.ParamName was incorrectly set");
		return ex;
	}

	public ArgumentNullException ArgumentNullException(string expectedParamName, Action body)
	{
		var ex = Exception<ArgumentNullException>(body);
		AreEqual(expectedParamName, ex.ParamName, "ArgumentNullException.ParamName was incorrectly set");
		return ex;
	}

	public ArgumentOutOfRangeException ArgumentOutOfRangeException(string expectedParamName, string expectedActualValue, Action body)
	{
		var ex = Exception<ArgumentOutOfRangeException>(body);
		AreEqual(expectedParamName, ex.ParamName, "ArgumentOutOfRangeException.ParamName was incorrectly set");
		Assert.AreEqual(expectedActualValue, ex.ActualValue, "ArgumentOutOfRangeException.ActualValue was incorrectly set");
		return ex;
	}

	public ArgumentOutOfRangeException ArgumentOutOfRangeException(string expectedParamName, int expectedActualValue, Action body)
	{
		var ex = Exception<ArgumentOutOfRangeException>(body);
		AreEqual(expectedParamName, ex.ParamName, "ArgumentOutOfRangeException.ParamName was incorrectly set");
		Assert.AreEqual(expectedActualValue, ex.ActualValue, "ArgumentOutOfRangeException.ActualValue was incorrectly set");
		return ex;
	}

	public ArgumentOutOfRangeException ArgumentOutOfRangeException(string expectedParamName, int expectedActualValue, Action<int> body)
	{
		var ex = Exception<ArgumentOutOfRangeException>(() => body(expectedActualValue));
		AreEqual(expectedParamName, ex.ParamName, "ArgumentOutOfRangeException.ParamName was incorrectly set");
		Assert.AreEqual(expectedActualValue, ex.ActualValue, "ArgumentOutOfRangeException.ActualValue was incorrectly set");
		return ex;
	}

	void IDisposable.Dispose()
	{
		if (m_TestResults.Any(x => x.Severity == Severity.Inconclusive))
			throw new AssertInconclusiveException(ToString());
		else if (m_TestResults.Any(x => x.Severity == Severity.Failed))
			throw new AssertFailedException(ToString());
	}

	public TException Exception<TException>(Action body) where TException : Exception
	{
		if (body == null)
			throw new ArgumentNullException("body", "body is null.");
		try
		{
			body();
			Fail(string.Format("Expected an exception of type {0}", typeof(TException).Name));
		}
		catch (TException ex)
		{
			return ex;
		}
		catch (Exception ex)
		{
			Fail(string.Format("Expected exception of type {0} but got a {1}", typeof(TException).Name, ex.GetType().Name));
		}

		throw new InvalidProgramException(); //this line can never be hit.
	}

	public TException Exception<TException>(Action body, string message) where TException : Exception
	{
		if (body == null)
			throw new ArgumentNullException(nameof(body), "body is null.");
		try
		{
			body();
		}
		catch (TException ex)
		{
			return ex;
		}
		catch (Exception ex)
		{
			Fail(string.Format("Expected exception of type {0} but got a {1}", typeof(TException).Name, ex.GetType().Name));
		}
		Fail(string.Format("Expected an exception of type {0} because {1}", typeof(TException).Name, message));

		throw new InvalidProgramException(); //this line can never be hit.
	}

	public async Task<TException> ExceptionAsync<TException>(Func<Task> body, string message) where TException : Exception
	{
		try
		{
			await body();
		}
		catch (TException ex)
		{
			return ex;
		}
		catch (Exception ex)
		{
			Fail(string.Format("Expected exception of type {0} but got a {1}", typeof(TException).Name, ex.GetType().Name));
		}
		Fail(string.Format("Expected an exception of type {0} because {1}", typeof(TException).Name, message));

		throw new Exception(); //this line can never be hit.
	}

	public void Fail(string message)
	{
		m_TestResults.Enqueue(new VerificationStep(null, message, Severity.Failed));
	}

	public void Inconclusive(string message)
	{
		m_TestResults.Enqueue(new VerificationStep(null, message, Severity.Inconclusive));
	}

	public bool IsBetween<T>(T minValue, T actualValue, T maxValue, string message) where T : IComparable<T>
	{
		var result = true;

		if (minValue.CompareTo(actualValue) > 0)
		{
			Fail($"Actual actualValue {actualValue} is less than allowed minimum {minValue}. {message}");
			result = false;
		}

		if (actualValue.CompareTo(maxValue) > 0)
		{
			Fail($"Actual actualValue {actualValue} is greater than allowed maximum {maxValue}. {message}");
			result = false;
		}

		return result;
	}

	public bool IsFalse(bool actual, string message)
	{
		return AreEqual(false, actual, message);
	}

	public bool IsNotNull<T>(T? actual, string message) where T : class
	{
		if (actual == null)
		{
			Fail($"Expected value was not null but the actual value was <null>. {message}");
			return false;
		}

		return true;
	}

	public bool IsNull<T>(T? actual, string message) where T : class
	{
		if (actual != null)
		{
			Fail($"Expected value was <null> but the actual value was {actual}. {message}");
			return false;
		}

		return true;
	}

	public bool IsTrue(bool actual, string message)
	{
		return AreEqual(true, actual, message);
	}

	public bool IsTrueForAll<T>(IEnumerable<T> list, Func<T, bool> predicate, string message)
	{
		if (list == null)
			return true;

		var count = 0;
		var result = true;
		foreach (var item in list)
		{
			var actual = predicate(item);
			result = result && actual;

			IsTrue(actual, message + $" [Item {count}]");

			count++;
		}

		return result;
	}

	public void ItemsAreEqual<T>(IList<T> expectedList, IList<T> actualList, string message)
	{
		IsNotNull(expectedList, $"expectedValue list. {message}");
		IsNotNull(actualList, $"actualValue list. {message}");

		AreEqual(expectedList.Count, actualList.Count, $"The counts differ between the two lists. {message}");
		for (var i = 0; i < expectedList.Count; i++)
		{
			AreEqual(expectedList[i], actualList[i], $"The lists differ in slot {i}. {message}");
		}
	}

	public void ItemsAreEqual<T>(IEnumerable<T> expectedEnumerable, IEnumerable<T> actualEnumerable, string message)
	{
		IsNotNull(expectedEnumerable, $"expectedValue enumerable {message}");
		IsNotNull(actualEnumerable, $"actualValue enumerable {message}");

		var expectedValue = expectedEnumerable.GetEnumerator();
		var actualValue = actualEnumerable.GetEnumerator();
		var index = 0;

		while (expectedValue.MoveNext())
		{
			IsTrue(actualValue.MoveNext(), $"actualValue had too few items. {message}");
			AreEqual(expectedValue.Current, actualValue.Current, $"expectedValue doesn't match actualValue in slot {index}. {message}");
			index++;
		}

		IsFalse(actualValue.MoveNext(), $"actualValue had too many items. {message}");
	}

	public void ItemsAreSame<T>(IList<T> expectedList, IList<T> actualList, string message) where T : class
	{
		IsNotNull(expectedList, $"expectedValue list. {message}");
		IsNotNull(actualList, $"actualValue list. {message}");

		AreEqual(expectedList.Count, actualList.Count, $"The counts differ between the two lists. {message}");
		for (var i = 0; i < expectedList.Count; i++)
		{
			AreSame(expectedList[i], actualList[i], $"The lists differ in slot {i}. {message}");
		}
	}

	public void ItemsAreSame<T>(IEnumerable<T> expectedEnumerable, IEnumerable<T> actualEnumerable, string message) where T : class
	{
		IsNotNull(expectedEnumerable, "expectedValue enumerable");
		IsNotNull(actualEnumerable, "actualValue enumerable");

		var expectedValue = expectedEnumerable.GetEnumerator();
		var actualValue = actualEnumerable.GetEnumerator();
		var index = 0;

		while (expectedValue.MoveNext())
		{
			IsTrue(actualValue.MoveNext(), $"actualValue had too few items. {message}");
			AreSame(expectedValue.Current, actualValue.Current, $"expectedValue doesn't match actualValue in slot {index}. {message}");
			index++;
		}

		IsFalse(actualValue.MoveNext(), $"actualValue had too many items. {message}");
	}

	public NotSupportedException NotSupportedException(Action body)
	{
		return Exception<NotSupportedException>(body, "This operation was expectedValue to throw a not supported exception.");
	}

	public override string ToString()
	{
		string Breaker = Environment.NewLine + "*****" + Environment.NewLine;
		return string.Join(Breaker, m_TestResults.Select(x => x.ToString()));
	}

	internal bool AreSame<T>(T? expected, T? actual, string message)
		where T : class
	{
		if (expected == null && actual == null)
			return true;

		if (expected == null)
		{
			Fail($"Expected value was <null> but the actual value was {actual}. {message}");
			return false;
		}

		if (actual == null)
		{
			Fail($"Expected value was {expected} but the actual value was <null>. {message}");
			return false;
		}

		if (ReferenceEquals(expected, actual))
			return true;

		if (expected is IEquatable<T> equatable && equatable.Equals(actual) || object.Equals(expected, actual))
		{
			Fail($"Expected value equals actual value, but they aren't the same object. {message}");
			return false;
		}

		Fail($"Expected value was {expected} but the actual value was {actual}. {message}");

		return false;
	}

	internal bool IsEmpty<T>(IReadOnlyCollection<T> list, string message)
	{
		return AreEqual(0, list.Count, $"Expected the collection to be empty. {message}");
	}

	internal void WriteLine(string message)
	{
		Debug.WriteLine(message);
		Add(message, Severity.Message);
	}

	void Add(string message, Severity severity)
	{
		m_TestResults.Enqueue(new VerificationStep(null, message, severity));
	}

	//void Add(string checkType, string message, Severity severity)
	//{
	//	m_TestResults.Enqueue(new VerificationStep(checkType, message, severity));
	//}
}
