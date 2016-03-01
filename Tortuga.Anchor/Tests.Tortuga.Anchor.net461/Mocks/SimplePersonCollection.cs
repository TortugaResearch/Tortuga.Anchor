using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Anchor.Modeling;

namespace Tests.Mocks
{
	public class SimplePersonCollection : SimpleModelCollection<SimplePerson>
	{
		/// <summary>
		/// Creates a model collection using the default property bag implementation..
		/// </summary>
		public SimplePersonCollection()
		{

		}
        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public SimplePersonCollection(List<SimplePerson> list)
            : base(list)
        {

        }
        /// <summary>
        /// Creates a model collection using the default property bag implementation..
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public SimplePersonCollection(IEnumerable<SimplePerson> collection)
            : base(collection)
        {

        }


        [Required()]
		public string FirstName
		{
			get { return Get<string>(); }
			set { Set(value); }
		}

		public string LastName
		{
			get { return GetDefault<string>(""); }
			set { Set(value); }
		}

		[CalculatedField("FirstName,LastName")]
		public string FullName
		{
			get { return (FirstName + " " + LastName).Trim(); }
		}

		protected override void OnValidateObject(ValidationResultCollection results)
		{
			base.OnValidateObject(results);

			if (FirstName == LastName && FirstName != "")
				results.Add(new ValidationResult("First and last names cannot match", new string[] { "FirstName", "LastName" }));
		}


        public SimplePerson Boss
        {
            get { return GetNew<SimplePerson>(() => new SimplePerson() { FirstName = "Da", LastName = "Boss" }); }
        }

        public SimplePerson Partner
        {
            get { return GetNew<SimplePerson>("Partner"); }
        }

        //public void InvokeBadPropertyMessage()
        //{
        //	base.OnPropertyChanged("Boom");
        //}


        public void InvokeGoodPropertyMessage()
		{
			base.OnPropertyChanged("FullName");
		}

		public void BadGetWithDefault()
		{
			base.GetDefault<int>(10, null);
		}


		public void BadGetWithDefault2()
		{
			base.GetDefault<int>(10, "");
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

		public void BadSet1()
		{
			base.Set(null, null);
		}

		public void BadSet2()
		{
			base.Set(null, "");
		}
	}
}
