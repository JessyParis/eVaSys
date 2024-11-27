/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 13/10/2021
/// ----------------------------------------------------------------------------------------------------- 
///
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";

//-----------------------------------------------------------------------------------
//Data models utils 

//-----------------------------------------------------------------------------------
//Get Adresse list text
export function getAdresseListText(adr: dataModelsInterfaces.Adresse): string {
  let r: string = "";
  if (adr.Libelle) { r += adr.Libelle + " - "; }
  if (adr.CodePostal) { r += adr.CodePostal + " "; }
  if (adr.Ville) { r += adr.Ville + " "; }
  if (adr.Pays) { r += "(" + adr.Pays.Libelle + ")"; }
  if (adr.AdresseType) { r += " - " + adr.AdresseType.Libelle + " "; }
  return r;
}
//-----------------------------------------------------------------------------------
//Get ContactAdresse list text
export function getContactAdresseListText(cA: dataModelsInterfaces.ContactAdresse): string {
  let r: string = "";
  if (cA.Contact.Nom) { r += cA.Contact.Nom; }
  if (cA.Contact.Prenom) { r += " " + cA.Contact.Prenom; }
  if (cA.TelMobile) { r += " - " + cA.TelMobile; }
  if (cA.Tel) { r += " - " + cA.Tel; }
  if (cA.Email) { r += " - " + cA.Email; }
  return r;
}
