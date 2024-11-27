import { Component, Input} from "@angular/core";
import { UntypedFormGroup } from "@angular/forms";
import { ApplicationUserContext } from "../../globals/globals";
import { ErrorStateMatcher } from "@angular/material/core";
import { EnvComponentControl } from "../../classes/appClasses";
import { FormControlType } from "../../globals/enums";

@Component({
    selector: "env-control",
    templateUrl: "./env-control.component.html",
    standalone: false
})
export class EnvControlComponent {
  constructor(public applicationUserContext: ApplicationUserContext) {
  }
  @Input() matcher : ErrorStateMatcher;
  @Input() form: UntypedFormGroup;
  @Input() ctrl: EnvComponentControl;

  CanReset(): boolean {
    return this.ctrl.FC.value != null && this.ctrl.FC.enabled && !this.IsCheckbox();
  }
  WarnRequired(): boolean {
    return (this.ctrl.FC.touched || this.ctrl.FC.dirty) && this.ctrl.FC.errors?.required;
  }
  Reset() {
    this.ctrl.FC.setValue(null);
  }
  description() {
    return this.applicationUserContext.getCulturedRessourceText(this.ctrl.ress);
  }
  IsRequired(): boolean {
    return this.ctrl.required;
  }
  IsInput(): boolean {
    return this.ctrl.type == FormControlType.Texte;
  }
  IsSelect(): boolean {
    return this.ctrl.type == FormControlType.List;
  }
  IsTextArea(): boolean {
    return this.ctrl.type == FormControlType.TexteMulti;
  }
  IsCheckbox(): boolean {
    return this.ctrl.type == FormControlType.Checkbox;
  }
  IsDatePicker(): boolean {
    return this.ctrl.type == FormControlType.DatePicker;
  }
}
