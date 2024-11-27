import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormControl, FormGroupDirective, NgForm, Validators } from "@angular/forms";
import { ApplicationUserContext } from '../../../globals/globals';
import { InputFormComponentElementType } from "../../../globals/enums";
import { ErrorStateMatcher } from "@angular/material/core";

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
    selector: "input-form",
    templateUrl: "./input-form.component.html",
    standalone: false
})

export class InputFormComponent {
  matcher = new MyErrorStateMatcher();
  //Form
  form: UntypedFormGroup;
  cmtFC: UntypedFormControl = new UntypedFormControl(null);
  eMailFC: UntypedFormControl = new UntypedFormControl(null, [Validators.required, Validators.email]);
  //Visibility
  cmtVisible: boolean = false;
  eMailVisible: boolean = false;
  //Misc
  buttonLabel: string = this.applicationUserContext.getCulturedRessourceText(6);
  //Constructor
  constructor(public applicationUserContext: ApplicationUserContext
    , public dialogRef: MatDialogRef<InputFormComponent>
    , @Inject(MAT_DIALOG_DATA) public data: any
    , private fb: UntypedFormBuilder
  ) {
    this.createForm();
  }
  //-----------------------------------------------------------------------------------
  //Form initial creation
  createForm() {
    this.form = this.fb.group({
      Cmt: this.cmtFC,
      EMail: this.eMailFC,
    });
    if (this.data.cmt) { this.cmtFC.setValue(this.data.cmt) }
    if (this.data.eMail) { this.eMailFC.setValue(this.data.eMail) }
    //Visibility
    this.eMailFC.disable();
    if (this.data.type == InputFormComponentElementType.NonConformiteCmt) { this.cmtVisible = true; }
    if (this.data.type == InputFormComponentElementType.LoginEmailLost) {
      this.eMailVisible = true;
      this.eMailFC.enable();
      this.buttonLabel = this.applicationUserContext.getCulturedRessourceText(676);
    }
    if (this.data.type == InputFormComponentElementType.NoEmail) {
      this.eMailVisible = true;
      this.eMailFC.enable();
    }
  }
  //-----------------------------------------------------------------------------------
  //Save data and close
  onSave() {
    //Close dialog
    switch (this.data.type) {
      case InputFormComponentElementType.NonConformiteCmt:
        this.dialogRef.close({ resp: this.cmtFC.value });
        break;
      case InputFormComponentElementType.LoginEmailLost:
        this.dialogRef.close({ resp: this.eMailFC.value });
        break;
      case InputFormComponentElementType.NoEmail:
        this.dialogRef.close({ resp: this.eMailFC.value });
        break;
      default:
        this.dialogRef.close({ resp: "" });
        break;
    }
  }
}
