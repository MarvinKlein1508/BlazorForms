using System.Collections;
using System.Reflection;

namespace BlazorForms.Core.Extensions
{
    public static class ObjectExtensions
    {
        private static bool HasEnumerableChanged(object original, object vergleich)
        {
            IEnumerable? originalEnumerable = original as IEnumerable;
            IEnumerable? vergleichEnumerable = vergleich as IEnumerable;


            if (originalEnumerable is null && vergleichEnumerable is null)
            {
                return false;
            }

            if (originalEnumerable is null || vergleichEnumerable is null)
            {
                return true;
            }

            int originalCount = 0;
            int vergleichCount = 0;
            foreach (var item in originalEnumerable)
            {
                originalCount++;
            }

            foreach (var item in vergleichEnumerable)
            {
                vergleichCount++;
            }

            if (originalCount != vergleichCount)
            {
                return true;
            }

            foreach (var item in originalEnumerable)
            {
                bool foundOneMatching = false;
                foreach (var modItem in vergleichEnumerable)
                {
                    if (modItem.GetType() != item.GetType())
                    {
                        continue;
                    }

                    if (!item.HasBeenModified(modItem, true))
                    {
                        foundOneMatching = true;
                        break;
                    }
                }
                if (!foundOneMatching)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool HasBeenModified<T>(this T original, T vergleich, bool innerLoop = false)
        {
            if (original is null && vergleich is null)
            {
                return false;
            }

            if (original is null || vergleich is null)
            {
                return true;
            }

            // String Implementiert IEnumerable<char> daher prüfen wir string davor, damit wir nicht jedes Zeichen im Loop durchgehen müssen
            if (original is string && vergleich is string)
            {
                if (!original.Equals(vergleich))
                {
                    return true;
                }
            }
            else if (original is IEnumerable && vergleich is IEnumerable)
            {
                if (HasEnumerableChanged(original, vergleich))
                {
                    return true;
                }
            }
            else
            {
                foreach (PropertyInfo prop in original.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var attribute = prop.GetCustomAttribute<IgnoreModificationCheckAttribute>();
                    if (attribute != null)
                    {
                        continue;
                    }

                    object? originalValue = prop.GetValue(original);
                    object? vergleichValue = prop.GetValue(vergleich);

                    if (originalValue is string)
                    {
                        if (string.IsNullOrWhiteSpace(originalValue as string) && string.IsNullOrWhiteSpace(vergleichValue as string))
                        {
                            continue;
                        }
                        else
                        {
                            if (!originalValue.Equals(vergleichValue))
                            {
                                return true;
                            }
                        }
                    }

                    if (originalValue is null && vergleichValue is null)
                    {
                        continue;
                    }

                    if (originalValue is null || vergleichValue is null)
                    {
                        return true;
                    }

                    if (originalValue is IEnumerable && vergleichValue is IEnumerable)
                    {
                        if (HasEnumerableChanged(originalValue, vergleichValue))
                        {
                            return true;
                        }
                    }
                    else if (originalValue is IEnumerable || vergleichValue is IEnumerable)
                    {
                        return true;
                    }
                    else if (originalValue.GetType().IsEnum && vergleichValue.GetType().IsEnum)
                    {
                        if (!Equals(originalValue, vergleichValue))
                        {
                            return true;
                        }
                    }
                    else if (originalValue.GetType().IsEnum || vergleichValue.GetType().IsEnum)
                    {
                        return true;
                    }
                    else
                    {
                        MethodInfo? setter = prop.GetSetMethod(/*nonPublic*/ true);
                        if (setter is null)
                        {
                            continue;
                        }

                        if (prop.PropertyType.IsPrimitive || originalValue is string or decimal or float or double)
                        {
                            if (!originalValue.Equals(vergleichValue))
                            {
                                return true;
                            }
                        }
                        else if (originalValue is DateTime)
                        {
                            DateTime? originalDT = originalValue as DateTime?;
                            DateTime? vergleichDT = vergleichValue as DateTime?;
                            if (!Equals(originalDT, vergleichDT))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (prop.PropertyType == original.GetType())
                            {
                                continue;
                            }

                            if (originalValue.HasBeenModified(vergleichValue, true))
                            {
                                return true;
                            }
                        }
                    }

                }
            }
            return false;
        }
    }
}
