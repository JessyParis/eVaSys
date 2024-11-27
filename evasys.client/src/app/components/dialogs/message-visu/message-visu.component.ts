import { Component, HostListener, Inject, OnInit, ViewEncapsulation } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { DataModelService } from "../../../services/data-model.service";
import { createRouterNavigationProperties, showErrorToUser } from "../../../globals/utils";
import { Router } from '@angular/router';
import { UtilsService } from "../../../services/utils.service";
import { EventEmitterService } from "../../../services/event-emitter.service";
import { MenuName, ModuleName } from "../../../globals/enums";

@Component({
    selector: "message-visu",
    templateUrl: "./message-visu.component.html",
    encapsulation: ViewEncapsulation.None,
    standalone: false
})

export class MessageVisuComponent implements OnInit {
  message: dataModelsInterfaces.Message;
  constructor(public dialogRef: MatDialogRef<MessageVisuComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    , public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , public dialog: MatDialog
    , private router: Router
    , private eventEmitterService: EventEmitterService
  ) {
  }
  //-----------------------------------------------------------------------------------
  //Listen to clicks
  @HostListener("click", ['$event'])
  onClick(event: MouseEvent) {
    // If we don't have an anchor tag, we don't need to do anything.
    if (event.target instanceof HTMLAnchorElement === false) {
      return;
    }
    // Prevent page from reloading
    event.preventDefault();
    let target = event.target as HTMLAnchorElement;
    let appLink = createRouterNavigationProperties(target.pathname)
    // Navigate to the path in the link if applicable
    if (appLink.valid) {
      //Change menu
      switch (appLink.componentName) {
        case "utilisateur":
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Administration);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.AdministrationMenuUtilisateur);
          //Emit event for app compoment refresh (active menus and modules)
          this.eventEmitterService.onChangeMenu();
          break;
        case "entite":
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Annuaire);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.AnnuaireMenuEntite);
          //Emit event for app compoment refresh (active menus and modules)
          this.eventEmitterService.onChangeMenu();
          break;
        case "commande-fournisseur":
          this.applicationUserContext.currentModule = this.applicationUserContext.envModules.find(x => x.name === ModuleName.Logistique);
          this.applicationUserContext.currentMenu = this.applicationUserContext.envMenus.find(x => x.name === MenuName.LogistiqueMenuCommandeFournisseur);
          //Emit event for app compoment refresh (active menus and modules)
          this.eventEmitterService.onChangeMenu();
          break;
      }
      //Navigate
      if (appLink.navExtra) {
        this.router.navigate([appLink.componentName, appLink.componentRef, appLink.navExtra])
      }
      else {
        this.router.navigate([appLink.componentName, appLink.componentRef])
      }
    }
    //Close dialog
    this.dialogRef.close();
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    //Reset all read, then read
    if (this.data.mode == "all") {
      //Look for message to display
      this.dataModelService.resetMessageRead()
        .subscribe(result => {
          this.dataModelService.getMessageToDisplay(null, true)
            .subscribe(result => {
              this.message = result;
              //Check for messages
              this.checkExistsMessageToDisplay();
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Read
      this.dataModelService.getMessageToDisplay(null, true)
        .subscribe(result => {
          this.message = result;
          //Check for messages
          this.checkExistsMessageToDisplay();
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
  }
  //-----------------------------------------------------------------------------------
  //Dismiss message
  onDismiss() {
    this.dataModelService.getMessageToDisplay(null, true)
      .subscribe(result => {
        this.message = null;
        if (result) {
          this.message = result;
        }
        else {
          //Close dialog if no message
          this.dialogRef.close();
        }
        //Check for messages
        this.checkExistsMessageToDisplay();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Confirm read message
  onRead() {
    this.dataModelService.getMessageToDisplay(this.message.RefMessage, true)
      .subscribe(result => {
        this.message = null;
        if (result) {
          this.message = result;
        }
        else {
          //Close dialog if no message
          this.dialogRef.close();
        }
        //Check for messages
        this.checkExistsMessageToDisplay();
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Check for messages
  checkExistsMessageToDisplay() {
    this.dataModelService.existsMessageToDisplay(false)
      .subscribe(result => {
        this.applicationUserContext.hasMessage = result; 
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
}
