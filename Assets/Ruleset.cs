using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
   
    public enum Ruleset
    {
        CRAWLERS = 1,
        ACCRETOR = 2,
        CLOUDS = 3,
        CRYSTALS = 4
    }
    public static class RulesetData
    {
       
        public static readonly Dictionary<Ruleset, string> Paths = new Dictionary<Ruleset, string>
        {
            { Ruleset.CRAWLERS, "Assets/ComputeStep.compute" },
            { Ruleset.ACCRETOR, "Assets/AccretorCompute.compute" },
            { Ruleset.CLOUDS, "Assets/CloudsCompute.compute" }
            
        };

        public static string GetPath(Ruleset ruleset)
        {
            return Paths.TryGetValue(ruleset, out var path) ? path : null;
        }
    }
}