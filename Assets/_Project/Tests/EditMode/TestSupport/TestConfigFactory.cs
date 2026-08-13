using System;
using System.Reflection;
using UnityEngine;
using GHSS.Core.Items;
using GHSS.Core.Spawners;

namespace GHSS.Tests.EditMode.TestSupport
{
    /// <summary>
    /// Builds ScriptableObject configs for tests via reflection. Production
    /// configs intentionally have no public setters (designer edits them in the
    /// Inspector, gameplay code only reads them) - reflection here keeps that
    /// guarantee intact instead of opening a back door in production code just
    /// for tests.
    /// </summary>
    internal static class TestConfigFactory
    {
        public static ItemDefinition ItemDefinition(int level)
        {
            var definition = ScriptableObject.CreateInstance<ItemDefinition>();
            SetField(definition, "level", level);
            return definition;
        }

        public static ItemChainConfig ItemChain(params ItemDefinition[] levels)
        {
            var chain = ScriptableObject.CreateInstance<ItemChainConfig>();
            SetField(chain, "levels", levels);
            return chain;
        }

        public static SpawnerDefinition SpawnerDefinition(int level, params SpawnWeight[] spawnTable)
        {
            var definition = ScriptableObject.CreateInstance<SpawnerDefinition>();
            SetField(definition, "level", level);
            SetField(definition, "spawnTable", spawnTable);
            return definition;
        }

        public static SpawnerChainConfig SpawnerChain(params SpawnerDefinition[] levels)
        {
            var chain = ScriptableObject.CreateInstance<SpawnerChainConfig>();
            SetField(chain, "levels", levels);
            return chain;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            while (type != null)
            {
                var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }

                type = type.BaseType;
            }

            throw new MissingFieldException($"Field '{fieldName}' not found on {target.GetType()} or its base types.");
        }
    }
}
