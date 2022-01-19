using System.ComponentModel;
using Tortuga.Anchor.Collections;

namespace Tests.Mocks;

public class ReadOnlyFooCollection : ReadOnlyObservableCollectionExtended<Foo>
{
	public ReadOnlyFooCollection(ObservableCollectionExtended<Foo> list)
		: base(list)
	{

	}
	public int Boom
	{
		get { return ((FooCollection)base.SourceList).Boom; }
	}

	protected override void OnSourcePropertyChanged(string propertyName)
	{
		base.OnSourcePropertyChanged(propertyName);
		switch (propertyName)
		{
			case "Boom":
				OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
				break;
		}
	}
}
