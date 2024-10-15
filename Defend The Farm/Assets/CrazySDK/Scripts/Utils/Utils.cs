using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using UnityEngine;

namespace CrazyGames
{
    public class Utils
    {
        public static string ToJson<T>(T[] array)
        {
            var wrapper = new JsonWrapper<T>();
            wrapper.Items = array;
            return FixJson(JsonUtility.ToJson(wrapper));
        }

        private static string FixJson(string value)
        {
            return value.Substring(9, value.Length - 10);
        }

        [Serializable]
        private class JsonWrapper<T>
        {
            public T[] Items;
        }

        public static string AppendQueryParameters(string baseUrl, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return baseUrl;

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var kvp in parameters)
            {
                query[kvp.Key] = kvp.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public static string ConvertDictionaryToJson(Dictionary<string, string> dictionary)
        {
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            var count = 0;
            foreach (var kvp in dictionary)
            {
                count++;
                jsonBuilder.AppendFormat("\"{0}\":\"{1}\"", EscapeString(kvp.Key), EscapeString(kvp.Value));

                if (count < dictionary.Count)
                {
                    jsonBuilder.Append(",");
                }
            }

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public static string EscapeString(string input)
        {
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}