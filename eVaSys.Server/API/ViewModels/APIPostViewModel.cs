/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 23/06/2022
/// ----------------------------------------------------------------------------------------------------- 
using Newtonsoft.Json;
using System;

namespace eVaSys.ViewModels
{
    [JsonObject(MemberSerialization.OptOut)]
    public class APIPostViewModel
    {
        public APIPostViewModel()
        {
        }
        public string Version { get; set; }
        public string AppId { get; set; }
        public string APIKey { get; set; }
        public string CRUD { get; set; }
        public APIObjectViewModel Object { get; set; }
        public int PageLength { get; set; }
        public string PageNumber { get; set; }
        public APIFilterViewModel[] Filters { get; set; }
        public APIOrderViewModel[] Orders { get; set; }
        public APIActionViewModel[] Actions { get; set; }
        public APIMiscViewModel[] Miscs { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class APIObjectViewModel
    {
        public APIObjectViewModel()
        {
        }
        public string Name { get; set; }
        public string Id { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class APIOrderViewModel
    {
        public APIOrderViewModel()
        {
        }
        public string Field { get; set; }
        public bool Asc { get; set; } = true;
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class APIActionViewModel
    {
        public APIActionViewModel()
        {
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class APIFilterViewModel
    {
        public APIFilterViewModel()
        {
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    [JsonObject(MemberSerialization.OptOut)]
    public class APIMiscViewModel
    {
        public APIMiscViewModel()
        {
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}