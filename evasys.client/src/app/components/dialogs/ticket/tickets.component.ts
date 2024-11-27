import { Component, Inject, OnInit, Input } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators, UntypedFormControl, ValidationErrors, FormGroupDirective, NgForm } from "@angular/forms";
import { HttpClient } from "@angular/common/http";
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ApplicationUserContext } from "../../../globals/globals";
import { ListService } from "../../../services/list.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { SelectionModel } from "@angular/cdk/collections";
import { showErrorToUser } from "../../../globals/utils";

@Component({
    selector: "ticket",
    templateUrl: "./tickets.component.html",
    standalone: false
})

export class TicketsComponent implements OnInit {
  tickets: dataModelsInterfaces.Ticket[];
  form: UntypedFormGroup;
  filterTexteFC: UntypedFormControl = new UntypedFormControl(null);
  //Global lock
  locked: boolean = true;
  //Misc
  title: string = this.applicationUserContext.getCulturedRessourceText(683);
  selection = new SelectionModel<any>(true, []);
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder
    , public applicationUserContext: ApplicationUserContext
    , private listService: ListService
    , public dialogRef: MatDialogRef<TicketsComponent>
    , public dialog: MatDialog
    , ) {
    // create an empty object from the Ticket interface
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      FilterText: this.filterTexteFC
    });
  }
  //-----------------------------------------------------------------------------------
  //Data loading
  ngOnInit() {
    //Get Tickets
    this.listService.getTickets("").subscribe(result => {
      this.tickets = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Apply filter
  applyFilter() {
    //Get Tickets
    this.listService.getTickets(this.filterTexteFC.value == null ? "" : this.filterTexteFC.value).subscribe(result => {
      this.tickets = result;
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Apply tickets to the e-mail
  onApply() {
    //Concanate tickets
    let s: string = "";
    s = (Array.from(this.selection.selected, item => item.TicketTexteHTMLFromText)).join("<br/>");
    //Emit event for dialog closing
    return this.dialogRef.close({ ticketTexte: s });
  }
}
