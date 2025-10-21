import { Component } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { UploadComponent } from "../../dialogs/upload/upload.component";
import { ApplicationUserContext } from "../../../globals/globals";
import * as appInterfaces from "../../../interfaces/appInterfaces";
import { SnackBarQueueService } from "../../../services/snackbar-queue.service";
import { SnackbarMsgType } from "../../../globals/enums";

@Component({
    selector: "upload-transport",
    templateUrl: "./upload-transport.component.html",
    standalone: false
})

export class UploadTransportComponent {
  constructor(public dialog: MatDialog
    , private snackBarQueueService: SnackBarQueueService
    , public applicationUserContext: ApplicationUserContext
  ) {
  }
  lastUploadResut: appInterfaces.UploadResponseBody;
  //-----------------------------------------------------------------------------------
  //Open dialog for uploading file
  import() {
    const dialogRef = this.dialog.open(UploadComponent, {
      width: "50%", height: "50%",
      data: {
        title: this.applicationUserContext.getCulturedRessourceText(339), message: this.applicationUserContext.getCulturedRessourceText(340)
        , multiple: false, type: "transport", ref: "none", fileType: "none"
      },
      autoFocus: false,
      restoreFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      this.lastUploadResut = result;
      if (!this.lastUploadResut) {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(341), duration: 4000, type: SnackbarMsgType.Error } as appInterfaces.SnackbarMsg);
      }
      else if (this.lastUploadResut.error != "") {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(342), duration: 4000, type: SnackbarMsgType.Error } as appInterfaces.SnackbarMsg);
      }
      else {
        this.snackBarQueueService.addMessage({ text: this.applicationUserContext.getCulturedRessourceText(343), duration: 4000 } as appInterfaces.SnackbarMsg);
      }
    });
  }
}
