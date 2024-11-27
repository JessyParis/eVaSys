import { Component, Inject, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ApplicationUserContext } from "../../../globals/globals";
import { HttpHeaders, HttpClient } from "@angular/common/http";
import { UntypedFormGroup, UntypedFormControl } from "@angular/forms";
import { ListService } from "../../../services/list.service";
import moment from "moment";
import * as dataModelsInterfaces from "../../../interfaces/dataModelsInterfaces";
import { showErrorToUser } from "../../../globals/utils";
import { MatDialog } from "@angular/material/dialog";

@Component({
    selector: "cible",
    templateUrl: "./cible.component.html",
    standalone: false
})

export class CibleComponent implements OnInit {
    form: UntypedFormGroup;
    filterCamionTypes: string="";
    camionTypeList: dataModelsInterfaces.CamionType[]=[];
    filterVilleArrivees: string = "";
    villeArriveeList: any[]=[];
    selectedCamionTypeList: number[];
    cibles: any[];
    @Input() refFournisseur: number;
    @Input() refTransporteur: number;
    @Input() refAdresseFournisseur: number;
    @Input() refProduit: number;
    @Input() refCamionType: number;
    @Input() refClient: number;
    @Input() dMoisDechargementPrevu: Date;
    @Input() type: string;
    @Output() askClose = new EventEmitter<any>();
    haut: number = 0;
    bas: number = 0;
    constructor(private http: HttpClient
        , @Inject("BASE_URL") private baseUrl: string
        , public applicationUserContext: ApplicationUserContext
        , private listService: ListService
        , public dialog: MatDialog
    ) { }
    //-----------------------------------------------------------------------------------
    //Data loading
    ngOnInit() {
        //Init form
        this.selectedCamionTypeList = [];
        if (this.refCamionType !== null) { this.selectedCamionTypeList.push(this.refCamionType); }
        //Get existing CamionType
        this.listService.getListCamionType().subscribe(result => {
            this.camionTypeList = result;
        }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        this.form = new UntypedFormGroup({
            CamionTypeList: new UntypedFormControl(this.selectedCamionTypeList),
            VilleArriveeList: new UntypedFormControl()
        });
        //Get carriages
        this.applyFilter();
    }
    //-----------------------------------------------------------------------------------
    //Get carriages
    getCarriages() {
        this.cibles = [];
        this.cibles.push({ RefTransport: null, RefAdresseDestination: null, Client: null, ResteALivrer: null, Positionne: null, Ville: null, RefTransporteur: null, Transporteur: null, CamionType: null, PUHT: null })
        if (this.refFournisseur && this.refFournisseur > 0 && this.refAdresseFournisseur && this.refAdresseFournisseur > 0
            && this.refProduit && this.refProduit > 0 && this.dMoisDechargementPrevu) {
            //Get data
            var url = this.baseUrl + "evapi/commandefournisseur/getcible";
            this.http.get<any[]>(url, {
                headers: new HttpHeaders()
                    .set("refFournisseur", this.refFournisseur.toString())
                    .set("refTransporteur", (this.refTransporteur ? this.refTransporteur.toString() : ""))
                    .set("refAdresseFournisseur", this.refAdresseFournisseur.toString())
                    .set("refProduit", this.refProduit.toString())
                    .set("filterCamionTypes", this.filterCamionTypes)
                    .set("filterVilleArrivees", this.filterVilleArrivees)
                    .set("refClient", (this.refClient ? this.refClient.toString(): ""))
                    .set("dMoisDechargementPrevu", moment(this.dMoisDechargementPrevu).format("YYYY-MM-DD 00:00:00.000")),
                responseType: "json"
            }).subscribe(result => {
                this.cibles = result;
                //Set min and max values
                let max: number = Math.max.apply(Math, this.cibles.map((r: any) => r.PUHT));
                let min: number = Math.min.apply(Math, this.cibles.map((r: any) => r.PUHT));
                this.haut = max - ((max - min) * 0.3);
                this.bas = max - ((max - min) * 0.7);
                //Set ville list
                this.villeArriveeList = []
                let villes = new Set(this.cibles.map(x => x.Ville))
                let villesTmp: any[] = [];
                villes.forEach(x => villesTmp.push({ Ville: x }));
                villesTmp = villesTmp.sort((obj1, obj2) => {
                        if (obj1.Ville > obj2.Ville) {
                            return 1;
                        }
                        if (obj1.Ville < obj2.Ville) {
                            return -1;
                        }
                        return 0;
                    });
                villesTmp.forEach(x => this.villeArriveeList.push(x));
            }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
        }
    }
    //-----------------------------------------------------------------------------------
    //Colouring
    coloredCible(item: any): string {
        let cssClass: string = "";
        if (item.PUHT > this.haut) { cssClass = "ev-cible-color-red"; }
        else if (item.PUHT < this.bas) { cssClass = "ev-cible-color-green"; }
        else { cssClass = "ev-cible-color-orange"; }
        return cssClass;
    }
    //-----------------------------------------------------------------------------------
    //Choose cible
    chooseCible(item: any) {
        //Emit event for dialog closing
        this.askClose.emit({ type: this.type, ref: item.RefTransport });
    }
    //-----------------------------------------------------------------------------------
    //Save filters
    saveFilter() {
        //Save filters
        if (this.form.get("CamionTypeList").value && this.form.get("CamionTypeList").value.length > 0) {
            this.filterCamionTypes = Array.prototype.map.call(this.form.get("CamionTypeList").value, function (item: number) { return item.toString(); }).join(",");
        }
        else {
            this.filterCamionTypes = "";
        }
        if (this.form.get("VilleArriveeList").value && this.form.get("VilleArriveeList").value.length > 0) {
            this.filterVilleArrivees = Array.prototype.map.call(this.form.get("VilleArriveeList").value, function (item: any) { return item.Ville; }).join(",");
        }
        else {
            this.filterVilleArrivees = "";
        }
    }
    //-----------------------------------------------------------------------------------
    //Apply filters
    applyFilter() {
        this.saveFilter();
        //Get carriages
        this.getCarriages();
    }
    //-----------------------------------------------------------------------------------
    //Manage multi select
    compareFnCamionType(item: number, selectedItem: number) {
        return item == selectedItem;
    }
    compareFnVilleArrivee(item: any, selectedItem: any) {
        return item && selectedItem ? item.Ville === selectedItem.Ville : item === selectedItem;
    }
}
