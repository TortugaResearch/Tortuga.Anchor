using System;
using System.ComponentModel.DataAnnotations;
using Tortuga.Anchor.DataAnnotations;
using Tortuga.Anchor.Modeling;
using Tortuga.Anchor.Modeling.Internals;

namespace Tests.Mocks
{
    public class EditablePerson : EditableObjectModelBase
    {
        [Required()]
        public string FirstName
        {
            get { return Get<string>(); }
            set { Set(value); }
        }

        public string? MiddleName
        {
            get { return Get<string?>(); }
            set { Set(value); }
        }

        public string LastName
        {
            get { return GetNew(); }
            set { Set(value); }
        }

        [CalculatedField("FirstName,LastName")]
        public string FullName
        {
            get { return (FirstName + " " + LastName).Trim(); }
        }

        public int Age
        {
            get { return GetDefault<int>(0); }
            set { Set(value); }
        }

        public DateTime DateOfBirth
        {
            get { return Get<DateTime>(); }
            set { Set(value, DateOfBirthChanged); }
        }

        public DateTime PreviousDateOfBirth
        {
            get { return Get<DateTime>(); }
            set { Set(value); }
        }

        void DateOfBirthChanged(DateTime oldValue, DateTime newValue)
        {
            PreviousDateOfBirth = oldValue;
        }

        protected override void OnValidateObject(ValidationResultCollection results)
        {
            base.OnValidateObject(results);

            if (FirstName == LastName && FirstName != "")
                results.Add(new ValidationResult("First and last names cannot match", new string[] { "FirstName", "LastName" }));
        }

        public Tests.Mocks.EditablePerson Boss
        {
            get
            {
                return GetNew<Tests.Mocks.EditablePerson>(() =>
                {
                    var newBoss = new Tests.Mocks.EditablePerson() { FirstName = "Da", LastName = "Boss" };
                    newBoss.AcceptChanges();
                    return newBoss;
                }, "Boss");
            }
        }

        public Tests.Mocks.SimplePerson Partner
        {
            get
            {
                return GetNew<Tests.Mocks.SimplePerson>("Partner");
            }
        }

        public Tests.Mocks.MockChangeTracking DummyObject
        {
            get { return GetNew<Tests.Mocks.MockChangeTracking>(); }
            set { Set(value); }
        }

        public void InvokeBadPropertyMessage()
        {
            base.OnPropertyChanged("Boom");
        }

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

        public EditableObjectPropertyBag GetPropertyBag()
        {
            return base.Properties;
        }

        public void AcceptChangesLocal()
        {
            Properties.AcceptChanges(false);
        }
    }
}
