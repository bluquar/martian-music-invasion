using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVersion
{
    public enum T
    {
        Integrated,
        NotIntegrated
    }

    public static bool ValidID(string name)
    {
        return Map.ContainsKey(name);
    }

    public static T GetVersion(string name)
    {
        if (Map.ContainsKey(name))
        {
            switch (Map[name].Trim())
            {
                case "N": return T.NotIntegrated;
                case "I": return T.Integrated;
                default:
                    Debug.Log("Bad version: " + Map[name].Trim());
                    return T.Integrated;
            }
        } else
        {
            Debug.Log("Bad name: " + name);
            return T.Integrated;
        }


    }

    #region Map
    private static Dictionary<string, string> Map = new Dictionary<string, string>()
    {
        // Class Name  Class Size  Grade
        // Class A 18  2nd
        // Class B 16  2nd
        // Class C 16  2nd
        // Class D 19  2nd
        // Class E 21  3rd
        // Class F 20  3rd
        // Class G 20  3rd

        // Class total: 18	2nd Grade
        // CLASS A
        #region Class A
        {"red pig", "N"},
        {"yellow frog", "N"},
        {"pink fish", "  N"},
        {"orange bear", "N"},
        {"gold bunny", " N"},
        {"orange frog", "N"},
        {"yellow goat", "N"},
        {"red fish", "   N"},
        {"gold dog", "   N"},
        {"white bird", " I"},
        {"purple goat", "I"},
        {"blue lion", "  I"},
        {"green bird", " I"},
        {"black horse", "I"},
        {"purple bear", "I"},
        {"black frog", " I"},
        {"green tiger", "I"},
        {"white lion", " I"},
        {"silver fish", "N"},
        {"silver goat", "I"},
        #endregion /* Class A */

        // Class total: 16  2nd Grade
        // CLASS B
        #region Class B
        {"red cow", "N"},
        {"yellow dog", " N"},
        {"pink tiger", " N"},
        {"gold cat", "   N"},
        {"orange goat", "N"},
        {"pink dog", "   N"},
        {"red cat", "N"},
        {"orange fish", "N"},
        {"white horse", "I"},
        {"black bird", " I"},
        {"purple bunny", "   I"},
        {"blue cow", "   I"},
        {"green lion", " I"},
        {"green pig", "  I"},
        {"purple fish", "I"},
        {"white goat", " I"},
        {"silver bear", "N"},
        {"silver tiger", "   I"},
        #endregion Class B

        // Class total: 16 2nd Grade
        // CLASS C
        #region Class C
        {"red bunny", "  N"},
        {"yellow fish", "N"},
        {"orange horse", "   N"},
        {"gold pig", "   N"},
        {"gold bear", "  N"},
        {"pink lion", "  N"},
        {"red tiger", "  N"},
        {"yellow bunny", "   N"},
        {"purple horse", "   I"},
        {"white bear", " I"},
        {"black goat", " I"},
        {"purple frog", "I"},
        {"blue bird", "  I"},
        {"green horse", "I"},
        {"blue goat", "  I"},
        {"black bear", " I"},
        {"silver lion", "N"},
        {"silver bird", "I"},
        #endregion Class C

        // Class total: 19 2nd Grade
        // CLASS D
        #region Class D
        {"red horse", "  N"},
        {"yellow bird", "N"},
        {"orange bird", "N"},
        {"gold cow", "   N"},
        {"red bear", "   N"},
        {"yellow cow", " N"},
        {"pink goat", "  N"},
        {"gold fish", "  N"},
        {"orange cat", " N"},
        {"purple cow", " I"},
        {"black bunny", "I"},
        {"white frog", " I"},
        {"blue tiger", " I"},
        {"blue cat", "   I"},
        {"green dog", "  I"},
        {"purple cat", " I"},
        {"black lion", " I"},
        {"white fish", " I"},
        {"purple tiger", "   I"},
        {"silver bunny", "   N"},
        {"siler frog", " I"},
        #endregion /* Class D */

        // Class total: 21 3rd Grade
        // CLASS E
        #region Class E
        {"orange dog", " N"},
        {"yellow bear", "N"},
        {"red goat", "   N"},
        {"yellow cat", " N"},
        {"gold bird", "  N"},
        {"gold tiger", " N"},
        {"pink frog", "  N"},
        {"red frog", "   N"},
        {"pink cow", "   N"},
        {"orange tiger", "   N"},
        {"orange pig", " N"},
        {"black cat", "  I"},
        {"white tiger", "I"},
        {"purple dog", " I"},
        {"blue horse", " I"},
        {"green frog", " I"},
        {"green goat", " I"},
        {"black pig", "  I"},
        {"white cat", "  I"},
        {"purple pig", " I"},
        {"blue bear", "  I"},
        {"silver cow", " N"},
        {"silver horse", "   I"},
        #endregion /* Class E */

        // Class total: 20 3rd Grade
        // CLASS F
        #region Class F
        {"red dog", "N"},
        {"yellow pig", " N"},
        {"gold goat", "  N"},
        {"pink bear", "  N"},
        {"yellow horse", "   N"},
        {"red lion", "   N"},
        {"yellow tiger", "   N"},
        {"pink bird", "  N"},
        {"gold frog", "  N"},
        {"orange bunny", "   N"},
        {"black dog", "  I"},
        {"white bunny", "I"},
        {"black tiger", "I"},
        {"purple lion", "I"},
        {"green fish", " I"},
        {"blue frog", "  I"},
        {"white pig", "  I"},
        {"blue bunny", " I"},
        {"green cow", "  I"},
        {"black cow", "  I"},
        {"silver pig", " N"},
        {"silver cat", " I"},
#endregion /* Class F */

        // Class total: 20 3rd Grade
        // CLASS G
        #region Class G
        {"red bird", "   N"},
        {"yellow lion", "N"},
        {"pink bunny", " N"},
        {"gold lion", "  N"},
        {"pink cat", "   N"},
        {"pink horse", " N"},
        {"gold horse", " N"},
        {"pink pig", "   N"},
        {"orange cow", " N"},
        {"orange lion", "N"},
        {"black fish", " I"},
        {"white dog", "  I"},
        {"purple bird", "I"},
        {"blue fish", "  I"},
        {"green bear", " I"},
        {"white cow", "  I"},
        {"blue dog", "   I"},
        {"blue pig", "   I"},
        {"green cat", "  I"},
        {"green bunny", "I"},
        {"silver moose", "   N"},
        {"silver snake", "   I"},
#endregion /* Class G */
    };
    #endregion /* Map */

}
