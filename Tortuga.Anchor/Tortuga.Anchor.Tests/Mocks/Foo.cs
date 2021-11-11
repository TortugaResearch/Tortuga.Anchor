using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tests.Mocks;

public class Foo : INotifyPropertyChanged
{
	readonly Collection<Bar> m_Bars = new();
	private string m_FooBar = null!;

	public event PropertyChangedEventHandler? PropertyChanged;

	public Collection<Bar> Bars
	{
		get { return m_Bars; }
	}

	public string FooBar
	{
		get { return m_FooBar; }
		set
		{
			if (m_FooBar == value)
				return;

			m_FooBar = value;

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FooBar"));
		}
	}
}
