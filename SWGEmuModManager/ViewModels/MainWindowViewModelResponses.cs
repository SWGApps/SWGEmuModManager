﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SWGEmuModManager.ViewModels; 

public class MainWindowViewModelResponses : ObservableObject
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

        [JsonPropertyName("source")]
        public string? Source { get; set; }

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

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("size")]
        public string? Size { get; set; }

        [JsonPropertyName("downloads")]
        public string? Downloads { get; set; }

        [JsonPropertyName("released")]
        public string? Released { get; set; }
    }

    public class InstallRequestResponse
    {
        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("downloadUrl")]
        public string? DownloadUrl { get; set; }

        [JsonPropertyName("fileList")]
        public List<string>? FileList { get; set; }

        [JsonPropertyName("archive")]
        public string? Archive { get; set; }

        [JsonPropertyName("conflictList")]
        public List<int>? ConflictList { get; set; }
    }

    public class UninstallRequestResponse
    {
        [JsonPropertyName("result")]
        public string? Result { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("fileList")]
        public List<string>? FileList { get; set; }
    }
}