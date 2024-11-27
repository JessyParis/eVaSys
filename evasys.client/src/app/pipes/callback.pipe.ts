/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 15/11/2021
/// ----------------------------------------------------------------------------------------------------- 
///
import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "callback",
    pure: false,
    standalone: false
})
//--------------------------------------------------------------------------------------------
/// <summary>
/// Generic pipe using acallback function
/// </summary>
export class CallbackPipe implements PipeTransform {
  transform(items: any[]
    , callback: (item: any, filterGlobalActif: boolean, refEntiteType: number, visibiliteTotale: boolean)
      => boolean, filterGlobalActif, refEntiteType, visibiliteTotale): any {
    if (!items || !callback) {
      return items;
    }
    return items.filter(item => callback(item, filterGlobalActif, refEntiteType, visibiliteTotale));
  }
}
