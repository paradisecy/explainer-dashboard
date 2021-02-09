import { Injectable } from '@angular/core';
import { Observable } from "rxjs";
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SolutionService  {
  baseController = 'solution';
  processController = 'process';
  currentSolution:any = {id:"0", name : "", kb: "" , coach :"", explain : ""}
  constructor(private http: HttpClient) {
  }

  create(params): Observable<object> {
    return this.http.post(`${this.baseController}/create-solution`, params);
  }

  process(params): Observable<object> {
    return this.http.post(`${this.processController}/process-solution`, params);
  }


  save(params): Observable<object> {
    return this.http.post(`${this.baseController}/save-solution`, params);
  }


  delete(params): Observable<object> {
    return this.http.post(`${this.baseController}/delete-solution`, params);
  }
  //delete(params: IDriver): Observable<object> {
  //  return this.http.post(`${this.baseController}/delete-driver`, params);
  //}

  //update(params: IUpdatedFields): Observable<object> {
  //  return this.http.post(`${this.baseController}/update-driver`, params);
  //}

  get(): Observable<object> {
    return this.http.get(`${this.baseController}/get-solutions`);
  }

}
