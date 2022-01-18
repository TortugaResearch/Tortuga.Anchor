using Tortuga.Anchor.Collections;

namespace Tests.Mocks;

public class NotifierCollection : ObservableCollectionExtended<Notifier>
{
	public void RaisePropertyName(string propertyName)
	{
		OnPropertyChanged(propertyName);
	}
}


public class ReadOnlyNotifierCollection : ReadOnlyObservableCollectionExtended<Notifier>
{
	public ReadOnlyNotifierCollection(ObservableCollectionExtended<Notifier> list)
		: base(list)
	{

	}

	public void RaisePropertyName(string propertyName)
	{
		OnPropertyChanged(propertyName);
	}
}
