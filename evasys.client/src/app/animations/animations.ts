/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 27/09/2018
/// ----------------------------------------------------------------------------------------------------- 
///

import { trigger, transition, style, query, animateChild, animate, group, state, keyframes } from "@angular/animations";

//------------------------------------------------------------------------------
//Animations for router actions
export const slideInAnimation =
    trigger("routeAnimations", [
        transition("Item => List", [
            style({ position: "relative" }),
            query(":enter, :leave", [
                style({
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%"
                })
            ]),
            query(":enter", [
                style({ left: "-100%" })
            ]),
            query(":leave", animateChild()),
            group([
                query(":leave", [
                    animate("300ms ease-out", style({ left: "100%" }))
                ]),
                query(":enter", [
                    animate("300ms ease-out", style({ left: "0%" }))
                ])
            ]),
            query(":enter", animateChild()),
        ]),

        transition("List => Item", [
            style({ position: "relative" }),
            query(":enter, :leave", [
                style({
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%"
                })
            ]),
            query(":enter", [
                style({ left: "100%" })
            ]),
            query(":leave", animateChild()),
            group([
                query(":leave", [
                    animate("300ms ease-out", style({ left: "-100%" }))
                ]),
                query(":enter", [
                    animate("300ms ease-out", style({ left: "0%" }))
                ])
            ]),
            query(":enter", animateChild()),
        ]),
        transition("* => FadeIn", [
            style({ position: "relative" }),
            query(":enter, :leave", [
                style({
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%"
                })
            ], { optional: true }),
            query(":enter", style({ opacity: 0 })),
            query(":leave", style({ opacity: 1 }), { optional: true }),
            query(":leave", animateChild(), { optional: true }),
            group([
                query(":leave", [
                    animate(600, style({ opacity: 0 }))
                ], { optional: true }),
                query(":enter", [
                    animate(600, style({ opacity: 1 }))
                ])
            ]),
            query(":enter", animateChild()),
        ]),
        transition("FadeIn => *", [
            style({ position: "relative" }),
            query(":enter, :leave", [
                style({
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%"
                })
            ]),
            query(":enter", style({ opacity: 0 })),
            query(":leave", style({ opacity: 1 })),
            query(":leave", animateChild()),
            group([
                query(":leave", [
                    animate(600, style({ opacity: 0 }))
                ]),
                query(":enter", [
                    animate(600, style({ opacity: 1 }))
                ])
            ]),
            query(":enter", animateChild()),
        ]),
    ]);
//------------------------------------------------------------------------------
//Animation for dashboard
export const dashboardAnimation =
  trigger('dashboardAnimationTrigger', [
    transition('0 => *', [
      animate('1000ms cubic-bezier(.17,.67,.27,1.26)', keyframes([
        style({ opacity: 0, transform: 'scale3d(.1, .1, .1)', offset: 0 }),
        style({ opacity: 1, transform: 'scale3d(1, 1, 1)', offset: 1 })
      ]))
    ])
  ]);
//------------------------------------------------------------------------------
//Animation for modules/main menu
export const modulesAnimation =
  trigger('modulesAnimationTrigger', [
    transition('* => open', [
      animate('500ms ease-out', keyframes([
        style({ opacity: 0, transform: 'scale3d(.1, .1, .1)', offset: 0 }),
        style({ opacity: 1, transform: 'scale3d(1, 1, 1)', offset: 1 })
      ]))
    ])
  ]);
//------------------------------------------------------------------------------
//Animation for global search results
export const searchResultAnimation =
  trigger('searchResultAnimationTrigger', [
    transition('void <=> *', [
      animate('500ms ease-out', keyframes([
        style({ opacity: 0, transform: 'scale3d(.1, .1, .1)', offset: 0 }),
        style({ opacity: 1, transform: 'scale3d(1, 1, 1)', offset: 1 })
      ]))
    ])
  ]);

