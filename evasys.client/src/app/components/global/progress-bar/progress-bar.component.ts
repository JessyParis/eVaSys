import { Component, Input, OnInit, ViewChild, TemplateRef, ViewContainerRef, DoCheck } from "@angular/core";
import { ThemePalette } from "@angular/material/core";
import { ProgressBarMode } from "@angular/material/progress-bar";
import { OverlayRef } from "@angular/cdk/overlay";
import { OverlayService, AppOverlayConfig } from "../../../services/overlay.service";

@Component({
    selector: "progress-bar",
    templateUrl: "./progress-bar.component.html",
    standalone: false
})
export class ProgressBarComponent {
  @Input() color?: ThemePalette;
  @Input() mode?: ProgressBarMode;
  @Input() value?: number;
  @Input() backdropEnabled = true;
  @Input() position = "GloballyCenter";
  @Input() displayProgressBar: boolean;

  @ViewChild("progressBarRef", { static: true })
  private progressBarRef: TemplateRef<any>;
  private progressBarOverlayConfig: AppOverlayConfig;
  private overlayRef: OverlayRef;
  constructor(private vcRef: ViewContainerRef, private overlayService: OverlayService) { }
  ngOnInit() {
    // Config for Overlay Service
    this.progressBarOverlayConfig = {
      hasBackdrop: this.backdropEnabled
    };
    if (this.position == "GloballyCenter") {
      this.progressBarOverlayConfig["positionStrategy"] = this.overlayService.positionGloballyCenter();
    }
    else if (this.position == "GloballyTop") {
      this.progressBarOverlayConfig["positionStrategy"] = this.overlayService.positionGloballyTop();
    }
    // Create Overlay for progress bar
    this.overlayRef = this.overlayService.createOverlay(this.progressBarOverlayConfig);
  }
  ngDoCheck() {
    // Based on status of displayProgressBar attach/detach overlay to progress bar template
    //this.manageDisplay();
  }
  manageDisplay(): boolean {
    // Based on status of displayProgressBar attach/detach overlay to progress bar template
    if (this.displayProgressBar && !this.overlayRef.hasAttached()) {
      this.overlayService.attachTemplatePortal(this.overlayRef, this.progressBarRef, this.vcRef);
    } else if (!this.displayProgressBar && this.overlayRef.hasAttached()) {
      this.overlayRef.detach();
    }
    return true;
  }
  stop(): boolean {
    // Based on status of displayProgressBar attach/detach overlay to progress bar template
    if (this.overlayRef.hasAttached()) {
      this.overlayRef.detach();
    }
    return true;
  }
  start(): boolean {
    // Based on status of displayProgressBar attach/detach overlay to progress bar template
    if (!this.overlayRef.hasAttached()) {
      this.overlayService.attachTemplatePortal(this.overlayRef, this.progressBarRef, this.vcRef);
    }
    return true;
  }
}
