﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
 
namespace Top2000_MVC.ViewModels
{
    public class ArtistViewModel
    {
        public int ArtiestId { get; set; }
        public string Naam { get; set; }
        public string? Wiki { get; set; }
        public string? Biografie { get; set; }
        public string? Foto { get; set; }
    }
}
