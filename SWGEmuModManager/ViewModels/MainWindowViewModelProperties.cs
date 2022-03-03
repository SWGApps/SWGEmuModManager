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
        public DateTime? Released { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }

        [JsonPropertyName("fileList")]
        public List<string>? FileList { get; set; }

        [JsonPropertyName("conflictList")]
        public List<Mod>? ConflictList { get; set; }
    }

    internal class MainWindowViewModelProperties : ObservableObject
    {
    }
}
