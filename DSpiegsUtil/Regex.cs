using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DSpiegsUtil
{
    public static class RegexExtensions
    {
        public static bool ContainsCustomPlaceholders(string template, out Regex customPlaceholdersRegex, string placeholderStart = "['",
           string placeholderEnd = "']")
        {
            customPlaceholdersRegex = null;
            const string matchAny = ".*?";
            const string matchAnyMoreThanOne = ".+?";

            //this creates a regular expression to match on placeholders with any text in between
            var placeholderRegex =
                new Regex(String.Format("{0}{1}{2}", Regex.Escape(placeholderStart), matchAnyMoreThanOne, Regex.Escape(placeholderEnd)));

            //creates string array with empty strings where placeholders were
            var templateChunks = placeholderRegex.Split(template);
            if (templateChunks.Length <= 1)
            {
                return false;
            }

            //escapes the non placeholder parts of the string with escaped string for exact match
            //puts strings to match any characters in between   
            //skip last because it has different reqs
            var newRegexPieces = new List<string>();
            for (int i = 0; i < templateChunks.Length - 1; i++)
            {
                var current = templateChunks[i];

                //sequestial matches insert empty strings
                if (string.IsNullOrWhiteSpace(current))
                {
                    continue;
                }

                newRegexPieces.Add(Regex.Escape(current));
                newRegexPieces.Add(matchAny);
            }

            //regex matches at the end are empty and need to be replaced with a pattern
            var last = templateChunks[templateChunks.Length - 1];
            newRegexPieces.Add(string.IsNullOrWhiteSpace(last) ? matchAny : Regex.Escape(last));

            //combines chunks back together for full patern
            var newRegexPattern = String.Join(String.Empty, newRegexPieces);

            //creates regex for new pattern
            customPlaceholdersRegex = new Regex(newRegexPattern);
            return true;
        }
    }
}
