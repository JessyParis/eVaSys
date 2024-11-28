/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 22/01/2019
/// ----------------------------------------------------------------------------------------------------- 
///
//--------------------------------------------------------------------------------------------------------

import { UntypedFormControl, UntypedFormGroup } from "@angular/forms";
import { FormControlType, ListContentType } from "../globals/enums";

//Class for managing app modules
export class EnvModule {
  name: string;
  refRessource: number;
  culturedCaption: string="";
  cmt: string;
}
//--------------------------------------------------------------------------------------------------------
///Class for managing app components
export class EnvComponent {
  name: string;
  ref: string;
  url: string;
  formClass: string;
  ressBeforeDel: number;
  ressAfterDel: number;
  ressLibel: number;
  libelle: string;
  moments: string;
  getId0: boolean;
  createModifTooltipVisible: boolean = false;
  hasHelp: boolean = false;
}
//--------------------------------------------------------------------------------------------------------
//Class for managing app menus
export class EnvMenu {
  name: string;
  refRessource: number;
  culturedCaption: string;
  moduleName: string;
  cmt: string;
  gridField: string;
  gridSort: string;
  gridGoto: string;
  gridRef: string;
}
//--------------------------------------------------------------------------------------------------------
//Class for managing data columns
export class EnvDataColumn {
  name: string;
  refRessource: number;
  culturedCaption: string;
  field: string;
  fullField: string;
  cmt: string;
  dataType: string;
}
//--------------------------------------------------------------------------------------------------------
//Class for managing CommandeFournisseur statuts
export class EnvCommandeFournisseurStatut {
  name: string;
  culturedCaption: string;
}
//--------------------------------------------------------------------------------------------------------
//Class for managing component controls
export class EnvComponentControl {
  field: string;
  FC: UntypedFormControl;
  type: FormControlType;
  class = "";
  required = false;
  comparer : Function;
  maxlength = 50;
  style = "height: 90px;";
  listContent: ListContentType;
  listItems: object[];    // Contains all the items that are available in the selection list (this is filled automatically by ancestor component)
  listDesc: string;       // Option description (used field)
  listStore = ".";        // Value to store on the source object. For the item itself, use ".", otherwise specify a field of the item
  listRef: string;        // Field containing the item reference
  ress: number;
  public constructor(init?: Partial<EnvComponentControl>) {
    Object.assign(this, init);
  }
}
//--------------------------------------------------------------------------------------------------------
//Class for managing boxes on a form
export class EnvBox {
  ressName: number;
  boxClass: string = "boxNormalSize";
  FG: UntypedFormGroup;
  Controls: EnvComponentControl[] = [];
  HTML: string = "";
}

