namespace Extensions.DataSource {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;

    public static class DataSourceCreator {
        private static readonly Regex PropertNameRegex =
            new Regex(@"^[A-Za-z]+[A-Za-z0-9_]*$", RegexOptions.Singleline);

        private static readonly Dictionary<string, Type> TypeBySigniture =
            new Dictionary<string, Type>();


        public static IEnumerable ToDataSource(this IEnumerable<IDictionary> list) {
            if (list == null || !list.Any()) {
                return null;
            }

            IDictionary firstDict = null;
            var hasData = false;
            foreach (var currentDict in list) {
                hasData = true;
                firstDict = currentDict;
                break;
            }
            if (!hasData) {
                return new object[] { };
            }
            if (firstDict == null) {
                throw new ArgumentException("IDictionary entry cannot be null");
            }

            var typeSigniture = GetTypeSigniture(firstDict, list);

            var objectType = GetTypeByTypeSigniture(typeSigniture);

            if (objectType != null) return GenerateEnumerable(objectType, list, firstDict);
            var tb = GetTypeBuilder(typeSigniture);

            var constructor =
                tb.DefineDefaultConstructor(
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.RTSpecialName);


            foreach (DictionaryEntry pair in firstDict) {
                if (PropertNameRegex.IsMatch(Convert.ToString(pair.Key), 0)) {
                    CreateProperty(tb,
                        Convert.ToString(pair.Key),
                        GetValueType(pair.Value, list, pair.Key));
                }
                else {
                    throw new ArgumentException(
                        @"Each key of IDictionary must be 
                                alphanumeric and start with character.");
                }
            }
            objectType = tb.CreateTypeInfo();

            TypeBySigniture.Add(typeSigniture, objectType);

            return GenerateEnumerable(objectType, list, firstDict);
        }



        private static Type GetTypeByTypeSigniture(string typeSigniture) {
            Type type;
            return TypeBySigniture.TryGetValue(typeSigniture, out type) ? type : null;
        }

        private static Type GetValueType(object value, IEnumerable<IDictionary> list, object key) {
            if (value != null) return value?.GetType() ?? typeof(object);
            foreach (var dictionary in list) {
                if (!dictionary.Contains(key)) continue;
                value = dictionary[key];
                if (value != null) break;
            }
            return value?.GetType() ?? typeof(object);
        }

        private static string GetTypeSigniture(IDictionary firstDict, IEnumerable<IDictionary> list) {
            var sb = new StringBuilder();
            foreach (DictionaryEntry pair in firstDict) {
                sb.AppendFormat("_{0}_{1}", pair.Key, GetValueType(pair.Value, list, pair.Key));
            }
            return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
        }

        private static IEnumerable GenerateEnumerable(
            Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict) {
            var listType = typeof(List<>).MakeGenericType(new[] { objectType });
            var listOfCustom = Activator.CreateInstance(listType);

            foreach (var currentDict in list) {
                if (currentDict == null) {
                    throw new ArgumentException("IDictionary entry cannot be null");
                }
                var row = Activator.CreateInstance(objectType);
                foreach (DictionaryEntry pair in firstDict) {
                    if (!currentDict.Contains(pair.Key)) continue;
                    var property =
                        objectType.GetProperty(Convert.ToString(pair.Key));

                    var value = currentDict[pair.Key];
                    if (value != null &&
                        value.GetType() != property.PropertyType &&
                        !property.PropertyType.IsGenericType) {
                        try {
                            value = Convert.ChangeType(
                                currentDict[pair.Key],
                                property.PropertyType,
                                null);
                        }
                        catch { }
                    }

                    property.SetValue(row, value, null);
                }
                listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
            }
            return listOfCustom as IEnumerable;
        }

        private static TypeBuilder GetTypeBuilder(string typeSigniture) {
            var an = new AssemblyName("TempAssembly" + typeSigniture);
            var assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(
                    an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            var tb = moduleBuilder.DefineType("TempType" + typeSigniture
                , TypeAttributes.Public |
                  TypeAttributes.Class |
                  TypeAttributes.AutoClass |
                  TypeAttributes.AnsiClass |
                  TypeAttributes.BeforeFieldInit |
                  TypeAttributes.AutoLayout
                , typeof(object));
            return tb;
        }

        private static void CreateProperty(
            TypeBuilder tb, string propertyName, Type propertyType) {
            if (propertyType.IsValueType && !propertyType.IsGenericType) {
                propertyType = typeof(Nullable<>).MakeGenericType(new[] { propertyType });
            }

            var fieldBuilder = tb.DefineField("_" + propertyName,
                propertyType,
                FieldAttributes.Private);


            var propertyBuilder =
                tb.DefineProperty(
                    propertyName, PropertyAttributes.HasDefault, propertyType, null);
            var getPropMthdBldr =
                tb.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    propertyType, Type.EmptyTypes);

            var getIL = getPropMthdBldr.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            var setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new Type[] { propertyType });

            var setIl = setPropMthdBldr.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}