/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 26/10/2022
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class APICommandeFournisseurRefExtViewModel
    {
        public APICommandeFournisseurRefExtViewModel()
        {
        }
        [Required]
        [StringLength(50)] 
        public string RefExt { get; set; }
        [Required]
        [StringLength(50)]
        public string ActionCode { get; set; }
    }
}