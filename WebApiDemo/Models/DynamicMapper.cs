using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;

namespace WebApiDemo.Models
{
    public static class DynamicMapper
    {
        private static Dictionary<string, Type> baseEntityTypesByType = new Dictionary<string, Type>();
        private static string connectionString = ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;

        private static List<dynamic> ToDynamicList(IEnumerable ierable)
        {
            if (ierable == null) return null;

            var list = new List<dynamic>();
            foreach (dynamic item in ierable)
            {
                list.Add(item);
            }
            return list;
        }

        public static int SqlCommand(string sql,params object[] parameters)
        {
            using (DbContext context = new DbContext(connectionString))
            {
                Database database = context.Database;
                return database.ExecuteSqlCommand(sql, parameters);
            }
        }

        public static List<dynamic> DynamicSqlQuery(string sql, params object[] parameters)
        {
            Type resultType;
            using (DbContext context = new DbContext(connectionString))
            {
                Database database = context.Database;
                if (baseEntityTypesByType.TryGetValue(sql, out resultType))
                {
                    Console.WriteLine("Inside Got the Type");
                    if (parameters != null)
                        return ToDynamicList(database.SqlQuery(resultType, sql, parameters));
                    else
                        return ToDynamicList(database.SqlQuery(resultType, sql));
                }

                TypeBuilder builder = DynamicMapper.createTypeBuilder("MyDynamicAssembly", "MyDynamicModule", "MyDynamicType");
                using (System.Data.IDbCommand command = database.Connection.CreateCommand())
                {
                    try
                    {
                        database.Connection.Open();
                        command.CommandText = sql;
                        command.CommandTimeout = command.Connection.ConnectionTimeout;
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.Add(param);
                            }
                        }

                        using (System.Data.IDataReader reader = command.ExecuteReader())
                        {
                            var schema = reader.GetSchemaTable();
                            foreach (System.Data.DataRow row in schema.Rows)
                            {
                                string name = (string)row["ColumnName"];
                                Type type = (Type)row["DataType"];
                                var allowNull = (bool)row["AllowDBNull"];
                                if (allowNull)
                                {
                                    type = GetNullableType(type);
                                }
                                DynamicMapper.createAutoImplementedProperty(builder, name, type);
                            }
                        }
                    }
                    finally
                    {
                        database.Connection.Close();
                        command.Parameters.Clear();
                    }
                }

                resultType = builder.CreateType();
                baseEntityTypesByType[sql] = resultType;

                if (parameters != null)
                    return ToDynamicList(database.SqlQuery(resultType, sql, parameters));
                else
                    return ToDynamicList(database.SqlQuery(resultType, sql));
            }
        }

        private static TypeBuilder createTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain
                .CurrentDomain
                .DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName)
                .DefineType(typeName, TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void createAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
                string.Concat(PrivateFieldPrefix, propertyName),
                              propertyType, FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(GetterPrefix, propertyName),
                propertyMethodAttributes, propertyType, Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
                string.Concat(SetterPrefix, propertyName),
                propertyMethodAttributes, null, new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        public static Type GetNullableType(Type TypeToConvert)
        {
            // Abort if no type supplied
            if (TypeToConvert == null)
                return null;

            // If the given type is already nullable, just return it
            if (IsTypeNullable(TypeToConvert))
                return TypeToConvert;

            // If the type is a ValueType and is not System.Void, convert it to a Nullable<Type>
            if (TypeToConvert.IsValueType && TypeToConvert != typeof(void))
                return typeof(Nullable<>).MakeGenericType(TypeToConvert);

            // Done - no conversion
            return null;
        }

        public static bool IsTypeNullable(Type TypeToTest)
        {
            // Abort if no type supplied
            if (TypeToTest == null)
                return false;

            // If this is not a value type, it is a reference type, so it is automatically nullable
            //  (NOTE: All forms of Nullable<T> are value types)
            if (!TypeToTest.IsValueType)
                return true;

            // Report whether TypeToTest is a form of the Nullable<> type
            return TypeToTest.IsGenericType && TypeToTest.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}