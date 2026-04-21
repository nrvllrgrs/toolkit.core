using UnityEngine;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ToolkitEditor
{
	public class LocalizationAuditor : EditorWindow
	{
		private List<UnusedEntry> unusedEntries = new List<UnusedEntry>();
		private bool useGrep = true;
		private string grepPath = @"C:\Tools\ripgrep\rg.exe";

		private ListView resultsListView;
		private Label statusLabel;
		private Label resultsCountLabel;
		private Button scanButton;
		private Button removeButton;

		[MenuItem("Window/Asset Management/Localization Auditor")]
		public static void ShowWindow()
		{
			var window = GetWindow<LocalizationAuditor>("Localization Auditor");
			window.minSize = new Vector2(600, 400);
		}

		private void CreateGUI()
		{
			// Load USS
			var styleSheet = AssetUtil.LoadFirstAsset<StyleSheet>("LocalizationAuditor");
			if (styleSheet != null)
			{
				rootVisualElement.styleSheets.Add(styleSheet);
			}

			// Create main container
			var root = rootVisualElement;
			root.AddToClassList("root-container");

			// Header
			var header = new VisualElement();
			header.AddToClassList("header");

			var title = new Label("Localization Auditor");
			title.AddToClassList("title");
			header.Add(title);

			var subtitle = new Label("Find and remove unused localized strings");
			subtitle.AddToClassList("subtitle");
			header.Add(subtitle);

			root.Add(header);

			// Settings Section
			var settingsSection = CreateSettingsSection();
			root.Add(settingsSection);

			// Action Buttons
			var actionButtons = new VisualElement();
			actionButtons.AddToClassList("action-buttons");

			scanButton = new Button(ScanForUnusedStrings) { text = "Scan for Unused Strings" };
			scanButton.AddToClassList("scan-button");
			actionButtons.Add(scanButton);

			root.Add(actionButtons);

			// Status Label
			statusLabel = new Label("Ready to scan");
			statusLabel.AddToClassList("status-label");
			root.Add(statusLabel);

			// Results Section
			var resultsSection = CreateResultsSection();
			root.Add(resultsSection);
		}

		private VisualElement CreateSettingsSection()
		{
			var section = new VisualElement();
			section.AddToClassList("settings-section");

			var sectionTitle = new Label("Settings");
			sectionTitle.AddToClassList("section-title");
			section.Add(sectionTitle);

			var settingsContainer = new VisualElement();
			settingsContainer.AddToClassList("settings-container");

			// Use Grep Toggle
			var grepToggle = new Toggle("Use Grep Tool (faster)") { value = useGrep };
			grepToggle.AddToClassList("setting-toggle");
			grepToggle.RegisterValueChangedCallback(evt =>
			{
				useGrep = evt.newValue;
				UpdateGrepPathVisibility();
			});
			settingsContainer.Add(grepToggle);

			// Grep Path Field
			var grepPathContainer = new VisualElement();
			grepPathContainer.AddToClassList("grep-path-container");
			grepPathContainer.name = "grep-path-container";

			var grepPathField = new TextField("Grep Path") { value = grepPath };
			grepPathField.AddToClassList("grep-path-field");
			grepPathField.RegisterValueChangedCallback(evt => grepPath = evt.newValue);
			grepPathContainer.Add(grepPathField);

			var browseButton = new Button(() =>
			{
				string path = EditorUtility.OpenFilePanel("Select grep executable", "", "exe");
				if (!string.IsNullOrEmpty(path))
				{
					grepPath = path;
					grepPathField.value = path;
				}
			})
			{ text = "Browse" };
			browseButton.AddToClassList("browse-button");
			grepPathContainer.Add(browseButton);

			settingsContainer.Add(grepPathContainer);

			// Help Box
			var helpBox = new HelpBox("Recommended: ripgrep (rg.exe) - Download from github.com/BurntSushi/ripgrep", HelpBoxMessageType.Info);
			helpBox.AddToClassList("help-box");
			settingsContainer.Add(helpBox);

			section.Add(settingsContainer);

			UpdateGrepPathVisibility();

			return section;
		}

		private VisualElement CreateResultsSection()
		{
			var section = new VisualElement();
			section.AddToClassList("results-section");

			// Results Header
			var resultsHeader = new VisualElement();
			resultsHeader.AddToClassList("results-header");

			resultsCountLabel = new Label("No results");
			resultsCountLabel.AddToClassList("results-count");
			resultsHeader.Add(resultsCountLabel);

			removeButton = new Button(RemoveUnusedEntries) { text = "Remove All Unused" };
			removeButton.AddToClassList("remove-button");
			removeButton.SetEnabled(false);
			resultsHeader.Add(removeButton);

			section.Add(resultsHeader);

			// Results ListView
			resultsListView = new ListView();
			resultsListView.AddToClassList("results-list");
			resultsListView.makeItem = MakeResultItem;
			resultsListView.bindItem = BindResultItem;
			resultsListView.itemsSource = unusedEntries;
			resultsListView.selectionType = SelectionType.Multiple;
			resultsListView.style.flexGrow = 1;

			section.Add(resultsListView);

			return section;
		}

		private VisualElement MakeResultItem()
		{
			var container = new VisualElement();
			container.AddToClassList("result-item");

			var infoContainer = new VisualElement();
			infoContainer.AddToClassList("result-info");

			var tableLabel = new Label();
			tableLabel.name = "table-label";
			tableLabel.AddToClassList("table-label");
			infoContainer.Add(tableLabel);

			var keyLabel = new Label();
			keyLabel.name = "key-label";
			keyLabel.AddToClassList("key-label");
			infoContainer.Add(keyLabel);

			container.Add(infoContainer);

			var selectButton = new Button();
			selectButton.name = "select-button";
			selectButton.text = "Select";
			selectButton.AddToClassList("select-button");
			container.Add(selectButton);

			return container;
		}

		private void BindResultItem(VisualElement element, int index)
		{
			if (index >= unusedEntries.Count) return;

			var entry = unusedEntries[index];

			var tableLabel = element.Q<Label>("table-label");
			tableLabel.text = entry.tableName;

			var keyLabel = element.Q<Label>("key-label");
			keyLabel.text = entry.key;

			var selectButton = element.Q<Button>("select-button");
			selectButton.clicked += () => Selection.activeObject = entry.table;
		}

		private void UpdateGrepPathVisibility()
		{
			var grepPathContainer = rootVisualElement.Q("grep-path-container");
			if (grepPathContainer != null)
			{
				grepPathContainer.style.display = useGrep ? DisplayStyle.Flex : DisplayStyle.None;
			}
		}

		private void ScanForUnusedStrings()
		{
			var stopwatch = Stopwatch.StartNew();
			unusedEntries.Clear();

			scanButton.SetEnabled(false);
			statusLabel.text = "Scanning...";

			// Get all keys from String Tables (now includes both key strings and KeyIds)
			var allKeys = GetAllLocalizationKeys();
			statusLabel.text = $"Found {allKeys.Count} total keys. Searching project...";

			// Get StringTable asset paths to exclude
			var excludedPaths = GetStringTableAssetPaths();

			// Find used keys via grep (searches for both key strings AND KeyIds)
			HashSet<string> usedKeys;
			if (useGrep && System.IO.File.Exists(grepPath))
			{
				usedKeys = ScanWithGrep(allKeys, excludedPaths);
			}
			else
			{
				usedKeys = ScanWithUnity(allKeys, excludedPaths);
			}

			UnityEngine.Debug.Log($"Found {usedKeys.Count} used keys out of {allKeys.Count} total keys");

			// Find unused entries
			foreach (var kvp in allKeys)
			{
				if (!usedKeys.Contains(kvp.Key))
				{
					unusedEntries.Add(kvp.Value);
				}
			}

			stopwatch.Stop();

			// Update UI
			resultsListView.itemsSource = unusedEntries;
			resultsListView.Rebuild();

			resultsCountLabel.text = unusedEntries.Count > 0
				? $"Found {unusedEntries.Count} unused entries"
				: "No unused entries found";

			statusLabel.text = $"Scan complete in {stopwatch.ElapsedMilliseconds}ms";

			removeButton.SetEnabled(unusedEntries.Count > 0);
			scanButton.SetEnabled(true);

			UnityEngine.Debug.Log($"Scan complete in {stopwatch.ElapsedMilliseconds}ms. Found {unusedEntries.Count} unused entries.");
		}

		private Dictionary<string, UnusedEntry> GetAllLocalizationKeys()
		{
			var allKeys = new Dictionary<string, UnusedEntry>();
			var stringTableCollections = LocalizationEditorSettings.GetStringTableCollections();

			foreach (var collection in stringTableCollections)
			{
				foreach (var entry in collection.SharedData.Entries)
				{
					var unusedEntry = new UnusedEntry
					{
						tableName = collection.TableCollectionName,
						key = entry.Key,
						keyId = entry.Id,
						table = collection
					};

					// Store with the string key as the lookup
					allKeys[entry.Key] = unusedEntry;
				}
			}

			return allKeys;
		}

		private HashSet<string> GetStringTableAssetPaths()
		{
			var stringTablePaths = new HashSet<string>();
			var stringTableCollections = LocalizationEditorSettings.GetStringTableCollections();

			foreach (var collection in stringTableCollections)
			{
				// Add the shared data asset path
				string sharedPath = AssetDatabase.GetAssetPath(collection.SharedData);
				if (!string.IsNullOrEmpty(sharedPath))
				{
					// Store both relative and absolute paths for comparison
					stringTablePaths.Add(sharedPath);
					stringTablePaths.Add(System.IO.Path.GetFullPath(sharedPath));
				}

				// Add all table asset paths
				foreach (var table in collection.StringTables)
				{
					string tablePath = AssetDatabase.GetAssetPath(table);
					if (!string.IsNullOrEmpty(tablePath))
					{
						stringTablePaths.Add(tablePath);
						stringTablePaths.Add(System.IO.Path.GetFullPath(tablePath));
					}
				}
			}

			UnityEngine.Debug.Log($"Excluding {stringTablePaths.Count / 2} StringTable asset files from search");
			foreach (var path in stringTablePaths)
			{
				if (!path.Contains(":")) // Log only relative paths for readability
				{
					UnityEngine.Debug.Log($"  Excluding: {path}");
				}
			}
			return stringTablePaths;
		}

		private HashSet<string> ScanWithGrep(Dictionary<string, UnusedEntry> allKeys, HashSet<string> excludedPaths)
		{
			var usedKeys = new HashSet<string>();
			var projectPath = Application.dataPath;

			// Get list of keys AND KeyIds to search for
			var searchPatterns = new List<string>();
			foreach (var kvp in allKeys)
			{
				searchPatterns.Add(kvp.Key); // The string key
				searchPatterns.Add(kvp.Value.keyId.ToString()); // The numeric KeyId
			}

			UnityEngine.Debug.Log($"Starting ripgrep scan for {allKeys.Count} keys ({searchPatterns.Count} patterns including KeyIds) in {projectPath}");

			// Process in batches to avoid command line length limits
			int batchSize = 100;
			int totalBatches = (searchPatterns.Count + batchSize - 1) / batchSize;

			for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
			{
				var batchPatterns = searchPatterns.Skip(batchIndex * batchSize).Take(batchSize).ToList();
				statusLabel.text = $"Searching batch {batchIndex + 1}/{totalBatches} ({batchPatterns.Count} patterns)...";

				string tempPatternFile = System.IO.Path.GetTempFileName();

				try
				{
					// Write patterns to temp file - use fixed strings, not regex
					System.IO.File.WriteAllLines(tempPatternFile, batchPatterns);

					ProcessStartInfo startInfo = new ProcessStartInfo
					{
						FileName = grepPath,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true,
						WorkingDirectory = projectPath
					};

					// Only search relevant file types (we'll filter out StringTable assets when reading results)
					// Show filenames with matches (not match content)
					startInfo.Arguments = $"-F --files-with-matches --no-heading -f \"{tempPatternFile}\" " +
										 $"-g \"*.cs\" -g \"*.prefab\" -g \"*.unity\" -g \"*.asset\" " +
										 $"\"{projectPath}\"";

					UnityEngine.Debug.Log($"Running: {startInfo.FileName} {startInfo.Arguments}");

					using (Process process = Process.Start(startInfo))
					{
						string output = process.StandardOutput.ReadToEnd();
						string error = process.StandardError.ReadToEnd();
						process.WaitForExit();

						if (!string.IsNullOrEmpty(error))
						{
							UnityEngine.Debug.LogWarning($"ripgrep stderr: {error}");
						}

						// ripgrep with --files-with-matches returns filenames that contain matches
						var matchedFiles = output.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

						UnityEngine.Debug.Log($"Batch {batchIndex + 1}: Found {matchedFiles.Length} files with potential matches");

						// Now check which specific keys are in these files (excluding StringTable assets)
						foreach (var filePath in matchedFiles)
						{
							// Normalize path for comparison (try both relative and absolute)
							string normalizedPath = filePath.Replace('\\', '/');
							string fullPath = System.IO.Path.GetFullPath(filePath);

							// Skip if this is an excluded StringTable asset
							if (excludedPaths.Contains(normalizedPath) || excludedPaths.Contains(fullPath) || excludedPaths.Contains(filePath))
							{
								UnityEngine.Debug.Log($"Skipping excluded StringTable file: {System.IO.Path.GetFileName(filePath)}");
								continue;
							}

							try
							{
								string fileContent = System.IO.File.ReadAllText(filePath);

								// Check both the key string AND the KeyId
								foreach (var kvp in allKeys)
								{
									if (usedKeys.Contains(kvp.Key))
										continue; // Already found

									// Check if either the key string OR KeyId exists in file
									if (fileContent.Contains(kvp.Key) || fileContent.Contains(kvp.Value.keyId.ToString()))
									{
										if (usedKeys.Add(kvp.Key))
										{
											string foundBy = fileContent.Contains(kvp.Key) ? "key" : "KeyId";
											UnityEngine.Debug.Log($"Found '{kvp.Key}' (by {foundBy}) in {System.IO.Path.GetFileName(filePath)}");
										}
									}
								}
							}
							catch (System.Exception ex)
							{
								UnityEngine.Debug.LogWarning($"Could not read file {filePath}: {ex.Message}");
							}
						}
					}
				}
				catch (System.Exception ex)
				{
					UnityEngine.Debug.LogError($"Error running ripgrep: {ex.Message}");
					statusLabel.text = "Ripgrep error, falling back to Unity scan...";

					// Fall back to Unity scan
					var fallbackUsed = ScanWithUnity(allKeys, excludedPaths);
					foreach (var key in fallbackUsed)
					{
						usedKeys.Add(key);
					}
					break;
				}
				finally
				{
					if (System.IO.File.Exists(tempPatternFile))
					{
						System.IO.File.Delete(tempPatternFile);
					}
				}
			}

			return usedKeys;
		}

		private HashSet<string> ScanWithUnity(Dictionary<string, UnusedEntry> allKeys, HashSet<string> excludedPaths)
		{
			var usedKeys = new HashSet<string>();

			// Get all relevant assets, EXCLUDING StringTable assets
			var allAssets = AssetDatabase.GetAllAssetPaths()
				.Where(path => path.StartsWith("Assets/") &&
					   (path.EndsWith(".cs") || path.EndsWith(".prefab") ||
						path.EndsWith(".unity") || path.EndsWith(".asset")) &&
					   !excludedPaths.Contains(path))
				.ToList();

			int current = 0;
			int total = allAssets.Count;

			UnityEngine.Debug.Log($"Unity scan: Checking {total} files for {allKeys.Count} keys (excluding {excludedPaths.Count} StringTable assets)");

			foreach (var assetPath in allAssets)
			{
				current++;
				if (current % 50 == 0)
				{
					statusLabel.text = $"Scanning: {current}/{total} files...";
				}

				try
				{
					string content = System.IO.File.ReadAllText(assetPath);

					foreach (var kvp in allKeys)
					{
						if (usedKeys.Contains(kvp.Key))
							continue; // Already found

						// Check if either the key string OR KeyId exists in file
						if (content.Contains(kvp.Key) || content.Contains(kvp.Value.keyId.ToString()))
						{
							usedKeys.Add(kvp.Key);
							string foundBy = content.Contains(kvp.Key) ? "key" : "KeyId";
							UnityEngine.Debug.Log($"Found '{kvp.Key}' (by {foundBy}) in {assetPath}");
						}
					}

					// Early exit if all keys found
					if (usedKeys.Count == allKeys.Count)
					{
						UnityEngine.Debug.Log("All keys found, stopping scan early");
						break;
					}
				}
				catch (System.Exception ex)
				{
					UnityEngine.Debug.LogWarning($"Could not read {assetPath}: {ex.Message}");
				}
			}

			return usedKeys;
		}

		private void RemoveUnusedEntries()
		{
			if (!EditorUtility.DisplayDialog("Confirm Deletion",
				$"Are you sure you want to remove {unusedEntries.Count} unused entries? This cannot be undone.",
				"Yes, Remove", "Cancel"))
			{
				return;
			}

			foreach (var entry in unusedEntries)
			{
				var sharedData = entry.table.SharedData;
				sharedData.RemoveKey(entry.keyId);
				EditorUtility.SetDirty(sharedData);
			}

			AssetDatabase.SaveAssets();

			statusLabel.text = $"Removed {unusedEntries.Count} unused entries";
			unusedEntries.Clear();
			resultsListView.itemsSource = unusedEntries;
			resultsListView.Rebuild();
			resultsCountLabel.text = "No results";
			removeButton.SetEnabled(false);

			UnityEngine.Debug.Log("Unused entries removed.");
		}

		private class UnusedEntry
		{
			public string tableName;
			public string key;
			public long keyId;
			public StringTableCollection table;
		}
	}
}