using System.ComponentModel;

namespace Tests.Mocks;

public class Notifier : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;
	private int m_Age;
	public int Age
	{
		get { return m_Age; }
		set
		{
			m_Age = value;
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs("Age"));
		}
	}

	private string m_Name;
	public string Name
	{
		get { return m_Name; }
		set
		{
			m_Name = value;
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs("Name"));
		}
	}
}
