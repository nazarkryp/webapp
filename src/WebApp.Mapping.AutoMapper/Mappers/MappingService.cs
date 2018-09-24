using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoMapper;
using Newtonsoft.Json;

namespace WebApp.Mapping.AutoMapper.Mappers
{
    public class MappingService : IMapper
    {
        static MappingService()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("WebApp."));

            var profiles = assemblies.SelectMany(FindAllDerivedTypes<Profile>).ToList();

            Mapper.Initialize(configuration =>
            {
                profiles.ForEach(profile =>
                {
                    configuration.AddProfile(Activator.CreateInstance(profile) as Profile);
                });

                configuration.Advanced.AllowAdditiveTypeMapCreation = true;
            });

            Mapper.AssertConfigurationIsValid();
        }

        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public TDestination Map<TDestination>(Dictionary<string, object> properties) where TDestination : new()
        {
            var result = new TDestination();
            var destinationType = result.GetType();

            foreach (var property in properties)
            {
                var destinationProperty = destinationType.GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (destinationProperty == null)
                {
                    continue;
                }

                var value = GetValue(destinationProperty, property);
                destinationProperty.SetValue(result, value, null);
            }

            return result;
        }

        private Type GetMemberType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException($"Invalid {nameof(member)} parameter.");
            }
        }

        private static object GetValue(PropertyInfo sourceProperty, KeyValuePair<string, object> property)
        {
            var value = sourceProperty.PropertyType.IsValueType
                ? GetValueTypeValue(sourceProperty, property)
                : GetReferenceTypeValue(sourceProperty, property);

            return value;
        }

        private static object GetValueTypeValue(PropertyInfo sourceProperty, KeyValuePair<string, object> property)
        {
            object value;

            var propertyType = sourceProperty.PropertyType;
            var enumValue = GetEnumValue(sourceProperty, property.Value);

            if (enumValue != null)
            {
                value = enumValue;
            }
            else
            {
                var type = property.Value != null
                    ? (Nullable.GetUnderlyingType(propertyType) ?? propertyType)
                    : propertyType;
                value = property.Value != null ? Convert.ChangeType(property.Value, type) : GetDefaultValue(type);
            }

            return value;
        }

        private static object GetReferenceTypeValue(PropertyInfo sourceProperty, KeyValuePair<string, object> property)
        {
            var propertyType = sourceProperty.PropertyType;

            var value = property.Value == null || property.Value.GetType() == propertyType
                ? property.Value
                : JsonConvert.DeserializeObject(property.Value.ToString(), propertyType);

            return value;
        }

        private static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);

            return assembly.GetTypes()
                .Where(t => t != derivedType && derivedType.IsAssignableFrom(t))
                .ToList();
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private static object GetEnumValue(PropertyInfo sourceProperty, object value)
        {
            object result = null;
            var sourcePropertyType = sourceProperty.PropertyType;

            if (sourcePropertyType.IsEnum)
            {
                var enumValue = !string.IsNullOrWhiteSpace(value?.ToString()) ? value.ToString() : "0";
                result = Enum.Parse(sourcePropertyType, enumValue, true);
            }
            else if (IsNullableEnum(sourcePropertyType))
            {
                var type = Nullable.GetUnderlyingType(sourcePropertyType);
                result = !string.IsNullOrWhiteSpace(value?.ToString()) ? Enum.Parse(type, value.ToString(), true) : null;
            }

            return result;
        }

        private static bool IsNullableEnum(Type type)
        {
            var isNullableEnum = Nullable.GetUnderlyingType(type)?.IsEnum;

            return isNullableEnum.GetValueOrDefault();
        }
    }
}
