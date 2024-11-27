import { Component, EventEmitter, Inject, Input, OnInit, Output, Renderer2, inject, signal } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "../../services/auth.service";
import { ApplicationUserContext } from "../../globals/globals"
import { DataModelService } from "../../services/data-model.service";
import { MatDialog } from "@angular/material/dialog";
import { GetUtilisateurTypeLocalizedLabel, changeCulture, cmp, showErrorToUser, showInformationToUser } from "../../globals/utils";
import moment from "moment";
import { InformationComponent } from "../dialogs/information/information.component";
import { InputFormComponentElementType, UtilisateurType } from "../../globals/enums";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { Utilisateur, UtilisateurList } from "../../interfaces/dataModelsInterfaces";
import { ComponentRelativeComponent } from "../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import { DateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
import { ListService } from "../../services/list.service";
@Component({
    selector: "profils",
    templateUrl: "./profils.component.html",
    standalone: false
})

export class ProfilsComponent implements OnInit {
  @Input() changeProfil: boolean = false;
  @Input() type: string;
  @Output() askClose = new EventEmitter<any>();
  profils: dataModelsInterfaces.UtilisateurList[] = [];
  //Constructor
  constructor(private router: Router
    , private authService: AuthService
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private listService: ListService
    , public dialog: MatDialog
    , private snackBarQueueService: SnackBarQueueService) {
  }
  //Init
  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      //Get all available profiles
      let refU = this.applicationUserContext.connectedUtilisateur.RefUtilisateur;
      if (this.applicationUserContext.connectedUtilisateur.RefUtilisateurMaitre) {
        refU = this.applicationUserContext.connectedUtilisateur.RefUtilisateurMaitre;
      }
      this.listService.getListUtilisateur(refU, "", "", null, true, false, false, true)
        .subscribe(result => {
          this.profils = result;
          result.sort(function (a, b) { return cmp(a.UtilisateurType + a.EntiteLibelle, b.UtilisateurType + b.EntiteLibelle); });
      },
        error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //On profil selected
  onProfil(profil: UtilisateurList) {
    if (this.applicationUserContext.profilSelected
      && profil.RefUtilisateur == this.applicationUserContext.connectedUtilisateur.RefUtilisateur) {
      //Emit event for dialog closing
      this.askClose.emit({ type: this.type, ref: "" });
    }
    else {
      this.authService.connectUser(profil, this.dialog, this.askClose, this.type);
    }
  }
  //-----------------------------------------------------------------------------------
  //Create profils info
  getProfilInfos(profil: UtilisateurList): string {
    let s = GetUtilisateurTypeLocalizedLabel(profil.UtilisateurType, this.applicationUserContext);
    //End
    return s;
  }
  //-----------------------------------------------------------------------------------
  //Create profil CSS
  getProfilClass(profil: UtilisateurList): string {
    let s = "background-color-primary-a-light";
    if (this.applicationUserContext.profilSelected && profil.RefUtilisateur == this.applicationUserContext.connectedUtilisateur.RefUtilisateur) {
      s = "background-color-accent";
    }
    //Add general styling
    s += " ev-profil-item"
    //End
    return s;
  }
}
