using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Database
{
    public static class DBDataTypesHelper
    {
        public static Dictionary<string, string> CLRTypes = new Dictionary<string, string>
        {
            {"bit", "bool"},
            {"smallint", "short"},
            {"tinyint", "short"},
            {"int", "int"},
            {"bigint", "long"},
            {"smallmoney", "decimal"},
            {"money", "decimal"},
            {"decimal", "decimal"},
            {"numeric", "decimal"},
            {"real", "single"},
            {"float", "double"},
            {"char", "string"},
            {"nchar", "string"},
            {"varchar", "string"},
            {"nvarchar", "string"},
            {"text", "string"},
            {"ntext", "string"},
            {"smalldatetime", "DateTime"},
            {"datetime", "DateTime"},
            {"datetime2", "DateTime"},
            {"datetimeoffset", "DateTimeOffset"},
            {"date", "DateTime"},
            {"time", "TimeSpan"},
            {"uniqueidentifier", "Guid"},
            {"binary", "Byte[]"},
            {"varbinary", "Byte[]"},
            {"image", "Byte[]"},
            {"timestamp", "Byte[]"},
        };

        public static Type GetCLRType(string sqlType)
        {
            var clrTypeString = GetCLRTypeString(sqlType);
            return GetTypeFromSimpleName(clrTypeString);
        }

        public static string GetCLRTypeString(string sqlType, bool nullable = false)
        {
            string clrType;
            int parentIndex = sqlType.IndexOf('(');
            if (parentIndex != -1)
            {
                sqlType = sqlType.Remove(parentIndex);
            }
            if (CLRTypes.TryGetValue(sqlType, out clrType))
            {
                if (nullable && clrType != "object" && clrType != "string" && clrType != "Byte[]")
                {
                    return clrType + "?";
                }
                return clrType;
            }
            return "object";
        }

        public static string GetTypeName(Type type)
        {
            string typeName;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeName = type.GetGenericArguments()[0].Name + "?";
            }
            else
            {
                typeName = type.Name;
            }
            return typeName.Replace("System.", "")
                .Replace("Int64", "long")
                .Replace("Int32", "int")
                .Replace("Int16", "short")
                .Replace("Boolean", "bool")
                .ToLower()
                .Replace("datetime", "DateTime")
                .Replace("timespan", "TimeSpan")
                .Replace("datetimeoffset", "DateTimeOffset")
                .Replace("guid", "Guid")
                .Replace("byte", "Byte");
        }


        //http://stackoverflow.com/questions/721870/how-can-i-get-generic-type-from-string-representation
        public static Type GetTypeFromSimpleName(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            bool isArray = false, isNullable = false;

            if (typeName.IndexOf("[]") != -1)
            {
                isArray = true;
                typeName = typeName.Remove(typeName.IndexOf("[]"), 2);
            }

            if (typeName.IndexOf("?") != -1)
            {
                isNullable = true;
                typeName = typeName.Remove(typeName.IndexOf("?"), 1);
            }

            typeName = typeName.ToLower();

            string parsedTypeName = null;
            switch (typeName)
            {
                case "bool":
                case "boolean":
                    parsedTypeName = "System.Boolean";
                    break;
                case "byte":
                    parsedTypeName = "System.Byte";
                    break;
                case "char":
                    parsedTypeName = "System.Char";
                    break;
                case "datetime":
                    parsedTypeName = "System.DateTime";
                    break;
                case "datetimeoffset":
                    parsedTypeName = "System.DateTimeOffset";
                    break;
                case "decimal":
                    parsedTypeName = "System.Decimal";
                    break;
                case "double":
                    parsedTypeName = "System.Double";
                    break;
                case "float":
                    parsedTypeName = "System.Single";
                    break;
                case "int16":
                case "short":
                    parsedTypeName = "System.Int16";
                    break;
                case "int32":
                case "int":
                    parsedTypeName = "System.Int32";
                    break;
                case "int64":
                case "long":
                    parsedTypeName = "System.Int64";
                    break;
                case "object":
                    parsedTypeName = "System.Object";
                    break;
                case "sbyte":
                    parsedTypeName = "System.SByte";
                    break;
                case "string":
                    parsedTypeName = "System.String";
                    break;
                case "timespan":
                    parsedTypeName = "System.TimeSpan";
                    break;
                case "uint16":
                case "ushort":
                    parsedTypeName = "System.UInt16";
                    break;
                case "uint32":
                case "uint":
                    parsedTypeName = "System.UInt32";
                    break;
                case "uint64":
                case "ulong":
                    parsedTypeName = "System.UInt64";
                    break;
                case "guid":
                    parsedTypeName = "System.Guid";
                    break;
            }

            if (parsedTypeName != null)
            {
                if (isArray)
                    parsedTypeName = parsedTypeName + "[]";

                if (isNullable)
                    parsedTypeName = String.Concat("System.Nullable`1[", parsedTypeName, "]");
            }
            else
                parsedTypeName = typeName;

            // Expected to throw an exception in case the type has not been recognized.
            return Type.GetType(parsedTypeName);
        }
    }
}
