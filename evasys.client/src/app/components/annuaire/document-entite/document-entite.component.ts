import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ApplicationUserContext } from "../../../globals/globals";
import { MatDialog } from "@angular/material/dialog";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { cmp, cmpDesc, getAttachmentFilename, showErrorToUser } from "../../../globals/utils";
import { DataModelService } from "../../../services/data-model.service";
import { HttpResponse } from "@angular/common/http";
import { DownloadService } from "../../../services/download.service";

@Component({
    selector: "document-entite",
    templateUrl: "./document-entite.component.html",
    standalone: false
})

export class DocumentEntiteComponent implements OnInit {
  @Input() refEntite: number;
  @Output() askClose = new EventEmitter<any>();
  documentEntites: dataModelsInterfaces.DocumentEntite[]=[];
  noDocument: boolean = false;
  //Pipes
  filterEntiteDocuments = (dE: dataModelsInterfaces.DocumentEntite, filterGlobalActif: boolean, refEntiteType: number, visibiliteTotale: boolean) => {
    return (dE.DocumentNoFile.VisibiliteTotale == visibiliteTotale);
  }
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private downloadService: DownloadService
    , public dialog: MatDialog) {
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
      //Get Utilisateur
    this.dataModelService.getDocumentEntites(this.refEntite).subscribe(result => {
        //Get data
        this.documentEntites = result;
        //Sort
      this.documentEntites.sort(function (a, b) { return cmpDesc(a.DocumentNoFile.DCreation, b.DocumentNoFile.DCreation); });
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Download a file
  getFile(fileType: string, refDocument: number) {
    this.downloadService.download(fileType, refDocument.toString(), "", this.refEntite, "")
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
}
