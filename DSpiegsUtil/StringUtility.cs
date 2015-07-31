using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSpiegsUtil
{
    public static class StringUtility
    {
        /// <summary>
        /// Removes the first occurence of a string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="stringToRemove"></param>
        /// <param name="startIndex">Index to start the search</param>
        /// <param name="count">Number of characters to from start to search</param>
        /// <returns>The original string without "stringToRemove" </returns>
        public static string Remove(this string s, string stringToRemove, int startIndex = 0, int? count = null)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }
            if (stringToRemove == null || !s.Contains(stringToRemove))
            {
                return s;
            }
            var index = s.IndexOf(stringToRemove, startIndex, count ?? s.Length - 1, StringComparison.Ordinal);
            return index >= 0 ? s.Remove(index, stringToRemove.Length) : s;
        }

        /// <summary>
        /// Removes the last occurence of a string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="stringToRemove"></param>
        /// <param name="startIndex">Index to start the search (number of chars from end)</param>
        /// <param name="count">Number of characters to from start to search (negative)</param>
        /// <returns>The original string without "stringToRemove" </returns>
        public static string RemoveLast(this string s, string stringToRemove, int? startIndex = null, int? count = null)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }
            if (stringToRemove == null || !s.Contains(stringToRemove))
            {
                return s;
            }
            var index = s.LastIndexOf(stringToRemove, startIndex ?? s.Length - 1, count ?? s.Length - 1, StringComparison.Ordinal);
            return index >= 0 ? s.Remove(index, stringToRemove.Length) : s;
        }

        public static string TrimOrEmpty(this string s)
        {
            return s == null ? string.Empty : s.Trim();
        }

        /// <summary>
        /// Replaces control characters with spaces as long as the previous character is not also a space
        /// </summary>
        /// <param name="s">String to change</param>
        /// <returns>String with control characters replaced with spaces</returns>
        public static string ReplaceControlChars(this string s)
        {
            if (s == null)
            {
                return string.Empty;
            }

            var outputChars = new List<char>();
            foreach (var c in s)
            {
                if (!char.IsControl(c))
                {
                    outputChars.Add(c);
                    continue;
                }

                if (!outputChars.Any())
                {
                    continue;
                }

                if (!char.IsWhiteSpace(outputChars.Last()))
                {
                    outputChars.Add(' ');
                }
            }

            s = new string(outputChars.ToArray());

            return s;
        }

        public static string HtmlEncode(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            return HtmlEncode(o.ToString().TrimOrEmpty());
        }
        public static string HtmlEncode(string s)
        {
            s = s ?? string.Empty;
            s = System.Web.HttpUtility.HtmlEncode(s);
            foreach (var mapping in HtmlCharacterMappings)
            {
                s = s.Replace(mapping.Key, mapping.Value);
            }
            return s;
        }

        public static void AppendLineNotEmpty(this StringBuilder sb, string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                sb.AppendLine(s);
            }
        }


        #region Special characters that System.Web.HttpUtility.HtmlEncode does not encode

        // http://eliasbland.wordpress.com/category/c/

        public static readonly Dictionary<string, string> HtmlCharacterMappings = new Dictionary<string, string>
        {
            {"–", "&#8211;"},
            {"—", "&#8212;"},
            {"‘", "&#8216;"},
            {"’", "&#8217;"},
            {"‚", "&#8218;"},
            {"“", "&#8220;"},
            {"”", "&#8221;"},
            {"„", "&#8222;"},
            {"†", "&#8224;"},
            {"‡", "&#8225;"},
            {"•", "&#8226;"},
            {"…", "&#8230;"},
            {"‰", "&#8240;"},
            {"€", "&#8364;"},
            {"™", "&#8482;"},
            {"ą", "&#261;"},
            {"Ą", "&#260;"},
            {"ę", "&#281;"},
            {"Ę", "&#280;"},
            {"Ā", "&#256;"},
            {"ā", "&#257;"},
            {"Ă", "&#258;"},
            {"ă", "&#259;"},
            {"Ǟ", "&#478;"},
            {"ǟ", "&#479;"},
            {"Ǻ", "&#506;"},
            {"ǻ", "&#507;"},
            {"Ǽ", "&#508;"},
            {"ǽ", "&#509;"},
            {"Ḃ", "&#7682;"},
            {"ḃ", "&#7683;"},
            {"Ć", "&#262;"},
            {"ć", "&#263;"},
            {"Č", "&#268;"},
            {"č", "&#269;"},
            {"Ĉ", "&#264;"},
            {"ĉ", "&#265;"},
            {"Ċ", "&#266;"},
            {"ċ", "&#267;"},
            {"Ḑ", "&#7696;"},
            {"ḑ", "&#7697;"},
            {"Ď", "&#270;"},
            {"ď", "&#271;"},
            {"Ḋ", "&#7690;"},
            {"ḋ", "&#7691;"},
            {"Đ", "&#272;"},
            {"đ", "&#273;"},
            {"Ǳ", "&#497;"},
            {"ǲ", "&#498;"},
            {"ǳ", "&#499;"},
            {"Ǆ", "&#452;"},
            {"ǅ", "&#453;"},
            {"ǆ", "&#454;"},
            {"Ě", "&#282;"},
            {"ě", "&#283;"},
            {"Ē", "&#274;"},
            {"ē", "&#275;"},
            {"Ĕ", "&#276;"},
            {"ĕ", "&#277;"},
            {"Ė", "&#278;"},
            {"ė", "&#279;"},
            {"Ʒ", "&#439;"},
            {"ʒ", "&#658;"},
            {"Ǯ", "&#494;"},
            {"ǯ", "&#495;"},
            {"Ḟ", "&#7710;"},
            {"ḟ", "&#7711;"},
            {"ƒ", "&#402;"},
            {"ﬀ", "&#64256;"},
            {"ﬁ", "&#64257;"},
            {"ﬂ", "&#64258;"},
            {"ﬃ", "&#64259;"},
            {"ﬄ", "&#64260;"},
            {"ﬅ", "&#64261;"},
            {"Ǵ", "&#500;"},
            {"ǵ", "&#501;"},
            {"Ģ", "&#290;"},
            {"ģ", "&#291;"},
            {"Ǧ", "&#486;"},
            {"ǧ", "&#487;"},
            {"Ĝ", "&#284;"},
            {"ĝ", "&#285;"},
            {"Ğ", "&#286;"},
            {"ğ", "&#287;"},
            {"Ġ", "&#288;"},
            {"ġ", "&#289;"},
            {"Ǥ", "&#484;"},
            {"ǥ", "&#485;"},
            {"Ĥ", "&#292;"},
            {"ĥ", "&#293;"},
            {"Ħ", "&#294;"},
            {"ħ", "&#295;"},
            {"Ĩ", "&#296;"},
            {"ĩ", "&#297;"},
            {"Ī", "&#298;"},
            {"ī", "&#299;"},
            {"Ĭ", "&#300;"},
            {"ĭ", "&#301;"},
            {"Į", "&#302;"},
            {"į", "&#303;"},
            {"İ", "&#304;"},
            {"ı", "&#305;"},
            {"Ĳ", "&#306;"},
            {"ĳ", "&#307;"},
            {"Ĵ", "&#308;"},
            {"ĵ", "&#309;"},
            {"Ḱ", "&#7728;"},
            {"ḱ", "&#7729;"},
            {"Ķ", "&#310;"},
            {"ķ", "&#311;"},
            {"Ǩ", "&#488;"},
            {"ǩ", "&#489;"},
            {"ĸ", "&#312;"},
            {"Ĺ", "&#313;"},
            {"ĺ", "&#314;"},
            {"Ļ", "&#315;"},
            {"ļ", "&#316;"},
            {"Ľ", "&#317;"},
            {"ľ", "&#318;"},
            {"Ŀ", "&#319;"},
            {"ŀ", "&#320;"},
            {"Ł", "&#321;"},
            {"ł", "&#322;"},
            {"Ǉ", "&#455;"},
            {"ǈ", "&#456;"},
            {"ǉ", "&#457;"},
            {"Ṁ", "&#7744;"},
            {"ṁ", "&#7745;"},
            {"Ń", "&#323;"},
            {"ń", "&#324;"},
            {"Ņ", "&#325;"},
            {"ņ", "&#326;"},
            {"Ň", "&#327;"},
            {"ň", "&#328;"},
            {"ŉ", "&#329;"},
            {"Ŋ", "&#330;"},
            {"ŋ", "&#331;"},
            {"Ǌ", "&#458;"},
            {"ǋ", "&#459;"},
            {"ǌ", "&#460;"},
            {"Ō", "&#332;"},
            {"ō", "&#333;"},
            {"Ŏ", "&#334;"},
            {"ŏ", "&#335;"},
            {"Ő", "&#336;"},
            {"ő", "&#337;"},
            {"Ǿ", "&#510;"},
            {"ǿ", "&#511;"},
            {"Œ", "&#338;"},
            {"œ", "&#339;"},
            {"Ṗ", "&#7766;"},
            {"ṗ", "&#7767;"},
            {"Ŕ", "&#340;"},
            {"ŕ", "&#341;"},
            {"Ŗ", "&#342;"},
            {"ŗ", "&#343;"},
            {"Ř", "&#344;"},
            {"ř", "&#345;"},
            {"ɼ", "&#636;"},
            {"Ś", "&#346;"},
            {"ś", "&#347;"},
            {"Ş", "&#350;"},
            {"ş", "&#351;"},
            {"Š", "&#352;"},
            {"š", "&#353;"},
            {"Ŝ", "&#348;"},
            {"ŝ", "&#349;"},
            {"Ṡ", "&#7776;"},
            {"ṡ", "&#7777;"},
            {"ſ", "&#383;"},
            {"Ţ", "&#354;"},
            {"ţ", "&#355;"},
            {"Ť", "&#356;"},
            {"ť", "&#357;"},
            {"Ṫ", "&#7786;"},
            {"ṫ", "&#7787;"},
            {"Ŧ", "&#358;"},
            {"ŧ", "&#359;"},
            {"Ũ", "&#360;"},
            {"ũ", "&#361;"},
            {"Ů", "&#366;"},
            {"ů", "&#367;"},
            {"Ū", "&#362;"},
            {"ū", "&#363;"},
            {"Ŭ", "&#364;"},
            {"ŭ", "&#365;"},
            {"Ų", "&#370;"},
            {"ų", "&#371;"},
            {"Ű", "&#368;"},
            {"ű", "&#369;"},
            {"Ẁ", "&#7808;"},
            {"ẁ", "&#7809;"},
            {"Ẃ", "&#7810;"},
            {"ẃ", "&#7811;"},
            {"Ŵ", "&#372;"},
            {"ŵ", "&#373;"},
            {"Ẅ", "&#7812;"},
            {"ẅ", "&#7813;"},
            {"Ỳ", "&#7922;"},
            {"ỳ", "&#7923;"},
            {"Ŷ", "&#374;"},
            {"ŷ", "&#375;"},
            {"Ÿ", "&#376;"},
            {"Ź", "&#377;"},
            {"ź", "&#378;"},
            {"Ž", "&#381;"},
            {"ž", "&#382;"},
            {"Ż", "&#379;"},
            {"ż", "&#380;"}
        };

        #endregion
    }
}
