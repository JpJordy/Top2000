using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Top2000_MVC.Models
{
    public class AdminUserList
    {
        [JsonPropertyName("$values")]
        public List<AdminUserWithRole> Values { get; set; }
    }
}
