﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Girvel.Graph;
using Isometric.Core.Modules.WorldModule.Buildings;
using Isometric.Editor.Extensions;
using Microsoft.Win32;

namespace Isometric.Editor
{
    public partial class MainWindow
    {
        protected BuildingPattern SelectedPattern;
        protected int SelectedPatternListIndex = -1;

        protected readonly Control[] BuildingSettingsControls;

        protected SaveFileDialog SaveFileDialog
            = new SaveFileDialog();

        protected OpenFileDialog OpenFileDialog
            = new OpenFileDialog();

        private const string DefaultFilter 
            = "Isometric buildings|*.isob|All files|*.*";

        private Dictionary<BuildingPattern, GraphNode<BuildingPattern>> _nodes
            = new Dictionary<BuildingPattern, GraphNode<BuildingPattern>>(); 



        public MainWindow()
        {
            InitializeComponent();
            
            GameData.Instance = new GameData();

            foreach (var type in typeof (BuildingType).GetEnumNames())
            {
                BuildingTypeComboBox.Items.Add(type);
            }

            SaveFileDialog.Filter = DefaultFilter;
            OpenFileDialog.Filter = DefaultFilter;

            BuildingSettingsControls = new Control[]
            {
                IdLabel,
                NameLabel,
                PriceLabel,
                ResourcesLabel,
                TypeLabel,
                IdTextBox,
                NameTextBox,
                PriceTextBox,
                ResourcesTextBox,
                BuildingTypeComboBox,
            };

            ResetBuildingSelection();
        }



        public void AddBuilding(BuildingPattern pattern)
        {
            BuildingsListBox.Items.Add(pattern.Name);

            _nodes[pattern] = GameData.Instance.BuildingGraph.FirstOrDefault(node => node.Value.Name == pattern.Name)
                ?? GameData.Instance.BuildingGraph.NewNode(pattern);
        }

        public void SelectBuilding(string name, int listIndex)
        {
            SelectedPattern = GameData.Instance.BuildingPatterns.First(p => p.Name == name);
            SelectedPatternListIndex = listIndex;

            foreach (var control in BuildingSettingsControls)
            {
                control.IsEnabled = true;
            }

            if (SelectedPattern == null)
            {
                MessageBox.Show(
                    "I can't find this building. I'm sorry. " +
                    "Please, report a bug (email: widauka@ya.ru).");

                return;
            }

            NameTextBox.Text = SelectedPattern.Name;
            IdTextBox.Text = SelectedPattern.Id.ToString();
            BuildingTypeComboBox.SelectedItem = SelectedPattern.Type.ToString();
            ResourcesTextBox.Text = SelectedPattern.Resources.GetValueString();
            PriceTextBox.Text = SelectedPattern.Price.GetValueString();

            UpgradesComboBox.Items.Clear();
            UpgradesListBox.Items.Clear();

            foreach (
                var obj 
                    in BuildingsListBox.Items
                        .Cast<object>()
                        .Where(item =>
                            item.ToString() != SelectedPattern.Name
                            && _nodes[SelectedPattern]
                                .GetChildren()
                                .All(node => node.Value.Name != item.ToString())))
            {
                UpgradesComboBox.Items.Add(obj);
            }

            foreach (var child in _nodes[SelectedPattern].GetChildren())
            {
                UpgradesListBox.Items.Add(child.Value.Name);
            }
        }

        public void ResetBuildingSelection()
        {
            SelectedPatternListIndex = -1;
            SelectedPattern = null;

            foreach (var control in BuildingSettingsControls)
            {
                control.IsEnabled = false;
            }
        }

        public void AddUpgrade(string name)
        {
            _nodes[SelectedPattern].AddChild(
                _nodes.First(pair => pair.Value.Value.Name == UpgradesComboBox.Text).Value);

            UpgradesListBox.Items.Add(name);
            UpgradesComboBox.Items.Remove(name);
        }



        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddBuilding(
                GameData.Instance.BuildingPatterns.NewPattern(
                    $"building{BuildingsListBox.Items.Count}"));
        }

        private void BuildingsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            SelectBuilding(BuildingsListBox.SelectedItem.ToString(), BuildingsListBox.SelectedIndex);
        }

        private void NameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedPattern == null || SelectedPatternListIndex == -1)
            {
                return;
            }

            SelectedPattern.Name = NameTextBox.Text;
            BuildingsListBox.Items[SelectedPatternListIndex] = NameTextBox.Text;
        }

        private void BuildingTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedPattern.Type = 
                (BuildingType) Enum.Parse(
                    typeof (BuildingType), 
                    BuildingTypeComboBox.SelectedItem.ToString());
        }

        private void ResourcesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedPattern == null)
            {
                return;
            }

            SelectedPattern.Resources = ResourcesTextBox.GameResources;
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SelectedPattern == null)
            {
                return;
            }

            SelectedPattern.Price = PriceTextBox.GameResources;
        }

        private void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog.OverwritePrompt = true;
            SaveFileDialog.ShowDialog(this);

            try
            {
                using (var stream = SaveFileDialog.OpenFile())
                {
                    GameData.Instance.SerializeData(stream);
                }
            }
            catch (InvalidOperationException) { }

            SaveFileDialog.Reset();
        }

        private void OpenMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog.ShowDialog(this);

            try
            {
                using (var stream = OpenFileDialog.OpenFile())
                {
                    BuildingsListBox.Items.Clear();
                    GameData.Instance = new GameData(stream);
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }

            OpenFileDialog.Reset();
            foreach (var pattern in GameData.Instance.BuildingPatterns)
            {
                AddBuilding(pattern);
            }
        }

        private void UpgradesAddButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (UpgradesComboBox.SelectedIndex != -1)
            {
                AddUpgrade(UpgradesComboBox.SelectedItem.ToString());
            }
        }
    }
}
