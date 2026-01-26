/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 22/05/2019
/// ----------------------------------------------------------------------------------------------------- 
///
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import moment from "moment";
import * as dataModelsInterfaces from "../interfaces/dataModelsInterfaces";
import { ApplicationUserContext } from "../globals/globals";
import { tap } from "rxjs/operators";
import { Month, Quarter } from "../interfaces/appInterfaces";

//-----------------------------------------------------------------------------------
//Data models services
@Injectable()
export class DataModelService {
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, public applicationUserContext: ApplicationUserContext) { }
  //-----------------------------------------------------------------------------------
  //Get generic data model from component name and id
  getDataModel<T extends { AllowStandardCRUD: string }>(id: number, component: string): Observable<T> {
    return this.getGenericDataModel<T>(id, component, null);
  }
  private getGenericDataModel<T>(id: number, component: string, headers: HttpHeaders): Observable<T> {
    let r = new Observable<T>();
    if (headers) {
      r = this.http.get<T>(this.baseUrl + this.applicationUserContext.getURLforComponent(component) + "/" + id.toString(), {
        headers: headers,
        responseType: "json"
      });
    }
    else {
      r = this.http.get<T>(this.baseUrl + this.applicationUserContext.getURLforComponent(component) + "/" + id.toString(), {
        responseType: "json"
      });
    }
    // Converts datetime strings into moments on the returned object
    return r.pipe(
      tap(result => this.SwitchMomentText(component, result, false))
    );
  }
  //-----------------------------------------------------------------------------------
  //Post generic data model from component name and object
  postDataModel<T extends { AllowStandardCRUD: string }>(obj: T, component: string): Observable<any> {
    return this.postGenericDataModel<T>(obj, component);
  }
  private postGenericDataModel<T>(obj: T, component: string): Observable<any> {
    this.SwitchMomentText(component, obj, true);
    return this.http.post<T>(this.baseUrl + this.applicationUserContext.getURLforComponent(component), obj);
  }
  //-----------------------------------------------------------------------------------
  //Delete generic data model from component name and id
  deleteDataModel<T extends { AllowStandardCRUD: string }>(id: number, component: string): Observable<any> {
    return this.deleteGenericDataModel(id, component);
  }
  private deleteGenericDataModel<T>(id: number, component: string): Observable<any> {
    return this.http.delete(this.baseUrl + this.applicationUserContext.getURLforComponent(component) + "/" + id.toString())
  }
  //-----------------------------------------------------------------------------------
  //Converts strings into Moments or Moments into strings
  private SwitchMomentText(component: string, obj: any, toText: boolean) {
    // Checks whether the object has moments or not
    let cmp = this.applicationUserContext.envComponents.find(x => x.name === component)
    if (!cmp || cmp.moments == "")
      return;
    // If moments have been defined, check all object properties to convert values
    let result = moment(obj.D).format("YYYY-MM-DDTHH:mm:ss");
    var list = cmp.moments.split(",")
    Object.keys(obj).forEach(key => {
      if (list.includes(key)) {
        obj[key] = (obj[key] == null ? null : (toText ? moment(obj[key]).format("YYYY-MM-DDTHH:mm:ss") : moment(obj[key])));
      }
    })
  }
  //-----------------------------------------------------------------------------------
  //Check if Adresse can be deleted
  isDeletableAdresse(refAdresse: number, excludeProcess: boolean): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/adresse/isdeletable", {
      headers: new HttpHeaders()
        .set("refAdresse", !refAdresse ? "" : refAdresse.toString())
        .set("excludeProcess", excludeProcess == true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if ContactAdresse can be deleted
  isDeletableContactAdresse(refContactAdresse: number, excludeProcess: boolean): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/contactadresse/isdeletable", {
      headers: new HttpHeaders()
        .set("refContactAdresse", !refContactAdresse ? "" : refContactAdresse.toString())
        .set("excludeProcess", excludeProcess == true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Aide search by component name (Composant)
  getAideByComposant(composant: string): Observable<dataModelsInterfaces.Aide> {
    return this.http.get<dataModelsInterfaces.Aide>(this.baseUrl + "evapi/aide/getaidebycomposant", {
      headers: new HttpHeaders()
        .set("composant", composant),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeClient
  getCommandeClient(refCommandeClient: number, refClient: number, refAdresse: number, refProduit: number, d: moment.Moment, refCommandeFournisseur: number): Observable<dataModelsInterfaces.CommandeClient> {
    return this.http.get<dataModelsInterfaces.CommandeClient>(this.baseUrl + "evapi/commandeclient/" + refCommandeClient.toString(), {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", !refCommandeFournisseur ? "" : refCommandeFournisseur.toString())
        .set("refEntite", !refClient ? "" : refClient.toString())
        .set("refAdresse", !refAdresse ? "" : refAdresse.toString())
        .set("refProduit", !refProduit ? "" : refProduit.toString())
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000")),
      responseType: "json"
    })
      .pipe(
        tap(result => this.setMomentFromTextCommandeClient(result))
      );
  }
  //-----------------------------------------------------------------------------------
  //Get e-mail template
  getEmailTemplate(emailType: string, year: number, d: moment.Moment): Observable<dataModelsInterfaces.Email> {
    return this.http.get<dataModelsInterfaces.Email>(this.baseUrl + "evapi/email/createemail", {
      headers: new HttpHeaders()
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000"))
        .set("year", !year ? "0" : year.toString())
        .set("emailType", !emailType ? "" : emailType),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeFournisseur
  getCommandeFournisseur(refCommandeFournisseur: number, lockData: boolean): Observable<dataModelsInterfaces.CommandeFournisseur> {
    return this.http.get<dataModelsInterfaces.CommandeFournisseur>(this.baseUrl + "evapi/commandefournisseur/" + refCommandeFournisseur.toString(), {
      headers: new HttpHeaders()
        .set("lockData", lockData === true ? "true" : "false"),
      responseType: "json"
    })
      .pipe(
        tap(result => this.setMomentFromTextCommandeFournisseur(result))
      );
  }
  //-----------------------------------------------------------------------------------
  //Post existing CommandeFournisseur
  postCommandeFournisseur(commandeFournisseur: dataModelsInterfaces.CommandeFournisseur, unLockData: boolean): Observable<dataModelsInterfaces.CommandeFournisseur> {
    this.setTextFromMomentCommandeFournisseur(commandeFournisseur);
    return this.http.post<dataModelsInterfaces.CommandeFournisseur>(this.baseUrl + "evapi/commandefournisseur", commandeFournisseur, {
      headers: new HttpHeaders()
        .set("unLockData", unLockData === true ? "true" : "false"),
      responseType: "json"
    })
      .pipe(
        tap(result => this.setMomentFromTextCommandeFournisseur(result))
      );
  }
  //-----------------------------------------------------------------------------------
  //Delete CommandeFournisseur
  deleteCommandeFournisseur(refCommandeFournisseur: number): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/commandefournisseur/" + refCommandeFournisseur.toString());
  }
  //-----------------------------------------------------------------------------------
  //Create email CommandeFournisseur
  createEmailCommandeFournisseur(refCommandeFournisseur: number, emailType: string): Observable<number> {
    return this.http.get<number>(this.baseUrl + "evapi/commandefournisseur/createemail", {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", !refCommandeFournisseur ? "" : refCommandeFournisseur.toString())
        .set("emailType", !emailType ? "" : emailType),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get SurcoutCarburant
  getSurcoutCarburantRatio(d: moment.Moment, refTransporteur: number, refPays: number): Observable<number> {
    var url = this.baseUrl + "evapi/surcoutcarburant/getsurcoutcarburantratio";
    return this.http.get<number>(url, {
      headers: new HttpHeaders()
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000"))
        .set("refTransporteur", !refTransporteur ? "" : refTransporteur.toString())
        .set("refPays", !refPays ? "" : refPays.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeFournisseurStatut
  getCommandeFournisseurStatut(refCommandeFournisseurStatut: number): Observable<dataModelsInterfaces.CommandeFournisseurStatut> {
    return this.http.get<dataModelsInterfaces.CommandeFournisseurStatut>(this.baseUrl + "evapi/commandefournisseurstatut/" + refCommandeFournisseurStatut.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get Controle
  getControle(refControle: number, refProduit: number): Observable<dataModelsInterfaces.Controle> {
    return this.http.get<dataModelsInterfaces.Controle>(this.baseUrl + "evapi/controle/" + refControle.toString(), {
      headers: new HttpHeaders()
        .set("refProduit", !refProduit ? "0" : refProduit.toString()),
      responseType: "json"
    })
      .pipe(
        tap(result => this.setMomentFromTextControle(result))
      );
  }
  //-----------------------------------------------------------------------------------
  //Post Controle
  postControle(controle: dataModelsInterfaces.Controle): Observable<dataModelsInterfaces.Controle> {
    this.setTextFromMomentControle(controle);
    return this.http.post<dataModelsInterfaces.Controle>(this.baseUrl + "evapi/controle", controle);
  }
  //-----------------------------------------------------------------------------------
  //Delete Controle
  deleteControle(controle: dataModelsInterfaces.Controle): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/controle/" + controle.RefControle.toString());
  }
  //-----------------------------------------------------------------------------------
  //Get CVQ
  getCVQ(refCVQ: number, refProduit: number): Observable<dataModelsInterfaces.CVQ> {
    return this.http.get<dataModelsInterfaces.CVQ>(this.baseUrl + "evapi/cvq/" + refCVQ.toString(), {
      headers: new HttpHeaders()
        .set("refProduit", !refProduit ? "0" : refProduit.toString()),
      responseType: "json"
    })
      .pipe(
        tap(result => this.setMomentFromTextCVQ(result))
      );
  }
  //-----------------------------------------------------------------------------------
  //Post CVQ
  postCVQ(cVQ: dataModelsInterfaces.CVQ): Observable<dataModelsInterfaces.CVQ> {
    this.setTextFromMomentCVQ(cVQ);
    return this.http.post<dataModelsInterfaces.CVQ>(this.baseUrl + "evapi/cvq", cVQ);
  }
  //-----------------------------------------------------------------------------------
  //Delete CVQ
  deleteCVQ(cVQ: dataModelsInterfaces.CVQ): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/cvq/" + cVQ.RefCVQ.toString());
  }
  //-----------------------------------------------------------------------------------
  //Get existing DescriptionControle
  getDescriptionControle(refDescriptionControle: number): Observable<dataModelsInterfaces.DescriptionControle> {
    return this.http.get<dataModelsInterfaces.DescriptionControle>(this.baseUrl + "evapi/descriptioncontrole/" + refDescriptionControle.toString(), {
      responseType: "json"
    });
  }
  //Get existing DescriptionControles
  getDescriptionControles(refProduit: number): Observable<dataModelsInterfaces.DescriptionControle[]> {
    return this.http.get<dataModelsInterfaces.DescriptionControle[]>(this.baseUrl + "evapi/descriptioncontrole/getdescriptioncontroles", {
      headers: new HttpHeaders()
        .set("refProduit", !refProduit ? "" : refProduit.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing DescriptionCVQ
  getDescriptionCVQ(refDescriptionCVQ: number): Observable<dataModelsInterfaces.DescriptionCVQ> {
    return this.http.get<dataModelsInterfaces.DescriptionCVQ>(this.baseUrl + "evapi/descriptioncvq/" + refDescriptionCVQ.toString(), {
      responseType: "json"
    });
  }
  //Get existing DescriptionCVQs
  getDescriptionCVQs(refProduit: number): Observable<dataModelsInterfaces.DescriptionCVQ[]> {
    return this.http.get<dataModelsInterfaces.DescriptionCVQ[]>(this.baseUrl + "evapi/descriptioncVQ/getdescriptioncvqs", {
      headers: new HttpHeaders()
        .set("refProduit", !refProduit ? "" : refProduit.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing DescriptionReception
  getDescriptionReception(refDescriptionReception: number): Observable<dataModelsInterfaces.DescriptionReception> {
    return this.http.get<dataModelsInterfaces.DescriptionReception>(this.baseUrl + "evapi/descriptionreception/" + refDescriptionReception.toString(), {
      responseType: "json"
    });
  }
  //Get existing DescriptionReceptions
  getDescriptionReceptions(): Observable<dataModelsInterfaces.DescriptionReception[]> {
    return this.http.get<dataModelsInterfaces.DescriptionReception[]>(this.baseUrl + "evapi/descriptionreception/getdescriptionreceptions", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get estimated nbBalle
  getEstimatedNbBalle(refProduit: number, refEntite: number): Observable<any> {
    var url = this.baseUrl + "evapi/commandefournisseur/getnbballe";
    return this.http.get<any[]>(url, {
      headers: new HttpHeaders()
        .set("refProduit", refProduit.toString())
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if CommandeFournisseur is linked to a Repartition
  hasRepartition(refCommandeFournisseur: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/hasrepartition";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if CommandeFournisseur is linked to a completed Repartition
  hasRepartitionComplete(refCommandeFournisseur: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/hasrepartitioncomplete";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if CommandeFournisseur is linked to a FicheControle
  hasFicheControle(refCommandeFournisseur: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/hasfichecontrole";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get all CommandeFournisseur from a Mixte
  getAllMixte(numeroAffretement: number): Observable<number[]> {
    var url = this.baseUrl + "evapi/commandefournisseur/getallmixte";
    return this.http.get<number[]>(url, {
      headers: new HttpHeaders()
        .set("numeroAffretement", numeroAffretement.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if CommandeFournisseur is Mixte
  isMixte(refCommandeFournisseur: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/ismixte";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if similar CommandeFournisseur without dates exists
  isSimilarToCommandeFournisseur(refCommandeFournisseur: number, d: moment.Moment): Observable<any[]> {
    var url = this.baseUrl + "evapi/commandefournisseur/issimilar";
    return this.http.get<any[]>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString())
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if similar Repartition exists: same Produit, less than 1 year old
  isSimilarRepartition(refCommandeFournisseur: number, d: moment.Moment): Observable<dataModelsInterfaces.Repartition> {
    var url = this.baseUrl + "evapi/repartition/issimilar";
    return this.http.get<dataModelsInterfaces.Repartition>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString())
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Unmark Mixtes linked CommandeFournisseur
  unmarkMixte(numeroCommande: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/unmarkmixte";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("numeroCommande", numeroCommande.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Unlock CommandeFournisseur
  unlockCommandeFournisseur(refCommandeFournisseur: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/commandefournisseur/unlock";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if Parametre has file
  existsBinaryData(refParametre: number): Observable<boolean> {
    var url = this.baseUrl + "evapi/parametre/existsbinarydata";
    return this.http.get<boolean>(url, {
      headers: new HttpHeaders()
        .set("refParametre", refParametre.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Repartition
  getRepartition(refRepartition: number, refCommandeFournisseur: number): Observable<dataModelsInterfaces.Repartition> {
    return this.http.get<dataModelsInterfaces.Repartition>(this.baseUrl + "evapi/repartition/" + (refRepartition ? refRepartition.toString() : "0"), {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", (refCommandeFournisseur ? refCommandeFournisseur.toString() : "0")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CommandeFournisseur
  getRepartitionCommandesFournisseurs(d: string | moment.Moment, refProduit: number, refFournisseur): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl + "evapi/repartition/getcommandefournisseurs", {
      headers: new HttpHeaders()
        .set("d", moment(d).format("YYYY-MM-DD 00:00:00.000"))
        .set("refProduit", refProduit.toString())
        .set("refFournisseur", refFournisseur.toString()),
      responseType: "json"
    })
  }
  //-----------------------------------------------------------------------------------
  //Post Repartition
  postRepartition(Repartition: dataModelsInterfaces.Repartition): Observable<dataModelsInterfaces.Repartition> {
    return this.http.post<dataModelsInterfaces.Repartition>(this.baseUrl + "evapi/repartition", Repartition);
  }
  //-----------------------------------------------------------------------------------
  //Delete Repartition
  deleteRepartition(refRepartition: number): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/repartition/" + refRepartition.toString());
  }
  //-----------------------------------------------------------------------------------
  //Get existing Action
  getAction(refAction: number): Observable<dataModelsInterfaces.Action> {
    return this.http.get<dataModelsInterfaces.Action>(this.baseUrl + "evapi/action/" + (refAction ? refAction.toString() : "0"), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Post Action
  postAction(action: dataModelsInterfaces.Action): Observable<dataModelsInterfaces.Action> {
    return this.http.post<dataModelsInterfaces.Action>(this.baseUrl + "evapi/action", action);
  }
  //-----------------------------------------------------------------------------------
  //Delete Action
  deleteAction(action: dataModelsInterfaces.Action): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/action/" + action.RefAction.toString());
  }
  //-----------------------------------------------------------------------------------
  //Get existing ActionFichierNoFiles
  getActionFichierNoFiles(refAction: number): Observable<dataModelsInterfaces.ActionFichierNoFile[]> {
    return this.http.get<dataModelsInterfaces.ActionFichierNoFile[]>(this.baseUrl + "evapi/action/getactionfichiernofiles/", {
      params: new HttpParams()
        .set("refAction", refAction.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Entite
  getEntite(refEntite: number, refContactAdresse: number, refAction: number): Observable<dataModelsInterfaces.Entite> {
    return this.http.get<dataModelsInterfaces.Entite>(this.baseUrl + "evapi/entite/" + (refEntite ? refEntite.toString() : "0"), {
      headers: new HttpHeaders()
        .set("refContactAdresse", !refContactAdresse ? "0" : refContactAdresse.toString())
        .set("refAction", !refAction ? "0" : refAction.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing ContactAdresse for Entite
  getEntiteContacts(refEntite: number): Observable<dataModelsInterfaces.Entite> {
    return this.http.get<dataModelsInterfaces.Entite>(this.baseUrl + "evapi/entite/getcontacts", {
      headers: new HttpHeaders()
        .set("refEntite", !refEntite ? "0" : refEntite.toString())
      ,
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Post Entite
  postEntite(entite: dataModelsInterfaces.Entite): Observable<dataModelsInterfaces.Entite> {
    return this.http.post<dataModelsInterfaces.Entite>(this.baseUrl + "evapi/entite", entite);
  }
  //-----------------------------------------------------------------------------------
  //Delete Entite
  deleteEntite(entite: dataModelsInterfaces.Entite): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/entite/" + entite.RefEntite.toString());
  }
  //-----------------------------------------------------------------------------------
  //Get existing DocumentEntites
  getDocumentEntites(refEntite: number): Observable<dataModelsInterfaces.DocumentEntite[]> {
    return this.http.get<dataModelsInterfaces.DocumentEntite[]>(this.baseUrl + "evapi/entite/getdocumententites/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EtatTrimestrielCSs
  getEntiteEtatTrimestrielCSs(refEntite: number): Observable<Quarter[]> {
    return this.http.get<Quarter[]>(this.baseUrl + "evapi/entite/getetattrimestrielcss/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EtatMensuelHCSs
  getEntiteEtatMensuelHCSs(refEntite: number): Observable<Month[]> {
    return this.http.get<Month[]>(this.baseUrl + "evapi/entite/getetatmensuelhcss/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CertificatRecyclageCSs
  getEntiteCertificatRecyclageCSs(refEntite: number): Observable<Quarter[]> {
    return this.http.get<Quarter[]>(this.baseUrl + "evapi/entite/getcertificatrecyclagecss/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing CertificatRecyclageHCSs
  getEntiteCertificatRecyclageHCSs(refEntite: number): Observable<Quarter[]> {
    return this.http.get<Quarter[]>(this.baseUrl + "evapi/entite/getcertificatrecyclagehcss/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing EtatMensuelReception
  getEntiteEtatMensuelReceptions(refEntite: number): Observable<Month[]> {
    return this.http.get<Month[]>(this.baseUrl + "evapi/entite/getetatmensuelreceptions/", {
      params: new HttpParams()
        .set("refEntite", refEntite.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing DocumentType
  getEntiteDocumentType(refEntite: number, refDocumentType: number, year: number): any {
    return this.http.get(this.baseUrl + "evapi/entite/getdocumenttype/", {
      headers: new HttpHeaders()
        .set("refEntite", refEntite.toString())
        .set("refDocumentType", refDocumentType.toString())
        .set("year", year.toString()),
      observe: "response",
      responseType: "blob"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get FicheControle
  getFicheControle(refFicheControle: number, refCommandeFournisseur: number): Observable<dataModelsInterfaces.FicheControle> {
    return this.http.get<dataModelsInterfaces.FicheControle>(this.baseUrl + "evapi/fichecontrole/" + refFicheControle.toString(), {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", !refCommandeFournisseur ? "0" : refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get related FicheControle
  getRelatedRefFicheControle(refCommandeFournisseur: number): Observable<number> {
    return this.http.get<number>(this.baseUrl + "evapi/fichecontrole/getrelatedreffichecontrole", {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", !refCommandeFournisseur ? "0" : refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Adresse
  getAdresse(refAdresse: number, withEntite: boolean): Observable<dataModelsInterfaces.Adresse> {
    return this.http.get<dataModelsInterfaces.Adresse>(this.baseUrl + "evapi/adresse/" + (refAdresse ? refAdresse.toString() : "0"), {
      headers: new HttpHeaders()
        .set("withEntite", withEntite ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Post Adresse
  postAdresse(adresse: dataModelsInterfaces.Adresse): Observable<dataModelsInterfaces.Adresse> {
    return this.http.post<dataModelsInterfaces.Adresse>(this.baseUrl + "evapi/adresse", adresse);
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformite
  getNonConformite(refNonConformite: number, refCommandeFournisseur: number): Observable<dataModelsInterfaces.NonConformite> {
    return this.http.get<dataModelsInterfaces.NonConformite>(this.baseUrl + "evapi/nonconformite/" + refNonConformite.toString(), {
      headers: new HttpHeaders()
        .set("refCommandeFournisseur", !refCommandeFournisseur ? "0" : refCommandeFournisseur.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Post existing NonConformite
  postNonConformite(nonConformite: dataModelsInterfaces.NonConformite, unLockData: boolean): Observable<dataModelsInterfaces.NonConformite> {
    return this.http.post<dataModelsInterfaces.NonConformite>(this.baseUrl + "evapi/nonconformite", nonConformite, {
      headers: new HttpHeaders()
        .set("unLockData", unLockData === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Delete NonConformite
  deleteNonConformite(refNonConformite: number): Observable<any> {
    return this.http.delete(this.baseUrl + "evapi/nonconformite/" + refNonConformite.toString());
  }
  //-----------------------------------------------------------------------------------
  //Set facture data to NonConformites
  setNonConforimiteIFClientFacture(refNonConformites: string, iFClientFactureNro: string, iFClientDFacture: moment.Moment): Observable<number> {
    return this.http.get<number>(this.baseUrl + "evapi/nonconformite/setnonconformiteifclientfacture", {
      headers: new HttpHeaders()
        .set("refNonConformites", refNonConformites)
        .set("iFClientFactureNro", iFClientFactureNro)
        .set("iFClientDFacture", !iFClientDFacture ? "" : moment(iFClientDFacture).format("YYYY-MM-DDT00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformiteDemandeClientType
  getNonConformiteDemandeClientType(refNonConformiteDemandeClientType: number): Observable<dataModelsInterfaces.NonConformiteDemandeClientType> {
    return this.http.get<dataModelsInterfaces.NonConformiteDemandeClientType>(this.baseUrl + "evapi/nonconformitedemandeclienttype/" + refNonConformiteDemandeClientType.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformiteNature
  getNonConformiteNature(refNonConformiteNature: number): Observable<dataModelsInterfaces.NonConformiteNature> {
    return this.http.get<dataModelsInterfaces.NonConformiteNature>(this.baseUrl + "evapi/nonconformitenature/" + refNonConformiteNature.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformiteFamille
  getNonConformiteFamille(refNonConformiteFamille: number): Observable<dataModelsInterfaces.NonConformiteFamille> {
    return this.http.get<dataModelsInterfaces.NonConformiteFamille>(this.baseUrl + "evapi/nonconformitefamille/" + refNonConformiteFamille.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformiteReponseClientType
  getNonConformiteReponseClientType(refNonConformiteReponseClientType: number): Observable<dataModelsInterfaces.NonConformiteReponseClientType> {
    return this.http.get<dataModelsInterfaces.NonConformiteReponseClientType>(this.baseUrl + "evapi/nonconformitereponseclienttype/" + refNonConformiteReponseClientType.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get NonConformiteReponseFournisseurType
  getNonConformiteReponseFournisseurType(refNonConformiteReponseFournisseurType: number): Observable<dataModelsInterfaces.NonConformiteReponseFournisseurType> {
    return this.http.get<dataModelsInterfaces.NonConformiteReponseFournisseurType>(this.baseUrl + "evapi/nonconformitereponsefournisseurtype/" + refNonConformiteReponseFournisseurType.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Reset all read messages for today for connected user
  resetMessageRead(): Observable<dataModelsInterfaces.Message> {
    return this.http.get<dataModelsInterfaces.Message>(this.baseUrl + "evapi/message/resetmessageread", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get next message to display to the user
  getMessageToDisplay(refMessageRead: number, notSeenToday: boolean): Observable<dataModelsInterfaces.Message> {
    return this.http.get<dataModelsInterfaces.Message>(this.baseUrl + "evapi/message/getmessagetodisplay", {
      headers: new HttpHeaders()
        .set("refMessageRead", refMessageRead == null ? "" : refMessageRead.toString())
        .set("notSeenToday", notSeenToday ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check if a message exists to display to the user
  existsMessageToDisplay(notSeenToday: boolean): Observable<number> {
    return this.http.get<number>(this.baseUrl + "evapi/message/getmessagetodisplay", {
      headers: new HttpHeaders()
        .set("checkExists", "true")
        .set("notSeenToday", notSeenToday ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Message
  getMessage(refMessage: number, visualisations: boolean): Observable<dataModelsInterfaces.Message> {
    return this.http.get<dataModelsInterfaces.Message>(this.baseUrl + "evapi/message/" + refMessage.toString(), {
      headers: new HttpHeaders()
        .set("visualisations", visualisations ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing MontantIncitationQualite
  getMontantIncitationQualite(refMontantIncitationQualite: number, lockData: boolean): Observable<dataModelsInterfaces.MontantIncitationQualite> {
    return this.http.get<dataModelsInterfaces.MontantIncitationQualite>(this.baseUrl + "evapi/montantincitationqualite/" + refMontantIncitationQualite.toString(), {
      headers: new HttpHeaders()
        .set("lockData", lockData === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing PrixReprise
  getPrixReprise(refPrixReprise: number, refProcess: number, refProduit: number, refComposant: number, refEntite: number, d: moment.Moment): Observable<dataModelsInterfaces.PrixReprise> {
    return this.http.get<dataModelsInterfaces.PrixReprise>(this.baseUrl + "evapi/prixreprise/" + refPrixReprise.toString(), {
      headers: new HttpHeaders()
        .set("refProcess", !refProcess ? "" : refProcess.toString())
        .set("refProduit", !refProduit ? "" : refProduit.toString())
        .set("refComposant", !refComposant ? "" : refComposant.toString())
        .set("refEntite", !refEntite ? "" : refEntite.toString())
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000")),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing PrixReprise for collectivite
  getCollectivitePrixReprises(refEntite: number, d: moment.Moment, filterCollecte: string): Observable<dataModelsInterfaces.CollectivitePrixReprise[]> {
    return this.http.get<dataModelsInterfaces.CollectivitePrixReprise[]>(this.baseUrl + "evapi/prixreprise/getcollectiviteprixreprises", {
      headers: new HttpHeaders()
        .set("refEntite", !refEntite ? "0" : refEntite.toString())
        .set("d", !d ? "" : moment(d).format("YYYY-MM-DDT00:00:00.000"))
        .set("filterCollecte", filterCollecte)
      ,
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Produit
  getProduit(refProduit: number): Observable<dataModelsInterfaces.Produit> {
    return this.http.get<dataModelsInterfaces.Produit>(this.baseUrl + "evapi/produit/" + refProduit.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Process
  getProcess(refProcess: number): Observable<dataModelsInterfaces.Process> {
    return this.http.get<dataModelsInterfaces.Process>(this.baseUrl + "evapi/process/" + refProcess.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get Securite
  getSecurite(): Observable<dataModelsInterfaces.Securite> {
    return this.http.get<dataModelsInterfaces.Securite>(this.baseUrl + "evapi/securite/", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Post existing Securite
  postSecurite(securite: dataModelsInterfaces.Securite): Observable<dataModelsInterfaces.Securite> {
    return this.http.post<dataModelsInterfaces.Securite>(this.baseUrl + "evapi/securite", securite, {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SurcoutCarburant
  getSurcoutCarburant(refSurcoutCarburant: number): Observable<dataModelsInterfaces.SurcoutCarburant> {
    return this.http.get<dataModelsInterfaces.SurcoutCarburant>(this.baseUrl + "evapi/surcoutcarburant/" + refSurcoutCarburant.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing SurcoutCarburants
  getSurcoutCarburants(refTransporteur: number, refPays: number, year: number): Observable<dataModelsInterfaces.SurcoutCarburant[]> {
    return this.http.get<dataModelsInterfaces.SurcoutCarburant[]>(this.baseUrl + "evapi/surcoutcarburant/getsurcoutcarburants", {
      headers: new HttpHeaders()
        .set("refTransporteur", !refTransporteur ? "" : refTransporteur.toString())
        .set("refPays", !refPays ? "" : refPays.toString())
        .set("year", !year ? "" : year.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Transport
  getTransport(refTransport: number): Observable<dataModelsInterfaces.Transport> {
    return this.http.get<dataModelsInterfaces.Transport>(this.baseUrl + "evapi/transport/" + refTransport.toString(), {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Get existing Transport Competitors
  getTransportCompetitors(refTransport: number): Observable<any> {
    return this.http.get<any>(this.baseUrl + "evapi/transport/getcompetitors", {
      headers: new HttpHeaders()
        .set("refTransport", !refTransport ? "" : refTransport.toString()),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check wether connected Utilisateur is linked to some Entites
  isDR(): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/utilisateur/isdr", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check wether connected Utilisateur must change his password
  mustModifyPassword(): Observable<number> {
    return this.http.get<number>(this.baseUrl + "evapi/utilisateur/mustmodifypassword", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Check wether connected Utilisateur has a e-mail address
  hasEmail(): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/utilisateur/hasemail", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Set FilterDR status
  setFilterDR(val: boolean): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/utilisateur/setfilterdr", {
      headers: new HttpHeaders()
        .set("val", val === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Set FilterGlobalActif status
  setFilterGlobalActif(val: boolean): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "evapi/utilisateur/setfilterglobalactif", {
      headers: new HttpHeaders()
        .set("val", val === true ? "true" : "false"),
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  // retrieve the connected user from the server
  getUserFromServer(): Observable<dataModelsInterfaces.Utilisateur> {
    return this.http.get<dataModelsInterfaces.Utilisateur>(this.baseUrl + "evapi/utilisateur/getconnected", {
      responseType: "json"
    });
  }
  //-----------------------------------------------------------------------------------
  //Change text dates to moment dates
  setMomentFromTextAction(obj: dataModelsInterfaces.Action) {
    obj.DAction = (obj.DAction == null ? null : moment(obj.DAction));
  }
  setMomentFromTextCommandeClient(obj: dataModelsInterfaces.CommandeClient) {
    obj.D = (obj.D == null ? null : moment(obj.D));
  }
  setMomentFromTextCommandeFournisseur(obj: dataModelsInterfaces.CommandeFournisseur) {
    obj.D = (obj.D == null ? null : moment(obj.D));
    obj.DChargementPrevue = (obj.DChargementPrevue == null ? null : moment(obj.DChargementPrevue));
    obj.DChargement = (obj.DChargement == null ? null : moment(obj.DChargement));
    obj.DDechargementPrevue = (obj.DDechargementPrevue == null ? null : moment(obj.DDechargementPrevue));
    obj.DDechargement = (obj.DDechargement == null ? null : moment(obj.DDechargement));
    obj.DMoisDechargementPrevu = (obj.DMoisDechargementPrevu == null ? null : moment(obj.DMoisDechargementPrevu));
    obj.DBlocage = (obj.DBlocage == null ? null : moment(obj.DBlocage));
    obj.DAffretement = (obj.DAffretement == null ? null : moment(obj.DAffretement));
    obj.DChargementAnnule = (obj.DChargementAnnule == null ? null : moment(obj.DChargementAnnule));
    obj.DChargementModif = (obj.DChargementModif == null ? null : moment(obj.DChargementModif));
    obj.DAnomalieChargement = (obj.DAnomalieChargement == null ? null : moment(obj.DAnomalieChargement));
    obj.DTraitementAnomalieChargement = (obj.DTraitementAnomalieChargement == null ? null : moment(obj.DTraitementAnomalieChargement));
    obj.DAnomalieClient = (obj.DAnomalieClient == null ? null : moment(obj.DAnomalieClient));
    obj.DTraitementAnomalieClient = (obj.DTraitementAnomalieClient == null ? null : moment(obj.DTraitementAnomalieClient));
    obj.DAnomalieTransporteur = (obj.DAnomalieTransporteur == null ? null : moment(obj.DAnomalieTransporteur));
    obj.DTraitementAnomalieTransporteur = (obj.DTraitementAnomalieTransporteur == null ? null : moment(obj.DTraitementAnomalieTransporteur));
    obj.DAnomalieOk = (obj.DAnomalieOk == null ? null : moment(obj.DAnomalieOk));
  }
  setMomentFromTextCVQ(obj: dataModelsInterfaces.CVQ) {
    obj.DCVQ = (obj.DCVQ == null ? null : moment(obj.DCVQ));
  }
  setMomentFromTextControle(obj: dataModelsInterfaces.Controle) {
    obj.DControle = (obj.DControle == null ? null : moment(obj.DControle));
  }
  setMomentFromTextEntite(obj: dataModelsInterfaces.Entite) {
    obj.EntiteEntites?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation)));
    obj.EntiteCamionTypes?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation)));
    obj.EntiteProduits?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation)));
    obj.ContratIncitationQualites?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut));
      e.DFin = (e.DFin == null ? null : moment(e.DFin));
    });
    obj.ContratCollectivites?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut));
      e.DFin = (e.DFin == null ? null : moment(e.DFin));
    });
    obj.Contrats?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut));
      e.DFin = (e.DFin == null ? null : moment(e.DFin));
    });
    obj.DocumentEntites?.forEach(e => {
      e.DocumentNoFile.DDebut = (e.DocumentNoFile?.DDebut == null ? null : moment(e.DocumentNoFile?.DDebut));
      e.DocumentNoFile.DFin = (e.DocumentNoFile?.DFin == null ? null : moment(e.DocumentNoFile?.DFin));
    });
    obj.Actions?.forEach(e => {
      e.DAction = (e.DAction == null ? null : moment(e.DAction));
    });
    obj.SAGEDocuments?.forEach(e => {
      e.D = (e.D == null ? null : moment(e.D));
    });
  }
  setMomentFromTextQuarter(obj: Quarter) {
    obj.Begin = (obj.Begin == null ? null : moment(obj.Begin));
    obj.End = (obj.End == null ? null : moment(obj.End));
  }
  setMomentFromTextMonth(obj: Month) {
    obj.Begin = (obj.Begin == null ? null : moment(obj.Begin));
    obj.End = (obj.End == null ? null : moment(obj.End));
  }
  setMomentFromTextEmail(obj: dataModelsInterfaces.Email) {
    obj.DReception = (obj.DReception == null ? null : moment(obj.DReception));
    obj.DEnvoi = (obj.DEnvoi == null ? null : moment(obj.DEnvoi));
  }
  setMomentFromTextMessage(obj: dataModelsInterfaces.Message) {
    obj.DDebut = (obj.DDebut == null ? null : moment(obj.DDebut));
    obj.DFin = (obj.DFin == null ? null : moment(obj.DFin));
    obj.MessageVisualisations?.forEach(e => {
      e.D = (e.D == null ? null : moment(e.D));
    });
  }
  setMomentFromTextNonConformite(obj: dataModelsInterfaces.NonConformite) {
    obj.DTransmissionFournisseur = (obj.DTransmissionFournisseur == null ? null : moment(obj.DTransmissionFournisseur));
    obj.IFClientDFacture = (obj.IFClientDFacture == null ? null : moment(obj.IFClientDFacture));
  }
  //-----------------------------------------------------------------------------------
  //Change moment dates to text dates
  setTextFromMomentAction(obj: dataModelsInterfaces.Action) {
    obj.DAction = (obj.DAction == null ? null : moment(obj.DAction).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentCommandeClient(obj: dataModelsInterfaces.CommandeClient) {
    obj.D = (obj.D == null ? null : moment(obj.D).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentCommandeFournisseur(obj: dataModelsInterfaces.CommandeFournisseur) {
    obj.D = (obj.D == null ? null : moment(obj.D).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DChargementPrevue = (obj.DChargementPrevue == null ? null : moment(obj.DChargementPrevue).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DChargement = (obj.DChargement == null ? null : moment(obj.DChargement).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DDechargementPrevue = (obj.DDechargementPrevue == null ? null : moment(obj.DDechargementPrevue).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DDechargement = (obj.DDechargement == null ? null : moment(obj.DDechargement).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DMoisDechargementPrevu = (obj.DMoisDechargementPrevu == null ? null : moment(obj.DMoisDechargementPrevu).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DBlocage = (obj.DBlocage == null ? null : moment(obj.DBlocage).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DAffretement = (obj.DAffretement == null ? null : moment(obj.DAffretement).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DChargementAnnule = (obj.DChargementAnnule == null ? null : moment(obj.DChargementAnnule).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DChargementModif = (obj.DChargementModif == null ? null : moment(obj.DChargementModif).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DAnomalieChargement = (obj.DAnomalieChargement == null ? null : moment(obj.DAnomalieChargement).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DTraitementAnomalieChargement = (obj.DTraitementAnomalieChargement == null ? null : moment(obj.DTraitementAnomalieChargement).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DAnomalieClient = (obj.DAnomalieClient == null ? null : moment(obj.DAnomalieClient).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DTraitementAnomalieClient = (obj.DTraitementAnomalieClient == null ? null : moment(obj.DTraitementAnomalieClient).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DAnomalieTransporteur = (obj.DAnomalieTransporteur == null ? null : moment(obj.DAnomalieTransporteur).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DTraitementAnomalieTransporteur = (obj.DTraitementAnomalieTransporteur == null ? null : moment(obj.DTraitementAnomalieTransporteur).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DAnomalieOk = (obj.DAnomalieOk == null ? null : moment(obj.DAnomalieOk).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentCVQ(obj: dataModelsInterfaces.CVQ) {
    obj.DCVQ = (obj.DCVQ == null ? null : moment(obj.DCVQ).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentControle(obj: dataModelsInterfaces.Controle) {
    obj.DControle = (obj.DControle == null ? null : moment(obj.DControle).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentEmail(obj: dataModelsInterfaces.Email) {
    obj.DReception = (obj.DReception == null ? null : moment(obj.DReception).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DEnvoi = (obj.DEnvoi == null ? null : moment(obj.DEnvoi).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentEntite(obj: dataModelsInterfaces.Entite) {
    obj.EntiteEntites?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation).format("YYYY-MM-DDTHH:mm:ss")));
    obj.EntiteCamionTypes?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation).format("YYYY-MM-DDTHH:mm:ss")));
    obj.EntiteProduits?.forEach(e => e.DCreation = (e.DCreation == null ? null : moment(e.DCreation).format("YYYY-MM-DDTHH:mm:ss")));
    obj.ContratIncitationQualites?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut).format("YYYY-MM-DDTHH:mm:ss"));
      e.DFin = (e.DFin == null ? null : moment(e.DFin).format("YYYY-MM-DDTHH:mm:ss"));
    });
    obj.ContratCollectivites?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut).format("YYYY-MM-DDTHH:mm:ss"));
      e.DFin = (e.DFin == null ? null : moment(e.DFin).format("YYYY-MM-DDTHH:mm:ss"));
    });
    obj.Contrats?.forEach(e => {
      e.DDebut = (e.DDebut == null ? null : moment(e.DDebut).format("YYYY-MM-DD"));
      e.DFin = (e.DFin == null ? null : moment(e.DFin).format("YYYY-MM-DD"));
    });
    obj.DocumentEntites?.forEach(e => {
      e.DocumentNoFile.DDebut = (e.DocumentNoFile?.DDebut == null ? null : moment(e.DocumentNoFile?.DDebut).format("YYYY-MM-DDTHH:mm:ss"));
      e.DocumentNoFile.DFin = (e.DocumentNoFile?.DFin == null ? null : moment(e.DocumentNoFile?.DFin).format("YYYY-MM-DDTHH:mm:ss"));
    });
    obj.Actions?.forEach(e => {
      e.DAction = (e.DAction == null ? null : moment(e.DAction).format("YYYY-MM-DDTHH:mm:ss"));
    });
  }
  setTextFromMomentMessage(obj: dataModelsInterfaces.Message) {
    obj.DDebut = (obj.DDebut == null ? null : moment(obj.DDebut).format("YYYY-MM-DDTHH:mm:ss"));
    obj.DFin = (obj.DFin == null ? null : moment(obj.DFin).format("YYYY-MM-DDTHH:mm:ss"));
  }
  setTextFromMomentNonConformite(obj: dataModelsInterfaces.NonConformite) {
    obj.DTransmissionFournisseur = (obj.DTransmissionFournisseur == null ? null : moment(obj.DTransmissionFournisseur).format("YYYY-MM-DDTHH:mm:ss"));
    obj.IFClientDFacture = (obj.IFClientDFacture == null ? null : moment(obj.IFClientDFacture).format("YYYY-MM-DDTHH:mm:ss"));
  }
}
