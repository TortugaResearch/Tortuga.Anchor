//using System;

//namespace Tortuga.Anchor.Modeling.Internals
//{
//    /// <summary>
//    /// This is used to indicate changes to an object's IsChanged property.
//    /// </summary>

//    public class IsChangedPropertyChangedEventArgs : EventArgs
//    {
//        private readonly bool _isChanged;

//        /// <summary>
//        /// Initializes a new instance of the ChildPropertyChangedEventArgs class.
//        /// </summary>
//        /// <param name="isChanged">Use the value of child object's IsChanged property</param>
//        private IsChangedPropertyChangedEventArgs(bool isChanged)
//        {
//            _isChanged = isChanged;
//        }

//        /// <summary>
//        /// The new value of the child object's IsChanged property. 
//        /// </summary>
//        public bool IsChanged
//        {
//            get { return _isChanged; }
//        }

//        /// <summary>
//        /// Gets the default instance of this class for the given parameters. 
//        /// </summary>
//        /// <param name="isChanged">Use the value of child object's IsChanged property</param>
//        /// <returns></returns>
//        public static IsChangedPropertyChangedEventArgs Default(bool isChanged)
//        {
//            if (isChanged)
//                return True;
//            else
//                return False;
//        }

//        private static readonly IsChangedPropertyChangedEventArgs True = new IsChangedPropertyChangedEventArgs(true);
//        private static readonly IsChangedPropertyChangedEventArgs False = new IsChangedPropertyChangedEventArgs(false);

//    }
//}
