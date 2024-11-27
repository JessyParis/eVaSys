import { Component, OnInit, Input } from "@angular/core";
import { Router } from "@angular/router";
import { ApplicationUserContext } from "../../globals/globals";
import * as appInterfaces from "../../interfaces/appInterfaces";
import { showErrorToUser } from "../../globals/utils";
import { MatDialog } from "@angular/material/dialog";
import { UtilsService } from "../../services/utils.service";
import { SearchResultType } from "../../globals/enums";

@Component({
    selector: "search-result",
    templateUrl: "./search-result.component.html",
    standalone: false
})

export class SearchResultComponent implements OnInit {
  boxClass: string ="boxSearchResultStandard"
  //Form
  searchResultDetails: appInterfaces.SearchResultDetails = {} as appInterfaces.SearchResultDetails;
  //Entries
  @Input() elementRef: string;
  @Input() elementType: string;
  @Input() searchText: string;
  //Constructor
  constructor(private router: Router
    , public applicationUserContext: ApplicationUserContext
    , private utilsService: UtilsService
    , public dialog: MatDialog) {
  }
  //-----------------------------------------------------------------------------------
  //Init
  ngOnInit() {
    let elRef = Number.parseInt(this.elementRef, 10);
    //Check parameters
    if (!isNaN(elRef)) {
      this.utilsService.getSearchResultDetails(elRef, this.elementType, this.searchText).subscribe(result => {
        //Get data
        this.searchResultDetails = result;
        //Set style
        switch (this.searchResultDetails.resultType) {
          case SearchResultType.CDT:
            this.boxClass = "boxSearchResultBlue";
            break;
          case SearchResultType.Client:
            this.boxClass = "boxSearchResultYellow";
            break;
          case SearchResultType.Collectivite:
            this.boxClass = "boxSearchResultGreen";
            break;
          case SearchResultType.Transporteur:
            this.boxClass = "boxSearchResultRed";
            break;
        }
        //Style inactif if applicable
        if (this.searchResultDetails.actif == false) {
          this.boxClass += " color-inactif";
        }
      }, error => showErrorToUser(this.dialog, error, this.applicationUserContext));
    }
    else {
      //Route back to list
      this.router.navigate(["grid"]);
    }
  }
}
