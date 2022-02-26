using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KPBSD.PowerShell.WindowsUpdate
{
    /// <summary>
    /// Base class for COM object models.
    /// </summary>
    public abstract class Model
    {
        public static object CreateModel(object comObject)
        {
            var pso = PSObject.AsPSObject(comObject);
            var match = Regex.Match(pso.TypeNames[0], @"^System.__ComObject#{(?<guid>.*)}$");
            if (match.Success)
            {
                var guid = Guid.Parse(match.Groups["guid"].Value);
                var type = Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => 
                {
                    var attr = t.GetCustomAttributes<ModelForGuidAttribute>();
                    return attr.Any(a => a.Guid == guid);
                }).SingleOrDefault();
                if (type == null)
                {
                    throw new ItemNotFoundException($"Could not find a type model for Guid {guid}.");
                }
                else
                {
                    return Activator.CreateInstance(
                        type: type,
                        bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                        binder: null,
                        args: new object[] { comObject },
                        culture: System.Globalization.CultureInfo.CurrentCulture
                        );
                }
            }
            else
            {
                throw new FormatException($"Could not parse Guid for object with type name {pso.TypeNames[0]}.");
            }
        }
        protected static T[] ToModelArray<T>(dynamic collection)
        {
            var array = new T[collection.Count];
            for(int i = 0 ; i < array.Length; i++)
            {
                array[i] = CreateModel(collection[i]);
            }
            return array;
        }
        protected static string[] ToStringArray(dynamic istringcoll)
        {
            var array = new string[istringcoll.Count];
            for(int i = 0 ; i < array.Length; i++)
            {
                array[i] = istringcoll[i];
            }
            return array;
        }
        internal Model()
        {

        }
    }
}