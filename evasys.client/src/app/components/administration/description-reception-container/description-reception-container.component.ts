import { Component, Inject, OnInit } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";
import { ApplicationUserContext } from "../../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { DataModelService } from "../../../services/data-model.service";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { showErrorToUser } from "../../../globals/utils";

class MyErrorStateMatcher implements ErrorStateMatcher {
  //Constructor
  constructor() { }
  isErrorState(control: UntypedFormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    let result: boolean = false;
    result = !!((control && control.invalid));
    return result;
  }
}

@Component({
    selector: "description-reception-container",
    templateUrl: "./description-reception-container.component.html",
    standalone: false
})

export class DescriptionReceptionContainerComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  descriptionReceptions: dataModelsInterfaces.DescriptionReception[] = [];
  // Subject that emits when the component has been destroyed.
  //Constructor
  constructor(private activatedRoute: ActivatedRoute, private router: Router,
    private http: HttpClient, @Inject("BASE_URL") private baseUrl: string, private fb: UntypedFormBuilder, public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , public dialog: MatDialog) {
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    //Load initial data
    //Get data
    this.dataModelService.getDescriptionReceptions().subscribe(result => {
      if (result) {
        this.descriptionReceptions = result;
      }
      else {
        this.descriptionReceptions = [];
      }
    }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
  }
  //-----------------------------------------------------------------------------------
  //Add new element
  onAdd() {
    //Create new element
    let newElement = <dataModelsInterfaces.DescriptionReception>{ RefDescriptionReception: 0 }
    //Add element
    this.descriptionReceptions.push(newElement);
  }
  //-----------------------------------------------------------------------------------
  //Back to list
  onBack() {
    this.router.navigate(["grid"]);
  }
}
