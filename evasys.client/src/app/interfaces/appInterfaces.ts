/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast (logiciel de gestion d'activité)
/// Création : 01/10/2018
/// ----------------------------------------------------------------------------------------------------- 
///

import { SeriesType } from "@progress/kendo-angular-charts";

export interface TokenResponse {
  token: string;
  expiration: number;
  error: string;
}
export interface ResetPassword {
  cultureName: string;
  error: boolean;
}
export interface UploadResponseBody {
  fileName: string;
  error: string;
  message: string;
  fileDbId: number;
  uploadedFiles: any; //Array of successfully uploaded files
}
export class CulturedRessource {
  refRessource: number;
  textRessource: string;
  textJavascriptRessource: string;
  hTMLRessource: string;
  hTMLButtonRessource: string;
}
export class Resource {
  name: string;
  value: string;
}
export class DashboardItem {
  nb: number;
  nbUrgent: number;
}
export class Month {
  d: Date | string;
  n: number;
  name: string;
  Name?: string;
  Nb?: number;
  Year?: number;
  Begin?: string | moment.Moment;
  End?: string | moment.Moment;
}
export class Quarter {
  n: number;
  name: string;
  Name?: string;
  Nb?: number;
  NameShort?: string;
  Year?: number;
  Begin?: string | moment.Moment;
  End?: string | moment.Moment;
}
export class Year {
  y: number;
}
export class YesNo {
  value: boolean;
  text: string;
}
export interface MatTableColumn {
  columnDef: string;
  header: string;
  sticky: boolean;
  stickyEnd: boolean;
  cell: (item: any) => any;
}
export interface SnackbarMsg {
  text: string;
  duration: number;
  type?: string;
}
export interface StringBody {
  val: string;
}
export interface SearchResultDetails {
  resultType: string;
  description: string;
  actif: boolean;
}
export interface ChartSerie {
  type: SeriesType;
  data: any[];
  field: string;
  categoryField: string;
  name: string;
  color?: any;
}
export interface AppLink {
  valid: boolean;
  componentName: string;
  componentRef: number;
  navExtra;
}
