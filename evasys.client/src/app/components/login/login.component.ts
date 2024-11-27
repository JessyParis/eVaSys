import { Component, Inject, OnInit, Renderer2, inject, signal } from "@angular/core";
import { UntypedFormGroup, UntypedFormBuilder, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { AuthService } from "../../services/auth.service";
import { ApplicationUserContext } from "../../globals/globals"
import { DataModelService } from "../../services/data-model.service";
import { MatDialog } from "@angular/material/dialog";
import { changeCulture, showErrorToUser } from "../../globals/utils";
import moment from "moment";
import { InformationComponent } from "../dialogs/information/information.component";
import { DeviceDetectorService } from 'ngx-device-detector';
import { InputFormComponent } from "../dialogs/input-form/input-form.component";
import { InputFormComponentElementType } from "../../globals/enums";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { SnackBarQueueService } from "../../services/snackbar-queue.service";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { Utilisateur } from "../../interfaces/dataModelsInterfaces";
import { ComponentRelativeComponent } from "../dialogs/component-relative/component-relative.component";
import * as dataModelsInterfaces from "../../interfaces/dataModelsInterfaces";
import { DateAdapter, MAT_DATE_LOCALE } from '@angular/material/core';
@Component({
    selector: "login",
    templateUrl: "./login.component.html",
    standalone: false
})

export class LoginComponent implements OnInit {
  title: string;
  form: UntypedFormGroup;
  legalMentionEndYear: string;
  deviceInfo = null;
  hide: boolean = true;
  //Locale
  private readonly _adapter = inject<DateAdapter<unknown, unknown>>(DateAdapter);
  private readonly _locale = signal(inject<unknown>(MAT_DATE_LOCALE));  //Constructor
  constructor(private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") private baseUrl: string,
    private fb: UntypedFormBuilder,
    private authService: AuthService,
    public applicationUserContext: ApplicationUserContext
    , private dataModelService: DataModelService
    , private renderer: Renderer2
    , public dialog: MatDialog
    , private snackBarQueueService: SnackBarQueueService
    , private deviceService: DeviceDetectorService) {
    this.title = "Bienvenue sur l'application e-Valorplast";
    // initialize the form
    this.createForm();
    moment.locale(this.applicationUserContext.currentCultureName.substring(0, 2));
    this.legalMentionEndYear = moment().format("YYYY");
    this.deviceInfo = this.deviceService.getDeviceInfo();
    if (this.deviceInfo.browser != "Chrome" && this.deviceInfo.browser != "Firefox" && this.deviceInfo.browser != "MS-Edge-Chromium") {
      //Browser compatibility message
      const dialogRef = this.dialog.open(InformationComponent, {
        width: "550px",
        data: {
          title: this.applicationUserContext.getCulturedRessourceText(798)
          , message: this.applicationUserContext.getCulturedRessourceText(799)
          , browserCompat: true
        },
        closeOnNavigation: false,
        disableClose: true,
        restoreFocus: false
      });
    }
  }
  //Init
  ngOnInit() {
    //Logout
    this.authService.logout();
    //Autologin if applicable
    if (this.applicationUserContext.devLogin != "") {
      this.onLogin(this.applicationUserContext.devLogin, this.applicationUserContext.devPwd);
    }
  }
  //Form creation
  createForm() {
    this.form = this.fb.group({
      Login: ["", Validators.required],
      Pwd: ["", Validators.required]
    });
  }
  //Form submit
  onSubmit() {
    var login = this.form.value.Login;
    var pwd = this.form.value.Pwd;
    this.onLogin(login, pwd);
  }
  //Process login
  onLogin(login: string, pwd: string) {
    //Process
    this.authService.login(login, pwd, this.applicationUserContext.currentCultureName)
      .subscribe(res => {
        if (res.token != "" && res.expiration != 0 && !res.error) {
          // login successful
          //Get connected user
          this.dataModelService.getUserFromServer().subscribe(result => {
            this.applicationUserContext.connectedUtilisateur = result;
            //Save ref of the Maitre
            this.applicationUserContext.refUtilisateurMaitre = this.applicationUserContext.connectedUtilisateur.RefUtilisateur;
            //Force password modification if applicable
            this.dataModelService.mustModifyPassword().subscribe(result => {
              if (result > 0) {
                this.onUserAccount(result);
              }
              else {
                //Ask for email if applicable
                this.dataModelService.hasEmail().subscribe(result => {
                  if (result == false) {
                    this.onNoEmail();
                  }
                }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
                //Go to profils list if applicable, or connect the user
                if (this.applicationUserContext.connectedUtilisateur.HasProfils) {
                  this.onChangeProfil();
                }
                else {
                  this.authService.connectUser(this.applicationUserContext.connectedUtilisateur, this.dialog, null, null);
                }
              }
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
          }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
        else {
          // login failed because of credentials
          this.form.setErrors({
            "auth": this.applicationUserContext.getCulturedRessourceText(282)
          });
          // Inform user if login fails because login was used istead of email
          if (res.error == "UseEmail") {
            this.form.setErrors({
              "auth": this.applicationUserContext.getCulturedRessourceText(1104)
            });
          }
          // Inform user if login fails because account not authorized yet
          if (res.error == "UseEVa1") {
            this.form.setErrors({
              "auth": this.applicationUserContext.getCulturedRessourceText(1389)
            });
          }
          // Inform user if login fails because account has a Maitre
          if (res.error == "UseMaitre") {
            this.form.setErrors({
              "auth": this.applicationUserContext.getCulturedRessourceText(1507)
            });
          }
        }
      },
        err => {
          // login failed
          this.form.setErrors({
            "auth": this.applicationUserContext.getCulturedRessourceText(283)
          });
        });
  }
  //Process browser back button action
  onBack() {
    this.router.navigate(["login"]);
  }
  //Globalization
  onChangeCulture() {
    if (this.applicationUserContext.currentCultureName === "fr-FR") {
      changeCulture("en-GB", this.applicationUserContext);
      this._locale.set('en-gb');
    }
    else {
      changeCulture("fr-FR", this.applicationUserContext);
      this._locale.set('fr');
    }
    this._adapter.setLocale(this._locale());
  }
  //Password lost
  onPasswordLost() {
    //open pop-up
    const dialogRef = this.dialog.open(InputFormComponent, {
      width: "50%",
      data: {
        type: InputFormComponentElementType.LoginEmailLost,
        title: this.applicationUserContext.getCulturedRessourceText(1092),
        //eMail: this.form.value.Login
        eMail: ""
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        var url = this.baseUrl + "evapi/utils/passwordlost";
        this.http.get<boolean>(url, {
          headers: new HttpHeaders()
            .set("cultureName", this.applicationUserContext.currentCultureName),
          params: new HttpParams()
            .set("eMail", result.resp.toString()),
          responseType: "json"
        }).subscribe(res => {
          if (res) {
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1093), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
          else {
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(712), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    });
  }
  //No existing email for user
  onNoEmail() {
    //open pop-up
    const dialogRef = this.dialog.open(InputFormComponent, {
      width: "50%",
      data: {
        type: InputFormComponentElementType.NoEmail,
        title: this.applicationUserContext.getCulturedRessourceText(1100),
        eMail: ""
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.applicationUserContext.connectedUtilisateur.EMail = result.resp.toString();
        var url = this.baseUrl + "evapi/utilisateur";
        this.http.post<Utilisateur>(url, this.applicationUserContext.connectedUtilisateur, {
          responseType: "json"
        }).subscribe(res => {
          if (res.RefUtilisateur == this.applicationUserContext.connectedUtilisateur.RefUtilisateur) {
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(1101), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
          else {
            this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(712), duration: 4000 } as appInterfaces.SnackbarMsg);
          }
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
      }
    });
  }
  onUserAccount(refUtilisateur: number) {
    let data: any;
    //Init
    data = { title: this.applicationUserContext.getCulturedRessourceText(1122), type: "Utilisateur", ref: refUtilisateur };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      width: "350px",
      data,
      autoFocus: false,
      closeOnNavigation: false,
      disableClose: true,
      restoreFocus: false
    });
  }
  //------------------------------------------------------------------------------
  //Change profil
  onChangeProfil() {
    let data: any;
    let title: string = "";
    //Title
    title = this.applicationUserContext.getCulturedRessourceText(1500);
    //Calculate id
    data = {
      title: title, type: "Profils", changeProfil: false
    };
    //Open dialog
    const dialogRef = this.dialog.open(ComponentRelativeComponent, {
      data, 
      disableClose: true,
      closeOnNavigation: false
    });
    dialogRef.afterClosed().subscribe(result => {
      //If something have been done
      if (result) {
        if (result.ref) {
          this.router.navigate([result.ref]);
        }
      }
      else {
        this.router.navigate(["login"]);
      }
    })
  }
}
