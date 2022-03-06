using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SWGEmuModManager.ViewModels
{
    public class Mod
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("bannerUrl")]
        public string? BannerUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("size")]
        public ulong? Size { get; set; }

        [JsonPropertyName("downloads")]
        public ulong? Downloads { get; set; }

        [JsonPropertyName("released")]
        public DateTime Released { get; set; }

        [JsonPropertyName("archive")]
        public string? Archive { get; set; }

        [JsonPropertyName("fileList")]
        public List<string>? FileList { get; set; }

        [JsonPropertyName("conflictList")]
        public List<int>? ConflictList { get; set; }
    }

    public class ModsDisplay
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("bannerUrl")]
        public string? BannerUrl { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("downloads")]
        public string? Downloads { get; set; }

        [JsonPropertyName("released")]
        public string? Released { get; set; }
    }

    internal class MainWindowViewModelProperties : ObservableObject
    {
        private List<ModsDisplay>? _modList;

        public List<ModsDisplay>? ModList
        {
            get => _modList;
            set => SetProperty(ref _modList, value);
        }
    }
}
