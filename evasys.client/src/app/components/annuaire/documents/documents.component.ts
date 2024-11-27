import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { cmpDesc, formatDateToStandard, getAttachmentFilename, showErrorToUser } from "../../../globals/utils";
import { DataModelService } from "../../../services/data-model.service";
import { DocumentType } from "../../../globals/enums";
import { DownloadService } from "../../../services/download.service";
import moment from "moment";
import { finalize } from "rxjs";
import { ProgressBarComponent } from "../../global/progress-bar/progress-bar.component";
import { ProgressBarMode } from "@angular/material/progress-bar";
import { ThemePalette } from "@angular/material/core";
import { fadeInOnEnterAnimation } from "angular-animations";
import { SAGECodeTransportComponent } from "../../administration/administration-components";

@Component({
    selector: "documents",
    templateUrl: "./documents.component.html",
    animations: [
        fadeInOnEnterAnimation(),
    ],
    standalone: false
})

export class DocumentsComponent implements OnInit {
  form: UntypedFormGroup;
  entite: dataModelsInterfaces.Entite = { Adresses: [], ContactAdresses: [] } as dataModelsInterfaces.Entite;
  refEntite: number = 0;
  documentsType: string = DocumentType.Documents;
  documentEntites: dataModelsInterfaces.DocumentEntite[] = [];
  entiteSAGEDocuments: dataModelsInterfaces.SAGEDocument[] = [];
  etatTrimestrielCSs: appInterfaces.Quarter[] = [];
  etatMensuelHCSs: appInterfaces.Month[] = [];
  certificatRecyclageCSs: appInterfaces.Quarter[] = [];
  certificatRecyclageHCSs: appInterfaces.Quarter[] = [];
  etatMensuelReceptions: appInterfaces.Month[] = [];

  currentMonth: string = "";
  //Functions
  formatDateToStandard = formatDateToStandard;
  //Loading indicator
  loading = false;
  color = "warn" as ThemePalette;
  mode = "indeterminate" as ProgressBarMode;
  value = 50;
  displayProgressBar = false;
  @ViewChild(ProgressBarComponent) progressBar: ProgressBarComponent;

  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router, private fb: UntypedFormBuilder
    , private http: HttpClient, @Inject("BASE_URL") private baseUrl: string
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , public dialog: MatDialog) {
    //Sow/hide ModuleCollectivite header
    if (applicationUserContext.connectedUtilisateur.Collectivite?.RefEntite) {
      applicationUserContext.visibleHeaderCollectivite = true;
    }
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    this.documentsType = this.activatedRoute.snapshot.params["documentsType"];
    if (this.applicationUserContext.connectedUtilisateur.CentreDeTri) {
      this.documentsType = DocumentType.EtatMensuelReception;
    }
    //Set filters
    if (this.applicationUserContext.filterCollectivites.length == 1) {
      this.refEntite = this.applicationUserContext.filterCollectivites[0].RefEntite;
    }
    if (this.applicationUserContext.filterTransporteurs.length == 1) {
      this.refEntite = this.applicationUserContext.filterTransporteurs[0].RefEntite;
    }
    if (this.applicationUserContext.filterClients.length == 1) {
      this.refEntite = this.applicationUserContext.filterClients[0].RefEntite;
    }
    if (this.applicationUserContext.filterCentreDeTris.length == 1) {
      this.refEntite = this.applicationUserContext.filterCentreDeTris[0].RefEntite;
    }
    this.dataModelService.getEntite(this.refEntite, null, null)
      .subscribe(result => {
        //Get data
        this.entite = result;
        this.dataModelService.setMomentFromTextEntite(this.entite);
        //Init arrays
        this.entite.DocumentEntites = this.entite.DocumentEntites.filter(f => f.DocumentNoFile.VisibiliteTotale == true);
        //SAGE documents only for collectivités
        if (this.entite.EntiteType?.RefEntiteType == 1 || this.entite.EntiteType?.RefEntiteType == 3) {
          this.entiteSAGEDocuments = this.entite.SAGEDocuments.filter(f => moment(f.D) > moment(new Date(new Date().getFullYear() - 4, 0, 1, 0, 0, 0, 0)));
        }
        //Sort
        this.entite.DocumentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
        this.entiteSAGEDocuments.sort(function (a, b) { return cmpDesc(a.D, b.D); });
        //Collectivité specific documents
        if (this.entite.EntiteType?.RefEntiteType === 1) {
          //Get EtatTrimestrielCSs
          this.dataModelService.getEntiteEtatTrimestrielCSs(this.refEntite)
            .subscribe(result => {
              this.etatTrimestrielCSs = result;
              this.etatTrimestrielCSs.forEach(e => this.dataModelService.setMomentFromTextQuarter(e))
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get EtatMensuelHCSs
          this.dataModelService.getEntiteEtatMensuelHCSs(this.refEntite)
            .subscribe(result => {
              this.etatMensuelHCSs = result;
              this.etatMensuelHCSs.forEach(e => this.dataModelService.setMomentFromTextMonth(e))
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get CertificatRecyclageCS
          this.dataModelService.getEntiteCertificatRecyclageCSs(this.refEntite)
            .subscribe(result => {
              this.certificatRecyclageCSs = result;
              this.certificatRecyclageCSs.forEach(e => this.dataModelService.setMomentFromTextQuarter(e))
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          //Get CertificatRecyclageHCS
          this.dataModelService.getEntiteCertificatRecyclageHCSs(this.refEntite)
            .subscribe(result => {
              this.certificatRecyclageHCSs = result;
              this.certificatRecyclageHCSs.forEach(e => this.dataModelService.setMomentFromTextQuarter(e))
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        if (this.entite.EntiteType?.RefEntiteType === 3) {
          //Get EtatMensuelReceptions
          this.dataModelService.getEntiteEtatMensuelReceptions(this.refEntite)
            .subscribe(result => {
              this.etatMensuelReceptions = result;
              this.etatMensuelReceptions.forEach(e => this.dataModelService.setMomentFromTextMonth(e))
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        //Get DocumentEntites
        this.dataModelService.getDocumentEntites(this.refEntite).subscribe(result => {
          //Get data
          this.documentEntites = result.filter(f => f.DocumentNoFile.VisibiliteTotale == true);
          //Sort
          this.documentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //On filter
  onChipChange($event: any) {
    if (!$event.selected) {
      $event.source.selected = true;
    }
    if (this.documentsType != $event.source.value) {
      this.documentsType = $event.source.value;
    }
  }
  public getSAGEDocumentDate(sD: dataModelsInterfaces.SAGEDocument): string {
    return moment(sD.D).format("L");
  }
  public getSAGEDocumentHT(sD: dataModelsInterfaces.SAGEDocument): string {
    return sD.TotalHT.toLocaleString(undefined, { minimumFractionDigits: 2 });
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refDocument: number, de : dataModelsInterfaces.DocumentEntite) {
    this.downloadService.download(fileType, refDocument.toString(), "", this.entite.RefEntite, "")
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        console.log(error);
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  downloadFile(data: any, fileName: string) {
    const blob = new Blob([data]);
    {
      const url = window.URL.createObjectURL(blob);
      const a: HTMLAnchorElement = document.createElement("a") as HTMLAnchorElement;

      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }
  }
  //-----------------------------------------------------------------------------------
  //Export PDF file
  exportPdf(typeDoc: string, refDoc: string) {
    //Show progress bar
    this.progressBar.start();
    //Get Excel file
    this.downloadService.download(typeDoc, refDoc, "pdf", 0, null)
      .pipe(
        finalize(() => this.progressBar.stop())
      )
      .subscribe((data: Response) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      },
        error => {
          showErrorToUser(this.dialog, error, this.applicationUserContext);
        });
  }
  //-----------------------------------------------------------------------------------
  //Export EtatTrimestrielCS
  exportEtatTrimestrielCS(q: appInterfaces.Quarter) {
    var url = this.baseUrl + "evapi/documentedit/etattrimerstrielcollectivitecs";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("filterCollectivites", this.refEntite.toString())
          .set("quarter", q == null ? "0" : q.Nb.toString())
          .set("year", q == null ? "0" : q.Year.toString()),
        observe: "response",
        responseType: "blob"
      })
      .pipe(
        finalize(() => this.displayProgressBar = false)
      )
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Export EtatTrimestrielHCS
  exportEtatMensuelHCS(m: appInterfaces.Month) {
    var url = this.baseUrl + "evapi/documentedit/etatmensuelcollectivitehcs";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("filterCollectivites", this.refEntite.toString())
          .set("month", m == null ? "0" : m.Nb.toString())
          .set("year", m == null ? "0" : m.Year.toString()),
        observe: "response",
        responseType: "blob"
      })
      .pipe(
        finalize(() => this.displayProgressBar = false)
      )
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Export CertificatRecyclage
  exportCertificatRecyclage(q: appInterfaces.Quarter, cS: boolean) {
    var url = this.baseUrl + "evapi/documentedit/certificatrecyclage";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("filterCollectivites", this.refEntite.toString())
          .set("cs", cS == true ? "true" : "false")
          .set("quarter", q == null ? "0" : q.Nb.toString())
          .set("year", q == null ? "0" : q.Year.toString()),
        observe: "response",
        responseType: "blob"
      })
      .pipe(
        finalize(() => this.displayProgressBar = false)
      )
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Export EtatTrimestrielReception
  exportEtatMensuelReception(m: appInterfaces.Month) {
    var url = this.baseUrl + "evapi/documentedit/etatmensuelreception";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("filterCentreDeTris", this.refEntite.toString())
          .set("month", m == null ? "0" : m.Nb.toString())
          .set("year", m == null ? "0" : m.Year.toString()),
        observe: "response",
        responseType: "blob"
      })
      .pipe(
        finalize(() => this.displayProgressBar = false)
      )
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
  //-----------------------------------------------------------------------------------
  //Export FicheRepartition
  exportFicheRepartion() {
    var url = this.baseUrl + "evapi/documentedit/ficherepartition";
    this.http
      .get(url, {
        headers: new HttpHeaders()
          .set("refCentreDeTri", this.refEntite.toString()),
        observe: "response",
        responseType: "blob"
      })
      .pipe(
        finalize(() => this.displayProgressBar = false)
      )
      .subscribe((data: HttpResponse<Blob>) => {
        let contentDispositionHeader = data.headers.get("Content-Disposition");
        let fileName = getAttachmentFilename(contentDispositionHeader);
        this.downloadFile(data.body, fileName);
      }, error => {
        showErrorToUser(this.dialog, error, this.applicationUserContext);
      });
  }
}
