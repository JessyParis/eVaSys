import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { PictureService } from "../../../services/picture.service";
import { showErrorToUser } from "../../../globals/utils";

@Component({
    selector: "image-gallery",
    templateUrl: "./image-gallery.component.html",
    styleUrls: ["./image-gallery.component.scss"],
    standalone: false
})
//-----------------------------------------------------------------------------------
//Component class
export class ImageGalleryComponent implements OnInit {
  public height: string = "600px";
  public width: string = "600px";
  public activeIndex: number = 0;
  pictures: any[] = [];
  @Input() elementType: string;
  @Input() elementId: string;
  @Input() pictureId: string;
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext
    , private pictureService: PictureService
    , public dialog: MatDialog
  ) {
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Check parameters
    //console.log(this.elementType);
    //console.log(this.elementId);
    //console.log(this.pictureId);
    let elId: number = parseInt(this.elementId, 10);
    if (!isNaN(elId)) {
      //Get pictures array
      switch (this.elementType) {
        case "CommandeFournisseur":
          this.pictureService.getCommandeFournisseurFichierPictures(elId).subscribe(result => {
            this.pictures = result;
            //Set the active index of the screen view
            let aI: number = parseInt(this.pictureId, 10);
            if (!isNaN(aI)) {
              let r = this.pictures.map(e => e.RefCommandeFournisseurFichier).indexOf(aI);
              if (r >= 0) { this.activeIndex = r; }
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
        case "NonConformite_1":
          this.pictureService.getNonConformitePictures(elId, 1).subscribe(result => {
            this.pictures = result;
            //Set the active index of the screen view
            let aI: number = parseInt(this.pictureId, 10);
            if (!isNaN(aI)) {
              let r = this.pictures.map(e => e.RefNonConformiteFichier).indexOf(aI);
              if (r >= 0) { this.activeIndex = r; }
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
        case "NonConformite_2":
          this.pictureService.getNonConformitePictures(elId, 2).subscribe(result => {
            this.pictures = result;
            //Set the active index of the screen view
            let aI: number = parseInt(this.pictureId, 10);
            if (!isNaN(aI)) {
              let r = this.pictures.map(e => e.RefNonConformiteFichier).indexOf(aI);
              if (r >= 0) { this.activeIndex = r; }
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
        case "NonConformite_3":
          this.pictureService.getNonConformitePictures(elId, 3).subscribe(result => {
            this.pictures = result;
            //Set the active index of the screen view
            let aI: number = parseInt(this.pictureId, 10);
            if (!isNaN(aI)) {
              let r = this.pictures.map(e => e.RefNonConformiteFichier).indexOf(aI);
              if (r >= 0) { this.activeIndex = r; }
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
        case "NonConformite_4":
          this.pictureService.getNonConformitePictures(elId, 4).subscribe(result => {
            this.pictures = result;
            //Set the active index of the screen view
            let aI: number = parseInt(this.pictureId, 10);
            if (!isNaN(aI)) {
              let r = this.pictures.map(e => e.RefNonConformiteFichier).indexOf(aI);
              if (r >= 0) { this.activeIndex = r; }
            }
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          break;
      }
    }
  }
}




