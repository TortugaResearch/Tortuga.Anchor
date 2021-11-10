using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Anchor.Modeling;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Mocks;

public class SimplePerson : ModelBase
{
	private int m_SecretaryChangeCounter;

	public int Age
	{
		get { return GetDefault<int>(0); }
		set { Set(value); }
	}

	public SimplePerson Boss
	{
		get { return GetNew<SimplePerson>(() => new SimplePerson() { FirstName = "Da", LastName = "Boss" }); }
	}

	public DateTime DateOfBirth
	{
		get { return Get<DateTime>(); }
		set { Set(value, DateOfBirthChanged); }
	}

	[Required()]
	public string FirstName
	{
		get { return Get<string>(); }
		set { Set(value); }
	}

	[CalculatedField("FirstName,LastName")]
	public string FullName
	{
		get { return (FirstName + " " + LastName).Trim(); }
	}

	public string LastName
	{
		get { return GetDefault<string>(""); }
		set { Set(value); }
	}
	public SimplePerson Partner
	{
		get { return GetNew<SimplePerson>(); }
	}

	public DateTime PreviousDateOfBirth
	{
		get { return Get<DateTime>(); }
		set { Set(value); }
	}

	public SimplePerson Secretary
	{
		get { return Get<SimplePerson>(); }
		set { Set(value, Secretary_Changed); }
	}

	public int SecretaryChangeCounter
	{
		get { return m_SecretaryChangeCounter; }
	}

	public void BadGet()
	{
		base.Get<int>(null);
	}

	public void BadGet2()
	{
		base.Get<int>("");
	}

	public void BadGetNew1()
	{
		base.GetNew<int>(() => 1, null);
	}

	public void BadGetNew2()
	{
		base.GetNew<int>(() => 1, "");
	}

	public void BadGetNew3()
	{
		base.GetNew<int>(null, "");
	}

	public void BadGetNew4()
	{
		base.GetNew<int>(null);
	}

	public void BadGetNew5()
	{
		base.GetNew<int>("");
	}

	public void BadGetNew6()
	{
		base.GetNew(null);
	}

	public void BadGetNew7()
	{
		base.GetNew("");
	}

	public void BadGetWithDefault()
	{
		base.GetDefault<int>(10, null);
	}

	public void BadGetWithDefault2()
	{
		base.GetDefault<int>(10, "");
	}

	public void BadSet1()
	{
		base.Set(null, null);
	}

	public void BadSet2()
	{
		base.Set(null, "");
	}

	public PropertyBag GetPropertyBag()
	{
		return base.Properties;
	}

	public void InvokeBadPropertyMessage()
	{
		base.OnPropertyChanged("Boom");
	}

	public void InvokeGoodPropertyMessage()
	{
		base.OnPropertyChanged("FullName");
	}

	protected override void OnValidateObject(ValidationResultCollection results)
	{
		base.OnValidateObject(results);

		if (FirstName == LastName && FirstName != "")
			results.Add(new ValidationResult("First and last names cannot match", new string[] { "FirstName", "LastName" }));
	}

	void DateOfBirthChanged(DateTime oldValue, DateTime newValue)
	{
		PreviousDateOfBirth = oldValue;
	}
	void Secretary_Changed(object sender, PropertyChangedEventArgs e)
	{
		m_SecretaryChangeCounter += 1;
	}
}
