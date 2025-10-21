import { Component, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { SnackbarMsg } from '../../../interfaces/appInterfaces';
import { SnackbarMsgType } from '../../../globals/enums';

@Component({
  selector: 'app-snackbar',
  templateUrl: './snackbar.component.html',
  styleUrls: ['./snackbar.component.scss'],
  standalone: false
})
export class SnackbarComponent {
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: SnackbarMsg) {
    if (!data.type) {
      data.type = SnackbarMsgType.Info;
    }
  }

  getIcon(type: string): string {
    switch (type) {
      case SnackbarMsgType.Success:
        return 'check_circle';
      case SnackbarMsgType.Error:
        return 'error';
      case SnackbarMsgType.Warning:
        return 'warning';
      case SnackbarMsgType.Info:
        return 'info';
      default:
        return 'info';
    }
  }
}
