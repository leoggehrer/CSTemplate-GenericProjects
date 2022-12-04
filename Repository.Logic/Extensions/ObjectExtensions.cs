using System.Reflection;

namespace Repository.Logic.Extensions
{
    public static partial class ObjectExtensions
    {
        public static void CheckArgument(this object? source, string argName)
        {
            if (source == null)
                throw new ArgumentNullException(argName);
        }
        public static void CheckNotNull(this object? source, string itemName)
        {
            if (source == null)
                throw new ArgumentNullException(itemName);
        }
        public static T CopyTo<T>(this object source) where T : class, new()
        {
            var target = new T();

            CopyProperties(target, source);
            return target;
        }
        public static T CopyTo<T>(this object source, Func<string, bool> filter) where T : class, new()
        {
            var target = new T();

            CopyProperties(target, source, filter, null);
            return target;
        }
        public static T CopyTo<T>(this object source, Func<string, string>? mapping) where T : class, new()
        {
            var target = new T();

            CopyProperties(target, source, null, mapping);
            return target;
        }
        public static T CopyTo<T>(this object source, Func<string, bool>? filter, Func<string, string>? mapping) where T : class, new()
        {
            var target = new T();

            CopyProperties(target, source, filter, mapping);
            return target;
        }

        public static void CopyTo(this object source, object target)
        {
            CopyProperties(target, source);
        }
        public static void CopyTo(this object source, object target, Func<string, bool> filter)
        {
            CopyProperties(target, source, filter, null);
        }
        public static void CopyTo(this object source, object target, Func<string, string> mapping)
        {
            CopyProperties(target, source, null, mapping);
        }
        public static void CopyTo(this object source, object target, Func<string, bool> filter, Func<string, string> mapping)
        {
            CopyProperties(target, source, filter, mapping);
        }

        public static void CopyFrom(this object target, object source)
        {
            if (source != null)
            {
                CopyProperties(target, source);
            }
        }
        public static void CopyFrom(this object target, object source, Func<string, bool> filter)
        {
            if (source != null)
            {
                CopyProperties(target, source, filter, null);
            }
        }
        public static void CopyFrom(this object target, object source, Func<string, string> mapping)
        {
            if (source != null)
            {
                CopyProperties(target, source, null, mapping);
            }
        }
        public static void CopyFrom(this object target, object source, Func<string, bool> filter, Func<string, string> mapping)
        {
            if (source != null)
            {
                CopyProperties(target, source, filter, mapping);
            }
        }

        public static void CopyProperties(object target, object source)
        {
            CopyProperties(target, source, null, null);
        }
        public static void CopyProperties(object target, object source, Func<string, bool>? filter, Func<string, string>? mapping)
        {
            Dictionary<string, PropertyItem> targetPropertyInfos = target.GetType().GetAllTypeProperties();
            Dictionary<string, PropertyItem> sourcePropertyInfos = source.GetType().GetAllTypeProperties();

            SetPropertyValues(target, source, filter, mapping, targetPropertyInfos, sourcePropertyInfos);
        }

        private static void SetPropertyValues(object target, object source, Func<string, bool>? filter, Func<string, string>? mapping, Dictionary<string, PropertyItem> targetPropertyInfos, Dictionary<string, PropertyItem> sourcePropertyInfos)
        {
            filter ??= (n => true);
            mapping ??= (n => n);
            foreach (KeyValuePair<string, PropertyItem> propertyItemTarget in targetPropertyInfos)
            {
                if (sourcePropertyInfos.TryGetValue(mapping(propertyItemTarget.Value.PropertyInfo.Name), out var propertyItemSource))
                {
                    if (propertyItemSource.PropertyInfo.PropertyType == propertyItemTarget.Value.PropertyInfo.PropertyType
                        && ((propertyItemSource.CanReadAndIsPublic && propertyItemTarget.Value.CanWriteAndIsPublic)
                            || (propertyItemSource.DeclaringType == propertyItemTarget.Value.DeclaringType && propertyItemSource.CanRead && propertyItemTarget.Value.CanWrite))
                        && (filter(propertyItemTarget.Value.PropertyInfo.Name)))
                    {
                        if (propertyItemSource.IsStringType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);

                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.IsArrayType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);

                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.PropertyInfo.PropertyType.IsValueType
                            && propertyItemTarget.Value.PropertyInfo.PropertyType.IsValueType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);

                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                        else if (propertyItemSource.IsComplexType)
                        {
                            object? value = propertyItemSource.PropertyInfo.GetValue(source);

                            propertyItemTarget.Value.PropertyInfo.SetValue(target, value);
                        }
                    }
                }
            }
        }
    }
}
