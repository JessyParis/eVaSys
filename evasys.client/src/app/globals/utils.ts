import { MatDialog } from "@angular/material/dialog";
import { InformationComponent } from "../components/dialogs/information/information.component";
import { ApplicationUserContext } from "./globals";
import { CommandeFournisseur, ContactAdresse, ContactAdresseServiceFonction, Contrat, ContratCollectivite, EntiteType } from "../interfaces/dataModelsInterfaces";
import moment from "moment";
import { EventEmitterService } from "../services/event-emitter.service";
import { AppLink, MatTableColumn } from '../interfaces/appInterfaces';
import { EnvDataColumn } from '../classes/appClasses';
import { DataType, MenuName, DataColumnName } from './enums';
import { ConfirmComponent } from "../components/dialogs/confirm/confirm.component";
import { Observable } from "rxjs";
import { DataModelService } from "../services/data-model.service";
import { FormArray } from "@angular/forms";
import { SeriesPoint } from "@progress/kendo-angular-charts";

/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 11/09/2018
/// ----------------------------------------------------------------------------------------------------- 
///


//----------------------------------------------------------------------------------------------------- 
//General utils for managing cookies in Typescript.
export function setCookie(name: string, val: string) {
  const date = new Date();
  const value = val;

  // Set it expire in 7 days
  date.setTime(date.getTime() + (7 * 24 * 60 * 60 * 1000));

  // Set it
  document.cookie = name + "=" + value + "; expires=" + date.toUTCString() + "; path=/";
}

export function getCookie(name: string) {
  const value = "; " + document.cookie;
  const parts = value.split("; " + name + "=");

  if (parts.length === 2) {
    return parts.pop().split(";").shift();
  }
  else {
    return "";
  }
}

export function deleteCookie(name: string) {
  const date = new Date();

  // Set it expire in -10 days
  date.setTime(date.getTime() + (-10 * 24 * 60 * 60 * 1000));

  // Set it
  document.cookie = name + "=; expires=" + date.toUTCString() + "; path=/";
}

export function delay(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

export function b64DecodeUnicode(str) {
  return decodeURIComponent(Array.prototype.map.call(atob(str), function (c) {
    return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2)
  }).join(""))
}

//----------------------------------------------------------------------------------------------------- 
//Create router navigation properties from literal link
export function createRouterNavigationProperties(pN: string): AppLink {
  let appLink: AppLink = {
    valid: false
  } as AppLink
  //Process
  if (pN.startsWith("/*appLink=")) {
    pN = pN.substring(10);
    try {
      //Process component/id
      let cpn = pN.split(";")[0];
      let cpnName = cpn.split("/")[0];
      let cpnRef = Number.parseInt(cpn.split("/")[1], 10);
      if (isNaN(cpnRef)) { return appLink; }
      else {
        appLink.valid = true;
        appLink.componentName = cpnName;
        appLink.componentRef = cpnRef;
      }
    }
    catch { return appLink; }
    //Process navigation extra
    if (pN.split(";").length > 1)
      try {
        let navExtra = pN.split(";")[1];
        let navExtraObj;
        switch (navExtra.split("=")[0]) {
          case "elementType":
            navExtraObj = { elementType: (navExtra.split("=")[1]) };
            appLink.navExtra = navExtraObj;
            break;
        }
      }
      catch {
        appLink.valid = false;
        return appLink;
      }
  }
  //End
  return appLink;
}

//----------------------------------------------------------------------------------------------------- 
//Get attachment file name. decode UTF8 filename if needed
export function getAttachmentFilename(str) {
  let fileName = str.split(";")[1].trim().split("filename=")[1];
  fileName = decodeURIComponent(fileName);
  //Remove "
  let re = /\"/gi;
  fileName = fileName.replace(re, "");
  re = /\+/gi;
  fileName = fileName.replace(re, " ");
  return fileName;
}

//-----------------------------------------------------------------------------------------------------
//Open information dialog
export function showInformationToUser(dialog: MatDialog, title: string, message: string, applicationUserContext: ApplicationUserContext) {
  if (title == "") { title = applicationUserContext.getCulturedRessourceText(337); }
  dialog.open(InformationComponent, {
    width: "350px",
    data: {
      title: title
      , message: message
      , htmlMessage: ""
    },
    restoreFocus: false
  });
}

//-----------------------------------------------------------------------------------------------------
//Open information dialog
export function showHtmlInformationToUser(dialog: MatDialog, title: string, htmlMessage: string, applicationUserContext: ApplicationUserContext) {
  if (title == "") { title = applicationUserContext.getCulturedRessourceText(337); }
  dialog.open(InformationComponent, {
    width: "350px",
    data: {
      title: title
      , message: ""
      , htmlMessage: htmlMessage
    },
    restoreFocus: false
  });
}

//-----------------------------------------------------------------------------------------------------
//Open error dialog
export function showErrorToUser(dialog: MatDialog, error: any, applicationUserContext: ApplicationUserContext) {
  const dialogRef = dialog.open(InformationComponent, {
    width: "350px",
    data: {
      title: (applicationUserContext.getCulturedRessourceText(319) == "---???---" ? "Error" : applicationUserContext.getCulturedRessourceText(319))
      , message: error.error?.Message ? error.error.Message : (error.message ? error.message : applicationUserContext.getCulturedRessourceText(712))
    },
    restoreFocus: false
  });
}

//-----------------------------------------------------------------------------------------------------
//Open confirmation dialog and subscribe to closing
export function showConfirmToUser(dialog: MatDialog, title: string, message: string, applicationUserContext: ApplicationUserContext):Observable<any> {
  if (title == "") { title = applicationUserContext.getCulturedRessourceText(300); }
  const dialogRef = dialog.open(ConfirmComponent, {
    width: "350px",
    data: {
      title: title
      , message: message
    },
    restoreFocus: false
  });
  return dialogRef.afterClosed();
}

//-----------------------------------------------------------------------------------------------------
//Generic comparison function
export const cmp = function (x, y) {
  return x > y ? 1 : x < y ? -1 : 0;
};

//-----------------------------------------------------------------------------------------------------
//Generic comparison function descending
export const cmpDesc = function (x, y) {
  return x < y ? 1 : x > y ? -1 : 0;
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Get formatted NumeroCommande
export function getFormattedNumeroCommande(s: number): string {
  let r: string = "";
  if (s) {
    r = s.toString();
    if (r.length === 10) {
      r = r.substring(0, 4) + " " + r.substring(4, 6) + " " + r.substring(6, 10);
    }
  }
  return r;
}

//----------------------------------------------------------------------------------------------------- 
//Create tooltip creation/modification text
export function getCreationModificationTooltipText<T extends { CreationText: string; ModificationText: string }>(item: T): string {
  let t: string = "";
  if (item?.CreationText) { t = item?.CreationText; }
  if (item?.CreationText && item?.ModificationText) { t += "\n"; }
  if (item?.ModificationText) { t += item?.ModificationText; }
  return t;
}
//----------------------------------------------------------------------------------------------------- 
//Create tooltip certification text
export function getCertficationTooltipText<T extends { CertificationText: string }>(item: T): string {
  let t: string = "";
  if (item?.CertificationText) { t = item?.CertificationText; }
  return t;
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Get cultured month name
export function getCulturedMonthName(n: number, applicationUserContext: ApplicationUserContext): string {
  let r: string = "";
  if (n >= 1 && n <= 12) {
    r = applicationUserContext.getCulturedRessourceText(446 + n);
  }
  return r;
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Globalization
export function changeCulture(cultureName:string, applicationUserContext: ApplicationUserContext) {
  applicationUserContext.currentCultureName = cultureName;
  applicationUserContext.initCulturedRessources();
  applicationUserContext.initEnvModules();
  applicationUserContext.initEnvMenus();
  applicationUserContext.initEnvDataColumns();
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Get EnvCommandeFournisseurStatut
export function getEnvCommandeFournisseurStatut(cmdF: CommandeFournisseur): string {
  let r: string = "";
  if (cmdF) {
    //Process
    if ((cmdF.DDechargement && moment(cmdF.DDechargement).year() < 2012) || this.reparti) {
      r = this.applicationUserContext.getCulturedRessourceText(545);
    }
    else if (cmdF.DDechargement !== null) {
      r = this.applicationUserContext.getCulturedRessourceText(546);
    }
    else if (cmdF.RefusCamion === true) {
      r = this.applicationUserContext.getCulturedRessourceText(551);
    }
    else if (cmdF.CommandeFournisseurStatut !== null) {
      if (cmdF.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 1) {
        r = this.applicationUserContext.getCulturedRessourceText(547);
      }
      else if (cmdF.CommandeFournisseurStatut.RefCommandeFournisseurStatut === 2) {
        r = this.applicationUserContext.getCulturedRessourceText(548);
      }
      else {
        r = this.applicationUserContext.getCulturedRessourceText(549);
      }
    }
    else {
      r = this.applicationUserContext.getCulturedRessourceText(550);
    }
  }
  //End
  return r;
}

//-----------------------------------------------------------------------------------
//Create ContratCollectivite label
export function getContratCollectiviteLabel(contratCollectivite: ContratCollectivite): string {
  let s = moment(contratCollectivite.DDebut).format("L") + ' -> ' + moment(contratCollectivite.DFin).format("L");
  //End
  return s;
}

//-----------------------------------------------------------------------------------
//Create Contrat label (single line)
export function getContratLabelSingleLine(contrat: Contrat): string {
  let s = contrat.ContratType?.Libelle + " - "
    + moment(contrat.DDebut).format("L") + ' -> ' + moment(contrat.DFin).format("L");
  //End
  return s;
}

//-----------------------------------------------------------------------------------
//Create Contrat label (multi-line)
export function getContratLabelMultiLine(contrat: Contrat): string {
  let s = contrat.ContratType?.Libelle + "\n"
    + moment(contrat.DDebut).format("L") + ' -> ' + moment(contrat.DFin).format("L");
  //End
  return s;
}

//-----------------------------------------------------------------------------------
//Create ContratEntites label
export function getContratEntitesLabel(contrat: Contrat): string {
  let s = contrat.ContratEntites?.sort(function (a, b) { return cmp(a.Entite.Libelle, b.Entite.Libelle); })
    .map(item => item.Entite.LibelleCode).join("\n");
  //End
  return s;
}
//-----------------------------------------------------------------------------------
//Get Service-Fonctions for ContactAdresse
export function getContactAdresseServiceFonctions(cA: ContactAdresse): string {
  let s: string = "";
  if (cA?.ContactAdresseServiceFonctions) {
    s = Array.prototype.map.call(cA.ContactAdresseServiceFonctions
      , function (item: ContactAdresseServiceFonction) {
        let s: string = "";
        if (item.Fonction) { s += item.Fonction.Libelle };
        if (item.Fonction && item.Service) { s += " - " };
        if (item.Service) { s += item.Service.Libelle };
        return s;
      }).join(", ")
  }
  return s;
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Set original Menu
export function setOriginalMenu(applicationUserContext: ApplicationUserContext, eventEmitterService: EventEmitterService) {
  if (applicationUserContext.fromMenu) {
    applicationUserContext.currentModule = applicationUserContext.envModules.find(x => x.name === applicationUserContext.fromMenu.moduleName);
    applicationUserContext.currentMenu = applicationUserContext.fromMenu;
    applicationUserContext.fromMenu = null;
    eventEmitterService.onChangeMenu();
  }
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Get new GUID
export function newGUID() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    var r = Math.random() * 16 | 0,
      v = c == "x" ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Format date to standard day date
export function formatDateToStandard(d: string|moment.Moment) {
  return moment(d).format("L");
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Custom toFixed
export function toFixed(num: number, precision: number) {
  return (+(Math.round(+(num + "e" + precision)) + "e" + -precision)).toFixed(precision);
};

//------------------------------------------------------------------------------------------------------------------------------------------
//To space thousands separator
export function toSpaceThousandSepatator(num: number) {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Decompose single accented unicode character into 2 character+accent. Then, remove accent character
export function removeAccents(s: string) {
  return s.normalize("NFD").replace(/[\u0300-\u036f]/g, "")
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Return the Nth first characters followed by '...'
export function shortenLongText(s: string, nChar: number = 100, nLine: number = 5) {
  let r: string = s;
  if (typeof s == "string") {
    if (s) {
      //Shorten charachter number if applicable
      if (s.length > nChar - 3) {
        s = s.substring(0, nChar - 4) + '...';
      }
      //Shorten line number if applicable
      let lines = s.split("\n");
      if (lines.length > nLine) {
        s = lines.slice(0, nLine).join("\n") + '...';
      }
    }
  }
  return s;
}

//------------------------------------------------------------------------------------------------------------------------------------------
//Get Tooltip for ContactAdresseProcess
export function GetContactAdresseProcessTooltipRessourceId(r: number) {
  let i: number = 319;
  if (r) {
    switch (r.toString()) {
      case "1": i = 1453; break;
      case "2": i = 1454; break;
      case "3": i = 1455; break;
      case "4": i = 1456; break;
      case "5": i = 1457; break;
      case "6": i = 1458; break;
      case "7": i = 1459; break;
      case "8": i = 1460; break;
      case "9": i = 1461; break;
    }
  }
  return i;
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Get UtilisateurType localized label
export function GetUtilisateurTypeLocalizedLabel(r: string, applicationUserContext: ApplicationUserContext) : string {
  let s: string= "";
  if (r) {
    switch (r) {
      case "Valorplast": s = applicationUserContext.getCulturedRessourceText(1501); break;
      case "Collectivite": s = applicationUserContext.getCulturedRessourceText(594); break;
      case "Transporteur": s = applicationUserContext.getCulturedRessourceText(255); break;
      case "Client": s = applicationUserContext.getCulturedRessourceText(1502); break;
      case "CentreDeTri": s = applicationUserContext.getCulturedRessourceText(1503); break;
      case "Prestataire": s = applicationUserContext.getCulturedRessourceText(1251); break;
    }
  }
  return s;
};

//------------------------------------------------------------------------------------------------------------------------------------------
//Create mat-table column
export function createMatTableColumn(dataColumnName: string, applicationUserContext: ApplicationUserContext): MatTableColumn {
  let column: MatTableColumn;
  let dataColumn: EnvDataColumn;
  //Default column
  column = {
    columnDef: dataColumnName,
    header: dataColumnName.replace("#/#", "/"),
    sticky: false,
    stickyEnd: false,
    cell: (item: any) => item[dataColumnName]
  };
  //Defined column
  if (dataColumnName === DataColumnName.CommandeFournisseurLimiteExclusivite) {
    column.cell = (item: any) => ((item.CommandeFournisseurLimiteExclusivite == null || item.RefCommandeFournisseurStatut != "1" || item.CommandeFournisseurDDechargementInvisible !== null) ? "" : moment(item.CommandeFournisseurLimiteExclusivite).format("L"));
    column.header = applicationUserContext.envDataColumns.find(e => e.name == dataColumnName).culturedCaption;
  }
  else if (dataColumnName === DataColumnName.NonConformiteEtapeLibelle) {
    column.cell = (item: any) => (item.NonConformiteClotureeInvisible ? applicationUserContext.getCulturedRessourceText(165) : item.NonConformiteEtapeLibelle);
    column.header = applicationUserContext.envDataColumns.find(e => e.name == dataColumnName).culturedCaption;
  }
  else if ((dataColumnName === DataColumnName.CommandeFournisseurDelaiChargement || dataColumnName === DataColumnName.CommandeFournisseurDelaiChargement)
    && applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuDelaiCommandeEnlevement.toString()) {
    column.cell = (item: any) => (item[dataColumnName] == null ? "" : item[dataColumnName].toLocaleString());
    column.header = applicationUserContext.envDataColumns.find(e => e.name == dataColumnName).culturedCaption;
  }
  else {
    if (applicationUserContext.currentMenu.name !== MenuName.LogistiqueMenuEtatReceptionEmballagePlastique.toString()
      && applicationUserContext.currentMenu.name !== MenuName.LogistiqueMenuEtatSuiviTonnageRecycleur.toString()
      && applicationUserContext.currentMenu.name !== MenuName.LogistiqueMenuEtatDesFluxDev.toString()
    ) {
      dataColumn = applicationUserContext.envDataColumns.find(e => e.name == dataColumnName);
      if (dataColumn) {
        if (dataColumn.culturedCaption === "") {
          //Invisible columns
          return null;
        }
        else {
          column.header = dataColumn.culturedCaption;
        }
        switch (dataColumn.dataType) {
          case DataType.date.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : moment(item[dataColumnName]).format("L"));
            break;
          case DataType.text.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : item[dataColumnName]);
            break;
          case DataType.dateTime.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : moment(item[dataColumnName]).format("L") + " " + moment(item[dataColumnName]).format("LT"));
            break;
          case DataType.monthNumber.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : getCulturedMonthName(item[dataColumnName], applicationUserContext));
            break;
          case DataType.month.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : moment(item[dataColumnName]).format("MMMM"));
            break;
          case DataType.monthYear.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : moment(item[dataColumnName]).format("MMMM YYYY"));
            break;
          case DataType.year.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : moment(item[dataColumnName]).format("YYYY"));
            break;
          case DataType.bit.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : (item[dataColumnName] ? applicationUserContext.getCulturedRessourceText(338) : applicationUserContext.getCulturedRessourceText(347)));
            break;
          case DataType.intNumber.toString():
          case DataType.quarterNumber.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : Number(item[dataColumnName].toFixed(0)).toLocaleString());
            break;
          case DataType.decimalNumber2.toString():
          case DataType.decimalNumber3.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : item[dataColumnName].toLocaleString());
            break;
          case DataType.numeroCommande.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : getFormattedNumeroCommande(item[dataColumnName]));
            break;
          case DataType.percent.toString():
            column.cell = (item: any) => (item[dataColumnName] == null ? "" : (item[dataColumnName] * 100).toLocaleString() + "%");
            break;
        }
        //Sticky
        if (applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuCommandeFournisseur.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.ModulePrestataireMenuCommandeFournisseur.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuCommandeFournisseur.toString()
          && dataColumnName === DataColumnName.Controles) { column.stickyEnd = true; }
        if (applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuTransportCommande.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuTransportCommandeEnCours.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.LogistiqueMenuTransportCommandeModifiee.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.QualiteMenuControle.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.QualiteMenuControle.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNonConformite) { column.stickyEnd = true; }
        if (applicationUserContext.currentMenu.name == MenuName.QualiteMenuNonConformite.toString()
          && dataColumnName === DataColumnName.CommandeFournisseurNumeroCommande) { column.sticky = true; }
        if (applicationUserContext.currentMenu.name == MenuName.QualiteMenuNonConformite.toString()
          && dataColumnName === DataColumnName.NonConformiteEtapeLibelle) { column.sticky = true; }
      }
      else {
        //Undefined column
        column.cell = (item: any) => (item[dataColumnName] == null ? "" : (isNaN(item[dataColumnName]) ? item[dataColumnName] : item[dataColumnName].toLocaleString()));
      }
    }
  }
  return column;
}

//-----------------------------------------------------------------------------------------------------
//Available columns for contact extraction utils for an EntiteType
export function createContactAvailableColumns(globalContactAvailableColumns: EnvDataColumn[], entiteType : EntiteType, dataModelService: DataModelService): EnvDataColumn[] {
  let contactAvailableColumns: EnvDataColumn[] = [];
  //If one EntiteType
  if (entiteType) {
        globalContactAvailableColumns.forEach(
          eDC => {
            switch (eDC.name) {
              case DataColumnName.AdresseHoraires:
                if (entiteType.Horaires) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteCodeCITEO:
                if (entiteType.CodeEE) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteSousContrat:
                if (entiteType.SousContrat) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteCapacite:
                if (entiteType.Capacite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteActionnaireProprietaire:
                if (entiteType.ActionnaireProprietaire) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteExploitant:
                if (entiteType.Exploitant) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteDimensionBalle:
                if (entiteType.DimensionBalle) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EquipementierLibelle:
                if (entiteType.RefEquipementier) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.FournisseurTOLibelle:
                if (entiteType.RefFournisseurTO) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntitePopulationContratN:
                if (entiteType.PopulationContratN) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteVisibiliteAffretementCommun:
                if (entiteType.VisibiliteAffretementCommun) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteSurcoutCarburant:
                if (entiteType.SurcoutCarburant) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.ListeDR:
                if (entiteType.LiaisonEntiteDR) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeProcesss:
                if (entiteType.Process) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeStandards:
                if (entiteType.LiaisonEntiteStandard) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteCodeValorisation:
                if (entiteType.CodeValorisation) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EcoOrganismeLibelle:
                if (entiteType.RefEcoOrganisme) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteRepartitionMensuelle:
                if (entiteType.RepartitionMensuelle) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteAssujettiTVA:
                if (entiteType.AssujettiTVA) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteCodeTVA:
                if (entiteType.CodeTVA) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteIdNational:
                if (entiteType.IdNational) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteSAGECodeComptable:
                if (entiteType.SAGECodeComptable) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteSAGECompteTiers:
                if (entiteType.SAGECompteTiers) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SAGECategorieAchatLibelle:
                if (entiteType.RefSAGECategorieAchat) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SAGEConditionLivraisonLibelle:
                if (entiteType.RefSAGEConditionLivraison) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SAGEPeriodiciteLibelle:
                if (entiteType.RefSAGEPeriodicite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SAGEModeReglementLibelle:
                if (entiteType.RefSAGEModeReglement) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SAGECategorieVenteLibelle:
                if (entiteType.RefSAGECategorieVente) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteExportSAGE:
                if (entiteType.ExportSAGE) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.MiseEnPlaceAutocontrole:
                if (entiteType.AutoControle) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteReconductionIncitationQualite:
                if (entiteType.IncitationQualite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.RepriseTypeLibelle:
                if (entiteType.RefRepriseType) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.RepreneurLibelle:
                if (entiteType.RefRepreneur) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.ContratCollectiviteDebut:
                if (entiteType.ContratCollectivite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.ContratCollectiviteFin:
                if (entiteType.ContratCollectivite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteContratCollectiviteActif:
                if (entiteType.ContratCollectivite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeContratCollectivites:
                if (entiteType.ContratCollectivite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.SousContratIncitationQualite:
                if (entiteType.IncitationQualite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.ContratIncitationQualiteDebut:
                if (entiteType.IncitationQualite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.ContratIncitationQualiteFin:
                if (entiteType.IncitationQualite) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeProduits:
                if (entiteType.LiaisonEntiteProduit) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeProduitInterdits:
                if (entiteType.LiaisonEntiteProduitInterdit) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeCollectiviteActifs:
                if (entiteType.RefEntiteType == 3) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeCollectiviteInactifs:
                if (entiteType.RefEntiteType == 3) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.CentreDeTriListeCamionTypes:
                if (entiteType.LiaisonEntiteCamionType && entiteType.RefEntiteType == 3) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.TransporteurListeCamionTypes:
                if (entiteType.LiaisonEntiteCamionType && entiteType.RefEntiteType == 2) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeCentreDeTriActifs:
                if (entiteType.RefEntiteType == 1) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeCentreDeTriInactifs:
                if (entiteType.RefEntiteType == 1) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeCentreDeTriActifInterdits:
                if (entiteType.RefEntiteType == 4 || entiteType.RefEntiteType == 2) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteTransporteurActifInterdits:
                if (entiteType.RefEntiteType == 3 || entiteType.RefEntiteType == 4) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeClientActifInterdits:
                if (entiteType.RefEntiteType == 3 || entiteType.RefEntiteType == 2) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeIndustrielActifs:
                if (entiteType.RefEntiteType == 4 || entiteType.RefEntiteType == 2) { contactAvailableColumns.push(eDC); }
                break;
              case DataColumnName.EntiteListeDocumentPublic:
              case DataColumnName.EntiteListeDocumentValorplast:
              case DataColumnName.Id:
              case DataColumnName.EntiteLibelle:
              case DataColumnName.EntiteTypeLibelle:
              case DataColumnName.EntiteCmt:
              case DataColumnName.AdresseTypeLibelle:
              case DataColumnName.AdresseLibelle:
              case DataColumnName.AdresseAdr1:
              case DataColumnName.AdresseAdr2:
              case DataColumnName.AdresseCodePostal:
              case DataColumnName.AdresseVille:
              case DataColumnName.AdresseSiteWeb:
              case DataColumnName.PaysLibelle:
              case DataColumnName.AdresseTel:
              case DataColumnName.AdresseEmail:
              case DataColumnName.TitreLibelle:
              case DataColumnName.CiviliteLibelle:
              case DataColumnName.ContactPrenom:
              case DataColumnName.ContactNom:
              case DataColumnName.ContactAdresseCmt:
              case DataColumnName.ContactAdresseListeFonctionServices:
              case DataColumnName.ContactAdresseCmtServiceFonction:
              case DataColumnName.ContactAdresseTel:
              case DataColumnName.ContactAdresseTelMobile:
              case DataColumnName.ContactAdresseEmail:
              case DataColumnName.ContactAdresseListeProcesss:
              case DataColumnName.ContactAdresseIsUser:
                contactAvailableColumns.push(eDC);
                break;
            }
          }
        )
  }
  //End
  return contactAvailableColumns;
}

//-----------------------------------------------------------------------------------------------------
//Colors for charting
export const chartSeriesColors: string[] = [
  "#4451cb",
  "#55b040",
  "#ffc82a",
  "#fe4201",
  "#ac00b5",
  "#05a5fb",
  "#ff9601",
  "#9e9e9e",
  "#009b8a",
  "#c71050",
  "#5a7d8d",
  "#68d34f",
  "#fff000",
  "#7e5444"
];
//-----------------------------------------------------------------------------------------------------
//Colors for charting
export const chartSeriesMonoColor: string[] = [
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
  "#6ab64e",
];
//-----------------------------------------------------------------------------------------------------
//Colors for serie's points
export function pointColor(point: SeriesPoint) {
  return getNextColor(point["index"]);
}
//-----------------------------------------------------------------------------------------------------
//Colors for serie's points
export function getNextColor(i: number) {
  let n : number = i % 14;
  return chartSeriesColors[n];
}
//-----------------------------------------------------------------------------------------------------
//Array utils
export function clearFormArray(formArray: FormArray) {
  while (formArray.length !== 0) {
    formArray.removeAt(0)
  }
}
////-----------------------------------------------------------------------------------------------------
////Array utils
//export function createCommaSeparatedList(arr:any[]): string {
//    let s: string = "";
//    let i: any;
//    for (i in arr) {
//        switch (i.kind) {
//        "":
//    }
//        s += i.RefEntite + ",";
//    }
//    //End
//    return s;
//}
