export interface Pagination
{
    currentPage : number;
    itemPerPage : number;
    totalItems : number;
    totalPages : number;
}

export class PaginationResult<T>{
    pagination : Pagination;
    result : T;
}