using System.Reflection;
using System.Text;

namespace Lession7
{

    class TestClass
    {
        public int I { get; set; }
        public string? S { get; set; }
        public decimal D { get; set; }
        public char[]? C { get; set; }

        public TestClass() { }
        private TestClass(int i) 
        {
            this.I = i;
        }
        public TestClass(int i, string s, decimal d, char[] c) : this(i)
        {
            this.S = s;
            this.D = d;
            this.C = c;

        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var testClass = new TestClass();
            Console.WriteLine(typeof(TestClass));
            ex1();
        }

        private static void ex1()
        {
            TestClass tc = new TestClass();
            TestClass tc1 = new TestClass();
            TestClass tc2 = new TestClass();
            var type = typeof(TestClass);
            type.InvokeMember("TestClass", 
                BindingFlags.Instance | 
                BindingFlags.Public | 
                BindingFlags.CreateInstance, 
                null, tc, new Object[] { });
            type.InvokeMember("TestClass",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.CreateInstance,
                null, tc1, new Object[] {1});
            type.InvokeMember("TestClass",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.CreateInstance,
                null, tc2, new Object[] {1, "1", 1.0M, new char[] { '1', '2', '3' } });
            Console.WriteLine("Hello, World!");
        }


        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
        public class CustomNameAttribute : Attribute
        {
            public string CustomFieldName { get; }

            public CustomNameAttribute(string customFieldName)
            {
                CustomFieldName = customFieldName;
            }
        }
        public static string ObjectToString(object obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var type = obj.GetType();
            stringBuilder.Append(type.ToString() + "\n");
            stringBuilder.Append(type.Assembly + "\n");
            stringBuilder.Append(type.Name + "\n");
            var properties = type.GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var item in properties)
            {
                var value = item.GetValue(obj);

                stringBuilder.Append(GetPropertyName(item) + ":");
                if (item.PropertyType == typeof(char[]))
                {
                    stringBuilder.Append(new String(value as char[]) + "\n");
                }
                else
                {
                    stringBuilder.Append(value + "\n");
                }
            }

            return stringBuilder.ToString();
        }
        private static string GetPropertyName(PropertyInfo property)
        {
            var customNameAttribute = (CustomNameAttribute)Attribute.GetCustomAttribute(property, typeof(CustomNameAttribute));
            return customNameAttribute != null ? customNameAttribute.CustomFieldName : property.Name;
        }
        public static object StringToObject(string endString)
        {
            string[] str = endString.Split("\n");
            var typeName = str[2];
            var assemblyName = str[1];
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName);
            if (assembly != null)
            {
                var type = assembly.GetTypes().FirstOrDefault(t => t.FullName == typeName);
                if (type != null)
                {
                    var obj = Activator.CreateInstance(type);
                    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 3; i < str.Length - 1; i++)
                    {
                        var propertyString = str[i].Split(":");
                        var propertyName = propertyString[0];
                        var propertyValue = propertyString[1];
                        var property = properties.FirstOrDefault(p => GetPropertyName(p) == propertyName.Trim());  // Используем вспомогательный метод для получения имени поля
                        if (property != null)
                        {
                            if (property.PropertyType == typeof(int))
                            {
                                property.SetValue(obj, int.Parse(propertyValue.Trim()));
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(obj, propertyValue.Trim());
                            }

                        }
                    }
                    return obj;
                }
            }
            return null;
        }
    }

}