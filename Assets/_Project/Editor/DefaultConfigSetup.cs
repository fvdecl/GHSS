using UnityEditor;
using UnityEngine;
using GHSS.Core.Board;
using GHSS.Core.Game;
using GHSS.Core.Items;
using GHSS.Core.Spawners;
using GHSS.Core.Timers;

namespace GHSS.EditorTools
{
    /// <summary>
    /// One-click creation of the default balance config assets (board size,
    /// item chain, spawner chain with the 90/10 and 50/50 tables, timer,
    /// GameConfig). Editor-only, lives in a folder literally named "Editor" so
    /// it's excluded from player builds without needing its own asmdef.
    ///
    /// Uses SerializedObject/SerializedProperty (not reflection) to set the
    /// private serialized fields - the same mechanism the Inspector itself
    /// uses, so it plays nicely with Undo/dirty-tracking and doesn't require
    /// opening up the production configs' API just for tooling.
    ///
    /// Safe to re-run: existing assets at the target paths are reused, not
    /// duplicated, and values are simply re-applied.
    /// </summary>
    internal static class DefaultConfigSetup
    {
        private const string RootFolder = "Assets/_Project/Configs";
        private const string ItemsFolder = RootFolder + "/Items";
        private const string SpawnersFolder = RootFolder + "/Spawners";
        private const string TimersFolder = RootFolder + "/Timers";

        [MenuItem("GHSS/Setup/Create Default Config Assets")]
        private static void CreateDefaultConfigs()
        {
            var confirmed = EditorUtility.DisplayDialog(
                "Create default GHSS configs",
                "This creates BoardConfig, 4 ItemDefinitions + ItemChainConfig, " +
                "1 unmergeable bonus ItemDefinition (deliberately NOT part of the chain), " +
                "2 SpawnerDefinitions (90/10, 50/50, both with a 30% bonus-item chance) + " +
                "SpawnerChainConfig, TimerConfig, TimedSpawnerConfig and GameConfig under " +
                RootFolder + ".\n\n" +
                "Existing assets at those paths are reused, not duplicated - safe to re-run.",
                "Create", "Cancel");

            if (!confirmed) return;

            EnsureFolder(RootFolder);
            EnsureFolder(ItemsFolder);
            EnsureFolder(SpawnersFolder);
            EnsureFolder(TimersFolder);

            var board = CreateOrLoad<BoardConfig>($"{RootFolder}/BoardConfig.asset");
            SetBoard(board, width: 7, height: 9, cellSize: 1f, origin: Vector2.zero);

            var item1 = CreateOrLoad<ItemDefinition>($"{ItemsFolder}/ItemDefinition_Level1.asset");
            var item2 = CreateOrLoad<ItemDefinition>($"{ItemsFolder}/ItemDefinition_Level2.asset");
            var item3 = CreateOrLoad<ItemDefinition>($"{ItemsFolder}/ItemDefinition_Level3.asset");
            var item4 = CreateOrLoad<ItemDefinition>($"{ItemsFolder}/ItemDefinition_Level4.asset");
            SetItemLevel(item1, 1, new Color(0.30f, 0.80f, 0.30f)); // green
            SetItemLevel(item2, 2, new Color(0.25f, 0.55f, 0.95f)); // blue
            SetItemLevel(item3, 3, new Color(0.65f, 0.35f, 0.85f)); // purple
            SetItemLevel(item4, 4, new Color(0.95f, 0.65f, 0.15f)); // orange

            var itemChain = CreateOrLoad<ItemChainConfig>($"{ItemsFolder}/ItemChainConfig.asset");
            SetChainLevels(itemChain, new Object[] { item1, item2, item3, item4 });

            // Deliberately never added to itemChain above - that's what makes
            // MergeRules.CanMerge refuse it (chain-membership check), with no
            // gameplay-code changes needed anywhere in the merge pipeline.
            var unmergeableItem = CreateOrLoad<ItemDefinition>($"{ItemsFolder}/ItemDefinition_Unmergeable.asset");
            SetItemLevel(unmergeableItem, 99, new Color(0.75f, 0.15f, 0.15f)); // red

            var spawner1 = CreateOrLoad<SpawnerDefinition>($"{SpawnersFolder}/SpawnerDefinition_Level1.asset");
            var spawner2 = CreateOrLoad<SpawnerDefinition>($"{SpawnersFolder}/SpawnerDefinition_Level2.asset");
            SetSpawnerDefinition(spawner1, level: 1, color: new Color(0.55f, 0.55f, 0.55f), // gray
                table: new (int itemLevel, float weight)[] { (1, 90f), (2, 10f) });
            SetSpawnerDefinition(spawner2, level: 2, color: new Color(0.90f, 0.75f, 0.20f), // gold
                table: new (int itemLevel, float weight)[] { (1, 50f), (2, 50f) });

            // The 30% bonus chance lives here (editor tooling), never in gameplay code -
            // SpawnerActivationController only ever reads it from SpawnerDefinition.
            SetSpawnerBonusItem(spawner1, unmergeableItem, 0.3f);
            SetSpawnerBonusItem(spawner2, unmergeableItem, 0.3f);

            var spawnerChain = CreateOrLoad<SpawnerChainConfig>($"{SpawnersFolder}/SpawnerChainConfig.asset");
            SetChainLevels(spawnerChain, new Object[] { spawner1, spawner2 });

            var timer = CreateOrLoad<TimerConfig>($"{TimersFolder}/TimerConfig.asset");
            SetTimerDuration(timer, 10f);

            var timedSpawner = CreateOrLoad<TimedSpawnerConfig>($"{TimersFolder}/TimedSpawnerConfig.asset");
            SetTimedSpawner(timedSpawner, timer, spawnerLevel: 1);

            var gameConfig = CreateOrLoad<GameConfig>($"{RootFolder}/GameConfig.asset");
            SetGameConfig(gameConfig, board, itemChain, spawnerChain, timedSpawner);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = gameConfig;
            EditorGUIUtility.PingObject(gameConfig);

            EditorUtility.DisplayDialog(
                "Done",
                "Configs created under " + RootFolder + ".\n\n" +
                "Still needed by hand: sprites for each level, the Item and Spawner " +
                "prefabs, and assigning each prefab to its ItemDefinition/SpawnerDefinition " +
                "'Prefab' field (they're left empty - CreateAsset can't guess a prefab that " +
                "doesn't exist yet). This includes ItemDefinition_Unmergeable - it reuses the " +
                "same Item prefab as the regular levels, nothing special needed for it.",
                "OK");
        }

        private static T CreateOrLoad<T>(string path) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath)) return;

            var lastSlash = assetPath.LastIndexOf('/');
            var parent = assetPath.Substring(0, lastSlash);
            var folderName = assetPath.Substring(lastSlash + 1);

            if (!AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);

            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static void SetBoard(BoardConfig board, int width, int height, float cellSize, Vector2 origin)
        {
            var so = new SerializedObject(board);
            so.FindProperty("width").intValue = width;
            so.FindProperty("height").intValue = height;
            so.FindProperty("cellSize").floatValue = cellSize;
            so.FindProperty("origin").vector2Value = origin;
            Apply(so, board);
        }

        private static void SetItemLevel(ItemDefinition item, int level, Color color)
        {
            var so = new SerializedObject(item);
            so.FindProperty("level").intValue = level;
            so.FindProperty("color").colorValue = color;
            Apply(so, item);
        }

        private static void SetChainLevels(ScriptableObject chain, Object[] definitions)
        {
            var so = new SerializedObject(chain);
            var levelsProp = so.FindProperty("levels");
            levelsProp.arraySize = definitions.Length;

            for (var i = 0; i < definitions.Length; i++)
                levelsProp.GetArrayElementAtIndex(i).objectReferenceValue = definitions[i];

            Apply(so, chain);
        }

        private static void SetSpawnerDefinition(SpawnerDefinition spawner, int level, Color color, (int itemLevel, float weight)[] table)
        {
            var so = new SerializedObject(spawner);
            so.FindProperty("level").intValue = level;
            so.FindProperty("color").colorValue = color;

            var tableProp = so.FindProperty("spawnTable");
            tableProp.arraySize = table.Length;

            for (var i = 0; i < table.Length; i++)
            {
                var element = tableProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("itemLevel").intValue = table[i].itemLevel;
                element.FindPropertyRelative("weight").floatValue = table[i].weight;
            }

            Apply(so, spawner);
        }

        private static void SetSpawnerBonusItem(SpawnerDefinition spawner, ItemDefinition bonusItem, float chance)
        {
            var so = new SerializedObject(spawner);
            so.FindProperty("unmergeableItem").objectReferenceValue = bonusItem;
            so.FindProperty("unmergeableItemChance").floatValue = chance;
            Apply(so, spawner);
        }

        private static void SetTimerDuration(TimerConfig timer, float durationSeconds)
        {
            var so = new SerializedObject(timer);
            so.FindProperty("durationSeconds").floatValue = durationSeconds;
            Apply(so, timer);
        }

        private static void SetTimedSpawner(TimedSpawnerConfig config, TimerConfig timer, int spawnerLevel)
        {
            var so = new SerializedObject(config);
            so.FindProperty("timer").objectReferenceValue = timer;
            so.FindProperty("spawnerLevel").intValue = spawnerLevel;
            Apply(so, config);
        }

        private static void SetGameConfig(
            GameConfig config, BoardConfig board, ItemChainConfig items, SpawnerChainConfig spawners, TimedSpawnerConfig timedSpawner)
        {
            var so = new SerializedObject(config);
            so.FindProperty("board").objectReferenceValue = board;
            so.FindProperty("items").objectReferenceValue = items;
            so.FindProperty("spawners").objectReferenceValue = spawners;
            so.FindProperty("timedSpawner").objectReferenceValue = timedSpawner;
            Apply(so, config);
        }

        private static void Apply(SerializedObject serializedObject, Object target)
        {
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
