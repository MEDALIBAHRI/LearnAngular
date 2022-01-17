import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginationResult } from "../_modules/pagination";

export function getPaginatedResult<T>(url,  params: HttpParams, http : HttpClient) {
    const paginationResult : PaginationResult<T> = new PaginationResult<T>();
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        paginationResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginationResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginationResult;
      })
    );
  }

  export function getPaginationHeader(pageNumber : number, pageSize : number)
  {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return params;
  }