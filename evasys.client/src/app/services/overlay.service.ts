/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 22/05/2019
/// ----------------------------------------------------------------------------------------------------- 
///

import { Injectable, TemplateRef, ViewContainerRef } from "@angular/core";
import { Overlay, OverlayConfig, OverlayRef, PositionStrategy } from "@angular/cdk/overlay";
import { TemplatePortal } from "@angular/cdk/portal";

@Injectable()

export class OverlayService {
  constructor(
    private overlay: Overlay
  ) { }
  createOverlay(config: AppOverlayConfig): OverlayRef {
    return this.overlay.create(config);
  }
  attachTemplatePortal(overlayRef: OverlayRef, templateRef: TemplateRef<any>, vcRef: ViewContainerRef) {
    let templatePortal = new TemplatePortal(templateRef, vcRef);
    overlayRef.attach(templatePortal);
  }
  positionGloballyCenter(): PositionStrategy {
    return this.overlay.position()
      .global()
      .centerHorizontally()
      .centerVertically();
  }
  positionGloballyTop(): PositionStrategy {
    return this.overlay.position()
      .global()
      .centerHorizontally()
      .top();
  }
}

export interface AppOverlayConfig extends OverlayConfig { }
