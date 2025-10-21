/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 20/12/2018
/// ----------------------------------------------------------------------------------------------------- 
import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders, HttpHeaderResponse } from "@angular/common/http";
import { Subject } from "rxjs";
import * as appInterfaces from "../interfaces/appInterfaces";

const url = "evapi/upload/";

@Injectable()
//-----------------------------------------------------------------------------------
//Upload service for file(s) upload
export class UploadService {
  //-----------------------------------------------------------------------------------
  //Constructor
  constructor(private http: HttpClient, @Inject("BASE_URL") private baseUrl: string) { }

  public upload(files: Set<File>, type: string, fileType: number, ref: string): any {
    // this will be the our resulting map
    const status: any = {};
    const res: any = {};
    files.forEach(file => {
      // create a new multipart-form for every file
      const formData: FormData = new FormData();
      formData.append("file", file, file.name);
      // create a http-post request and pass the form
      // tell it to report the upload progress
      const req = new HttpRequest("POST", this.baseUrl + url + type
        , formData, {
        headers: new HttpHeaders()
          .set("ref", ref.toString())
          .set("fileType", fileType.toString()),
        reportProgress: true,
      });
      // create a new progress-subject for every file
      const progress = new Subject<number>();
      const uploadBodyResult = new Subject<appInterfaces.UploadResponseBody>();

      // send the http-request and subscribe for progress-updates
      //let startTime = new Date().getTime();
      this.http.request(req).subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          // calculate the progress percentage
          const percentDone = Math.round((100 * event.loaded) / event.total);
          // pass the percentage into the progress-stream
          progress.next(percentDone);
        } else if (event instanceof HttpResponse) {
          // Close the progress-stream if we get an answer form the API
          // The upload is complete
          progress.complete();
          const b = <appInterfaces.UploadResponseBody>event.body;
          uploadBodyResult.next(b);
          uploadBodyResult.complete();

        } else if (event instanceof HttpHeaderResponse) {
          if (event.status != 200) {
            // Close the progress-stream if we get an answer form the API
            // The upload is complete
            progress.complete();
            const b: appInterfaces.UploadResponseBody = {
              error: "631",
              message: "",
              fileName: file.name,
              fileDbId: 0,
              uploadedFiles: undefined
            };
            uploadBodyResult.next(b);
            uploadBodyResult.complete();

          }
        }
      });

      // Save every progress-observable in a map of all observables
      status[file.name] = {
        progress: progress.asObservable()
      };

      // Save every result-observable in a map of all observables
      res[file.name] = {
        uploadBodyResult: uploadBodyResult.asObservable()
      };
    });

    // return the map of progress.observables
    return { status: status, res: res };
  }
}

