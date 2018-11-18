using System;
using System.Reflection;

namespace ClumsyBat.DataManagement
{
    [Serializable]
    public class CloneableContainerBase<T> where T : new()
    {
        public T Clone()
        {
            T clone = new T();
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                field.SetValue(clone, field.GetValue(this));
            }

            return clone;
        }
    }
}
